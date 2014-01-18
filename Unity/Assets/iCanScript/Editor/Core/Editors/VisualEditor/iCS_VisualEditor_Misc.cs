#define NEW_RECONNECTION
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Properties.
    // ----------------------------------------------------------------------
    iCS_EditorObject SelectedObjectBeforeMouseDown= null;
    iCS_EditorObject myBookmark= null;
	bool			 ShouldRotateMuxPort= false;
    

    // ======================================================================
	// ----------------------------------------------------------------------
    // Manages the object selection.
    iCS_EditorObject DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        iCS_EditorObject newSelected= GetObjectAtMousePosition();
		if(SelectedObject != null && newSelected != null &&
		   newSelected.IsMuxPort && SelectedObject.IsMuxPort &&
		   IStorage.GetParentMuxPort(newSelected) == IStorage.GetParentMuxPort(SelectedObject)) {
			ShouldRotateMuxPort= true;
			return SelectedObject;
		}
		ShouldRotateMuxPort= false;
        SelectedObject= newSelected;
        ShowInstanceEditor();
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    void RotateSelectedMuxPort() {
		if(SelectedObject == null || !SelectedObject.IsMuxPort) return;
		if(SelectedObject.IsParentMuxPort) {
			IStorage.UntilMatchingChild(SelectedObject, 
				c=> {
					if(c.IsChildMuxPort) {
						SelectedObject= c;
						return true;
					}
					return false;
				}
			);
			return;
		}
		iCS_EditorObject parentMuxPort= SelectedObject.Parent;
		if(!parentMuxPort.IsParentMuxPort) return;
		bool takeNext= false;
		bool found= IStorage.UntilMatchingChild(parentMuxPort,
			c=> {
				if(takeNext) {
					SelectedObject= c;
					return true;
				}
				if(c == SelectedObject) takeNext= true;
				return false;
			}
		);
		if(!found) SelectedObject= parentMuxPort;
	}
	
	// ----------------------------------------------------------------------
    bool VerifyNewDragConnection() {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= IStorage.GetOverlappingPort(DragObject);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!DragFixPort.IsDataOrControlPort || !overlappingPort.IsDataOrControlPort) return false;
        // Destroy drag port since it is not needed anymore.
        IStorage.DestroyInstance(DragObject);
        DragObject= null;
        return VerifyNewConnection(DragFixPort, overlappingPort);
    }
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject overlappingPort) {
        // Only data ports can be connected together.
        if(!fixPort.IsDataOrControlPort || !overlappingPort.IsDataOrControlPort) return false;
        // Verify for Mux port creation
        if(overlappingPort.IsRelayPort) {
            CreateMuxPort(fixPort, overlappingPort);
            return true;
        }
        iCS_EditorObject overlappingPortParent= overlappingPort.Parent;
        if(overlappingPort.IsOutputPort && 
		   overlappingPort.Source != null &&
		   (overlappingPortParent.IsKindOfPackage || overlappingPortParent.IsKindOfState)) {
            CreateMuxPort(fixPort, overlappingPort);
            return true;
        }
        
        // Connect function & modules ports together.
        iCS_EditorObject portParent= fixPort.Parent;
        iCS_EditorObject inPort = null;
        iCS_EditorObject outPort= null;

        bool portIsChildOfOverlapping= IStorage.IsChildOf(portParent, overlappingPortParent);
        bool overlappingIsChildOfPort= IStorage.IsChildOf(overlappingPortParent, portParent);
        if(portIsChildOfOverlapping || overlappingIsChildOfPort) {
            if(fixPort.IsInputPort && overlappingPort.IsInputPort) {
                if(portIsChildOfOverlapping) {
                    inPort= fixPort;
                    outPort= overlappingPort;
                } else {
                    inPort= overlappingPort;
                    outPort= fixPort;
                }
            } else if(fixPort.IsOutputPort && overlappingPort.IsOutputPort) {
                if(portIsChildOfOverlapping) {
                    inPort= overlappingPort;
                    outPort= fixPort;
                } else {
                    inPort= fixPort;
                    outPort= overlappingPort;
                }                    
            } else {
                ShowNotification(new GUIContent("Cannot connect nested node ports from input to output !!!"));
                return true;
            }
        } else {
            inPort = fixPort.IsInputPort          ? fixPort : overlappingPort;
            outPort= overlappingPort.IsOutputPort ? overlappingPort : fixPort;
        }
        if(inPort != outPort) {
            iCS_TypeCastInfo conversion= null;
            if(VerifyConnectionTypes(inPort, outPort, out conversion)) {
                SetNewDataConnection(inPort, outPort, conversion);                
            }
        } else {
            string direction= inPort.IsInputPort ? "input" : "output";
            ShowNotification(new GUIContent("Cannot connect an "+direction+" port to an "+direction+" port !!!"));
        }
        return true;
    }
	// ----------------------------------------------------------------------
    bool VerifyConnectionTypes(iCS_EditorObject inPort, iCS_EditorObject outPort, out iCS_TypeCastInfo typeCast) {
        typeCast= null;
		Type inType= inPort.RuntimeType;
		Type outType= outPort.RuntimeType;
        if(iCS_Types.CanBeConnectedWithoutConversion(outType, inType)) { // No conversion needed.
            return true;
        }
        // A conversion is required.
		if(iCS_Types.CanBeConnectedWithUpConversion(outType, inType)) {
			if(EditorUtility.DisplayDialog("Up Conversion Connection", "Are you sure you want to generate a conversion from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)+"?", "Generate Conversion", "Abort")) {
                return true;
			}
            return false;
		}
        typeCast= iCS_LibraryDatabase.FindTypeCast(outType, inType);
        if(typeCast == null) {
			ShowNotification(new GUIContent("No automatic type conversion exists from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)));
            return false;
        }
        return true;
    }
	// ----------------------------------------------------------------------
	void CreateMuxPort(iCS_EditorObject fixPort, iCS_EditorObject parentMuxPort) {
        iCS_TypeCastInfo conversion= null;
        if(!VerifyConnectionTypes(parentMuxPort, fixPort, out conversion)) return;
        var childPortType= parentMuxPort.IsInputPort ? iCS_ObjectTypeEnum.InChildMuxPort : iCS_ObjectTypeEnum.OutChildMuxPort;
		var source= parentMuxPort.Source;
		// Convert source port to child port.
		if(source != null) {
			var firstChildMux= IStorage.CreatePort(fixPort.Name, parentMuxPort.InstanceId, parentMuxPort.RuntimeType, childPortType);
			IStorage.SetSource(firstChildMux, source);
			IStorage.SetSource(parentMuxPort, null);
			parentMuxPort.ObjectType= parentMuxPort.IsInputPort ? iCS_ObjectTypeEnum.InParentMuxPort : iCS_ObjectTypeEnum.OutParentMuxPort;
		}
		// Create new mux input port.
		var childMuxPort= IStorage.CreatePort(fixPort.Name, parentMuxPort.InstanceId, parentMuxPort.RuntimeType, childPortType);
		SetNewDataConnection(childMuxPort, fixPort, conversion);
		IStorage.CleanupMuxPort(parentMuxPort);
	}
	// ----------------------------------------------------------------------
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_TypeCastInfo conversion= null) {
		iCS_EditorObject inParentNode  = inPort.ParentNode;
        iCS_EditorObject outParentNode = outPort.ParentNode;
        iCS_EditorObject inGrandParent = inParentNode.ParentNode;        
        iCS_EditorObject outGrandParent= outParentNode.ParentNode;

        // No need to create module ports if both connected nodes are under the same parent.
        if(inGrandParent == outGrandParent || inGrandParent == outParentNode || inParentNode == outGrandParent) {
            IStorage.SetSource(inPort, outPort, conversion);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= outGrandParent.ParentNode; op != null; op= op.ParentNode) {
            if(inGrandParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inGrandParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, inGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
            IStorage.SetSource(inPort, newPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
            SetNewDataConnection(newPort, outPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= inGrandParent.ParentNode; ip != null; ip= ip.ParentNode) {
            if(outGrandParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outGrandParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, outGrandParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
            IStorage.SetSource(newPort, outPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.LayoutPosition, outPort.LayoutPosition);
            SetNewDataConnection(inPort, newPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        IStorage.SetSource(inPort, outPort, conversion);
        IStorage.OptimizeDataConnection(inPort, outPort);
    }
	// ----------------------------------------------------------------------
	// This attempt to properly locate an autocreated data port.
	void SetBestPositionForAutocreatedPort(iCS_EditorObject port, Vector2 inPortPosition, Vector2 outPortPosition) {
		// Determine the parent edge position to use.
		var parent= port.Parent;
		var parentGlobalRect= parent.LayoutRect;
		float x= port.IsInputPort ? parentGlobalRect.xMin : parentGlobalRect.xMax;
		// Assure that the in position X value is smaller then the out position.
		if(inPortPosition.x > outPortPosition.x) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Manage situation where new port is between the in & out ports.
		var parentGlobalPosition= Math3D.Middle(parentGlobalRect);
		var top   = parentGlobalPosition.y+parent.VerticalPortsTop;
		var bottom= parentGlobalPosition.y+parent.VerticalPortsBottom;
		float y;
		if(Math3D.IsSmaller(inPortPosition.x, x) && Math3D.IsGreater(outPortPosition.x, x)) {
			float ratio= (x-inPortPosition.x)/(outPortPosition.x-inPortPosition.x);
			y= Math3D.Lerp(inPortPosition.y, outPortPosition.y, ratio);
			if(y < top) { 
				y= top;
			}
			if(y > bottom) {
				y= bottom;
			}
			port.SetAnchorAndLayoutPosition(new Vector2(x,y));
			return;
		}
		if(Math3D.IsEqual(inPortPosition.y, outPortPosition.y)) {
			port.SetAnchorAndLayoutPosition(new Vector2(x, 0.5f*(top+bottom)));
			return;
		}
		// Assure that the in position Y value is smaller then the out position.
		if(inPortPosition.y > outPortPosition.y) {
			var tmp= inPortPosition; inPortPosition= outPortPosition; outPortPosition= tmp;
		}
		// Compute some type of ratio if Y position traverse the top port position
		if(Math3D.IsSmaller(inPortPosition.y, top) && Math3D.IsGreater(outPortPosition.y, top)) {
			float y1= outPortPosition.y-top;
			float y2= top-inPortPosition.y;
			y= top+(y1*y1/(y1+y2));
		} else {
			float y2= outPortPosition.y-bottom;
			float y1= bottom-inPortPosition.y;
			y= bottom-(y1*y1/(y1+y2));			
		}
		port.SetAnchorAndLayoutPosition(new Vector2(x,y));
		return;			
	}

	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= edObj.Parent;
        for(; parentModule != null && !parentModule.IsKindOfPackage; parentModule= parentModule.Parent);
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(Vector2 point, iCS_ObjectTypeEnum objType, string objName) {
        iCS_EditorObject newParent= IStorage.GetNodeAt(point);
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(objName, objType, newParent, IStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(Vector2 graphPos, iCS_EditorObject node) {
        if(!node.IsNode) return null;
        iCS_EditorObject newParent= IStorage.GetNodeAt(graphPos, node);
        if(newParent == node.Parent) return newParent;
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(node.Name, node.ObjectType, newParent, IStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= node.Parent;
        if(newParent == null || newParent == oldParent) return;
		// Change parent and relayout.
		var nodePos= node.LayoutPosition;
		node.Parent= newParent;
		node.SetAnchorAndLayoutPosition(nodePos);
		node.LayoutNode();
		node.LayoutParentNodesUntilTop(iCS_AnimationControl.Always);
		if(node.IsState) CleanupEntryState(node, oldParent);
#if NEW_RECONNECTION
        RebuildConnectionsFor(node);
#else
		CleanupConnections(node);
#endif
    }
	// ----------------------------------------------------------------------
	void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= state.Parent;
		bool anEntryExists= false;
		IStorage.ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		IStorage.ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			IStorage.UntilMatchingChild(prevParent,
				child=> {
					if(child.IsState) {
						child.IsEntryState= true;
						return true;
					}
					return false;
				}
			);
		}
	}
#if NEW_RECONNECTION
	// ----------------------------------------------------------------------
	void RebuildConnectionsFor(iCS_EditorObject node) {
		node.ForEachChildPort(
			p=> {
			    if(p.IsDataOrControlPort) {
    				var srcEndPoint= p.SourceEndPort;
    				foreach(var dep in p.DestinationEndPoints) {
    					RebuildDataConnection(srcEndPoint, dep);
    				}			        
			    }
			    if(p.IsStatePort) {
			        var fromState= IStorage.GetFromStatePort(p);
			        var toState  = IStorage.GetToStatePort(p);
			        RebuildStateConnection(fromState, toState);
			    }
			}
		);
	}
	// ----------------------------------------------------------------------
	void RebuildDataConnection(iCS_EditorObject srcEndPoint, iCS_EditorObject dstPort) {
		// Have we completed rebuilding ... if so return.
		if(dstPort == srcEndPoint) return;
		var dstNode= dstPort.ParentNode;
		var commonParentNode= srcEndPoint.GetCommonParent(dstPort);
		if(dstNode == commonParentNode) {
			// Rebuild moving down from the common parent towards the source port.
			var newDstNode= srcEndPoint.ParentNode;
			while(newDstNode != dstNode && newDstNode.ParentNode != dstNode) {
				newDstNode= newDstNode.ParentNode;
			}
			var existingPort= FindPortWithSourceEndPoint(newDstNode, srcEndPoint);
			if(existingPort != null) {
				var prevSource= dstPort.Source;
				if(prevSource != existingPort) {
					dstPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !dstPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(srcEndPoint, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(dstPort.Name, newDstNode.InstanceId, dstPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, srcEndPoint.LayoutPosition, dstPort.LayoutPosition);
				newPort.Source= dstPort.Source;
				dstPort.Source= newPort;
				RebuildDataConnection(srcEndPoint, newPort);				
			}			
			return;
		}
		var dstNodeParent= dstNode.ParentNode;
		if(dstNodeParent == commonParentNode) {
			// Rebuild traversing from moving upwards to downwords.
			var newDstNode= srcEndPoint.ParentNode;
			while(newDstNode != commonParentNode && newDstNode.ParentNode != commonParentNode) {
				newDstNode= newDstNode.ParentNode;
			}
			var existingPort= FindPortWithSourceEndPoint(newDstNode, srcEndPoint);
			if(existingPort != null) {
				var prevSource= dstPort.Source;
				if(prevSource != existingPort) {
					dstPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !dstPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(srcEndPoint, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(dstPort.Name, newDstNode.InstanceId, dstPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, srcEndPoint.LayoutPosition, dstPort.LayoutPosition);
				newPort.Source= dstPort.Source;
				dstPort.Source= newPort;
				RebuildDataConnection(srcEndPoint, newPort);				
			}
			return;
		} else {
			// Rebuilding moving up from the destination port towards the common parent.
			var existingPort= FindPortWithSourceEndPoint(dstNodeParent, srcEndPoint);
			if(existingPort != null) {
				var prevSource= dstPort.Source;
				if(prevSource != existingPort) {
					dstPort.Source= existingPort;
					if(prevSource.IsDynamicDataPort && !dstPort.IsPartOfConnection(prevSource)) {
						CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(srcEndPoint, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(dstPort.Name, dstNodeParent.InstanceId, dstPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
				SetBestPositionForAutocreatedPort(newPort, srcEndPoint.LayoutPosition, dstPort.LayoutPosition);
				newPort.Source= dstPort.Source;
				dstPort.Source= newPort;
				RebuildDataConnection(srcEndPoint, newPort);
			}			
		}
	}
    // ----------------------------------------------------------------------
	void CleanupHangingConnection(iCS_EditorObject port) {
		if(port.Destinations.Length == 0) {
			var src= port.Source;
			IStorage.DestroyInstance(port);
			if(src != null) {
				CleanupHangingConnection(src);
			}
		}
	}
	// ----------------------------------------------------------------------
	void RebuildStateConnection(iCS_EditorObject fromStatePort, iCS_EditorObject toStatePort) {
		var commonParent= fromStatePort.GetCommonParent(toStatePort);
		if(commonParent == null) {
			Debug.LogWarning("iCanScript: Unable to find common parent after relocating state !!!");
			return;
		}
		var transitionPackage= IStorage.GetTransitionPackage(toStatePort);
		if(transitionPackage == null) return;
		if(transitionPackage.ParentNode == commonParent) return;
		ChangeParent(transitionPackage, commonParent);
		IStorage.LayoutTransitionPackage(transitionPackage);
	}
#endif
    // ----------------------------------------------------------------------
	iCS_EditorObject FindPortWithSourceEndPoint(iCS_EditorObject node, iCS_EditorObject srcEP) {
		iCS_EditorObject result= null;
		node.UntilMatchingChild(
			p=> {
				if(p.IsPort && p.SourceEndPort == srcEP) {
					result= p;
					return true;
				}
				return false;
			}
		);
		return result;
	}
    // ----------------------------------------------------------------------
    void CleanupConnections(iCS_EditorObject node) {
        switch(node.ObjectType) {
            case iCS_ObjectTypeEnum.StateChart: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;                
            }
            case iCS_ObjectTypeEnum.State: {
                // Attempt to relocate transition modules.
                IStorage.ForEachChildPort(node,
                    p=> {
                        if(p.IsStatePort) {
                            iCS_EditorObject transitionPackage= null;
                            if(p.IsInStatePort) {
                                transitionPackage= p.Source.Parent;
                            } else {
                                iCS_EditorObject[] connectedPorts= p.Destinations;
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionPackage= cp.Parent;
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= IStorage.GetFromStatePort(transitionPackage).Parent;
                            iCS_EditorObject inState= IStorage.GetToStatePort(transitionPackage).Parent;
                            iCS_EditorObject newParent= IStorage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != transitionPackage.Parent) {
                                ChangeParent(transitionPackage, newParent);
                                IStorage.LayoutTransitionPackage(transitionPackage);
                            }
                        }
                    }
                );
                // Ask our children to cleanup their connections.
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionPackage:
            case iCS_ObjectTypeEnum.Package: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                goto case iCS_ObjectTypeEnum.InstanceFunction;
            }
            case iCS_ObjectTypeEnum.InstanceFunction:
            case iCS_ObjectTypeEnum.ClassFunction:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.ClassField:
            case iCS_ObjectTypeEnum.TypeCast: {
                IStorage.ForEachChildPort(node,
                    port=> {
                        if(port.IsInDataOrControlPort) {
                            iCS_EditorObject sourcePort= RemoveConnection(port);
                            // Rebuild new connection.
                            if(sourcePort != port) {
                                SetNewDataConnection(port, sourcePort);
                            }
                        }
                        if(port.IsOutDataOrControlPort) {
							// FIXME: Removing & recreating ports looses user position.
                            iCS_EditorObject[] allInPorts= FindAllConnectedInDataPorts(port);
                            foreach(var inPort in allInPorts) {
                                RemoveConnection(inPort);
                            }
                            foreach(var inPort in allInPorts) {
                                if(inPort != port) {
                                    SetNewDataConnection(inPort, port);                                    
                                }
                            }
                        }
                    }
                );
                break;
            }
            default: {
                break;
            }
        }
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject RemoveConnection(iCS_EditorObject inPort) {
        iCS_EditorObject sourcePort= IStorage.GetSourceEndPort(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= inPort.Source;
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= tmpPort.Destinations;
            if(connected.Length == 1) {
                iCS_EditorObject t= tmpPort.Source;
                toDestroy.Add(tmpPort);
                tmpPort= t;
            } else {
                break;
            }
        }
        foreach(var byebye in toDestroy) {
            IStorage.DestroyInstance(byebye.InstanceId);
        }
        return sourcePort;        
    }
    // ----------------------------------------------------------------------
    iCS_EditorObject[] FindAllConnectedInDataPorts(iCS_EditorObject outPort) {
        List<iCS_EditorObject> allInDataPorts= new List<iCS_EditorObject>();
        FillConnectedInDataPorts(outPort, allInDataPorts);
        return allInDataPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    void FillConnectedInDataPorts(iCS_EditorObject outPort, List<iCS_EditorObject> result) {
        if(outPort == null) return;
        iCS_EditorObject[] connectedPorts= outPort.Destinations;
        foreach(var port in connectedPorts) {
            if(port.IsDataOrControlPort) {
                if(port.IsKindOfPackagePort) {
                    FillConnectedInDataPorts(port, result);
                } else {
                    if(port.IsInputPort) {
                        result.Add(port);
                    }
                }
            }
        }
    }
	// ----------------------------------------------------------------------
    void PasteIntoGraph(Vector2 point, iCS_Storage sourceStorage, iCS_EngineObject sourceRoot) {
        if(sourceRoot == null) return;
        iCS_EditorObject validParent= GetValidParentNodeUnder(point, sourceRoot.ObjectType, sourceRoot.Name);
        if(validParent == null) {
			var node= IStorage.GetNodeAt(point);
			if(node == null && IStorage.IsEmptyBehaviour) {
				int option= EditorUtility.DisplayDialogComplex("Behaviour message required !", "Unity behaviour requires that nodes be added to a predefined message.  Use the buttons below to create the desired message type for your node.","Create Update", "More events...","Create OnGUI");
				switch(option) {
					case 0:
						validParent= AutoCreateBehaviourMessage(iCS_Strings.Update, point);
						break;
					case 1:
						ShowNotification(new GUIContent("Please use right mouse click on canvas to create behaviour event type before adding new object."));
						SelectedObject= IStorage.EditorObjects[0];
						myDynamicMenu.Update(SelectedObject, IStorage, point);
						return;
					case 2:
						validParent= AutoCreateBehaviourMessage(iCS_Strings.OnGUI, point);
						break;
				}
			} else {
	            EditorUtility.DisplayDialog("Operation Aborted", "Unable to find a suitable parent to paste into !!!", "Cancel");				
				return;
			}
        }
        iCS_IStorage srcIStorage= new iCS_IStorage(sourceStorage);
        iCS_EditorObject srcRoot= srcIStorage.EditorObjects[sourceRoot.InstanceId];
        iCS_EditorObject pasted= IStorage.Copy(srcRoot, srcIStorage, validParent, point, IStorage);
        if(pasted.IsUnfoldedOnDisplay) {
            IStorage.Fold(pasted);            
        }
        pasted.LayoutNode();
        pasted.LayoutParentNodesUntilTop(iCS_AnimationControl.Always);
    }
    iCS_EditorObject AutoCreateBehaviourMessage(string messageName, Vector2 point) {
		var validParent= IStorage.CreatePackage(0, point, messageName);
		validParent.Tooltip= iCS_AllowedChildren.TooltipForBehaviourChild(iCS_Strings.Update);
        IStorage.Unfold(validParent);
        return validParent;
    }

}
