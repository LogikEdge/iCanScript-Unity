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
		if(SelectedObject != null && newSelected != null && newSelected.IsOutMuxPort && IStorage.GetOutMuxPort(SelectedObject) == newSelected) {
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
		if(SelectedObject == null || !SelectedObject.IsDataPort) return;
		if(SelectedObject.IsOutMuxPort) {
			IStorage.UntilMatchingChild(SelectedObject, 
				c=> {
					if(c.IsDataPort) {
						SelectedObject= c;
						return true;
					}
					return false;
				}
			);
			return;
		}
		iCS_EditorObject muxPort= SelectedObject.Parent;
		if(!muxPort.IsDataPort) return;
		bool takeNext= false;
		bool found= IStorage.UntilMatchingChild(muxPort,
			c=> {
				if(takeNext) {
					SelectedObject= c;
					return true;
				}
				if(c == SelectedObject) takeNext= true;
				return false;
			}
		);
		if(!found) SelectedObject= muxPort;
	}
	
	// ----------------------------------------------------------------------
    bool VerifyNewDragConnection() {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= IStorage.GetOverlappingPort(DragObject);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!DragFixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        // Destroy drag port since it is not needed anymore.
        IStorage.DestroyInstance(DragObject);
        DragObject= null;
        return VerifyNewConnection(DragFixPort, overlappingPort);
    }
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject overlappingPort) {
        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        iCS_EditorObject portParent= fixPort.Parent;
        iCS_EditorObject overlappingPortParent= overlappingPort.Parent;
        if(overlappingPort.IsOutputPort && (overlappingPortParent.IsState || overlappingPortParent.IsStateChart)) {
			CreateStateMux(fixPort, overlappingPort);
			return true;
		}
        
        // Connect function & modules ports together.
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
                MyWindow.ShowNotification(new GUIContent("Cannot connect nested node ports from input to output !!!"));
                return true;
            }
        } else {
            inPort = fixPort.IsInputPort          ? fixPort : overlappingPort;
            outPort= overlappingPort.IsOutputPort ? overlappingPort : fixPort;
        }
        if(inPort != outPort) {
            iCS_ReflectionInfo conversion= null;
            if(VerifyConnectionTypes(inPort, outPort, out conversion)) {
                SetNewDataConnection(inPort, outPort, conversion);                
            }
        } else {
            string direction= inPort.IsInputPort ? "input" : "output";
            MyWindow.ShowNotification(new GUIContent("Cannot connect an "+direction+" port to an "+direction+" port !!!"));
        }
        return true;
    }
	// ----------------------------------------------------------------------
    bool VerifyConnectionTypes(iCS_EditorObject inPort, iCS_EditorObject outPort, out iCS_ReflectionInfo typeCast) {
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
        typeCast= iCS_DataBase.FindTypeCast(outType, inType);
        if(typeCast == null) {
			MyWindow.ShowNotification(new GUIContent("No automatic type conversion exists from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)));
            return false;
        }
        return true;
    }
	// ----------------------------------------------------------------------
	void CreateStateMux(iCS_EditorObject fixPort, iCS_EditorObject stateMuxPort) {
        iCS_ReflectionInfo conversion= null;
        if(!VerifyConnectionTypes(stateMuxPort, fixPort, out conversion)) return;
		var source= stateMuxPort.Source;
		// Simply connect a disconnected mux state port.
		if(source == null && IStorage.NbOfChildren(stateMuxPort, c=> c.IsDataPort) == 0) {
			SetNewDataConnection(stateMuxPort, fixPort, conversion);
			return;
		}
		// Convert source port to child port.
		if(source != null) {
			var firstMuxInput= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
			IStorage.SetSource(firstMuxInput, source);
			IStorage.SetSource(stateMuxPort, null);
			stateMuxPort.ObjectType= iCS_ObjectTypeEnum.OutMuxPort;
		}
		// Create new mux input port.
		var inMuxPort= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
		SetNewDataConnection(inMuxPort, fixPort, conversion);
	}
	// ----------------------------------------------------------------------
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionInfo conversion= null) {
		iCS_EditorObject inNode= inPort.Parent;
        iCS_EditorObject outNode= outPort.Parent;
        iCS_EditorObject inParent= GetParentNode(inNode);
        
        iCS_EditorObject outParent= GetParentNode(outNode);
        // No need to create module ports if both connected nodes are under the same parent.
        if(inParent == outParent || inParent == outNode || inNode == outParent) {
            IStorage.SetSource(inPort, outPort, conversion);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= GetParentNode(outParent); op != null; op= GetParentNode(op)) {
            if(inParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, inParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            IStorage.SetSource(inPort, newPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.GlobalDisplayPosition, outPort.GlobalDisplayPosition);
            SetNewDataConnection(newPort, outPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= GetParentNode(inParent); ip != null; ip= GetParentNode(ip)) {
            if(outParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outParent != null) {
            iCS_EditorObject newPort= IStorage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            IStorage.SetSource(newPort, outPort, conversion);
			SetBestPositionForAutocreatedPort(newPort, inPort.GlobalDisplayPosition, outPort.GlobalDisplayPosition);
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
		var parentGlobalRect= parent.GlobalDisplayRect;
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
			port.SetGlobalAnchorAndLayoutPosition(new Vector2(x,y));
			return;
		}
		if(Math3D.IsEqual(inPortPosition.y, outPortPosition.y)) {
			port.SetGlobalAnchorAndLayoutPosition(new Vector2(x, 0.5f*(top+bottom)));
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
		port.SetGlobalAnchorAndLayoutPosition(new Vector2(x,y));
		return;			
	}

	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= edObj.Parent;
        for(; parentModule != null && !parentModule.IsModule; parentModule= parentModule.Parent);
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentNode(iCS_EditorObject edObj) {
        iCS_EditorObject parentNode= edObj.Parent;
        for(; parentNode != null && !parentNode.IsNode; parentNode= parentNode.Parent);
        return parentNode;
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
		var nodePos= node.GlobalDisplayPosition;
		node.Parent= newParent;
		node.SetGlobalAnchorAndLayoutPosition(nodePos);
		node.LayoutNode();
		node.LayoutParentNodesUntilTop(iCS_AnimationControl.Always);
		if(node.IsState) CleanupEntryState(node, oldParent);
        CleanupConnections(node);
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
                            iCS_EditorObject transitionModule= null;
                            if(p.IsInStatePort) {
                                transitionModule= p.Source.Parent;
                            } else {
                                iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= cp.Parent;
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= IStorage.GetFromStatePort(transitionModule).Parent;
                            iCS_EditorObject inState= IStorage.GetToStatePort(transitionModule).Parent;
                            iCS_EditorObject newParent= IStorage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != transitionModule.Parent) {
                                ChangeParent(transitionModule, newParent);
                                IStorage.LayoutTransitionModule(transitionModule);
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
            case iCS_ObjectTypeEnum.TransitionModule:
            case iCS_ObjectTypeEnum.TransitionGuard:
            case iCS_ObjectTypeEnum.TransitionAction:
            case iCS_ObjectTypeEnum.Module: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                IStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                goto case iCS_ObjectTypeEnum.InstanceMethod;
            }
            case iCS_ObjectTypeEnum.InstanceMethod:
            case iCS_ObjectTypeEnum.StaticMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.StaticField:
            case iCS_ObjectTypeEnum.TypeCast: {
                IStorage.ForEachChildPort(node,
                    port=> {
                        if(port.IsInDataPort) {
                            iCS_EditorObject sourcePort= RemoveConnection(port);
                            // Rebuild new connection.
                            if(sourcePort != port) {
                                SetNewDataConnection(port, sourcePort);
                            }
                        }
                        if(port.IsOutDataPort) {
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
        iCS_EditorObject sourcePort= IStorage.GetDataConnectionSource(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= inPort.Source;
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= IStorage.FindConnectedPorts(tmpPort);
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
        iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(outPort);
        foreach(var port in connectedPorts) {
            if(port.IsDataPort) {
                if(port.IsModulePort) {
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
				int option= EditorUtility.DisplayDialogComplex("Behaviour event required !", "Unity behaviour requires that nodes be added to a predefined event.  Use the buttons below to create the desired event type for your node.","Create Update", "More events...","Create OnGUI");
				switch(option) {
					case 0:
						validParent= AutoCreateBehaviourEvent(iCS_Strings.Update, point);
						break;
					case 1:
						MyWindow.ShowNotification(new GUIContent("Please use right mouse click on canvas to create behaviour event type before adding new object."));
						SelectedObject= IStorage.EditorObjects[0];
						myDynamicMenu.Update(SelectedObject, IStorage, point);
						return;
					case 2:
						validParent= AutoCreateBehaviourEvent(iCS_Strings.OnGUI, point);
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
    iCS_EditorObject AutoCreateBehaviourEvent(string eventName, Vector2 point) {
		var validParent= IStorage.CreateModule(0, point, iCS_Strings.Update);
		validParent.Tooltip= iCS_AllowedChildren.TooltipForBehaviourChild(iCS_Strings.Update);
        IStorage.Unfold(validParent);
        return validParent;
    }

}
