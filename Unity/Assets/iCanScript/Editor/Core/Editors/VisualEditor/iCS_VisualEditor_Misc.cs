//#define DEBUG
#define NEW_RECONNECTION
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

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
		/*
			TODO : Process multi-select key (Command/Windows)
		*/
		if(IsMultiSelectKeyDown) {
			iCS_UserCommands.ToggleMultiSelection(newSelected);
		}
		else {
	        iCS_UserCommands.Select(newSelected, IStorage);
		}
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
	void ToggleMultiSelection(iCS_EditorObject obj) {
		IStorage.ToggleMultiSelection(obj);
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
        if(overlappingPort.IsRelayPort && overlappingPort.IsDynamicDataPort) {
            CreateMuxPort(fixPort, overlappingPort);
            return true;
        }
        iCS_EditorObject overlappingPortParent= overlappingPort.Parent;
        if(overlappingPort.IsOutputPort && 
		   overlappingPort.ProviderPort != null &&
		   overlappingPort.IsDynamicDataPort) {
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
                IStorage.SetAndAutoLayoutNewDataConnection(inPort, outPort, conversion);                
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
		var source= parentMuxPort.ProviderPort;
		// Convert source port to child port.
		if(source != null) {
			var firstChildMux= IStorage.CreatePort(fixPort.Name, parentMuxPort.InstanceId, parentMuxPort.RuntimeType, childPortType);
			IStorage.SetSource(firstChildMux, source);
			IStorage.SetSource(parentMuxPort, null);
			parentMuxPort.ObjectType= parentMuxPort.IsInputPort ? iCS_ObjectTypeEnum.InParentMuxPort : iCS_ObjectTypeEnum.OutParentMuxPort;
		}
		// Create new mux input port.
		var childMuxPort= IStorage.CreatePort(fixPort.Name, parentMuxPort.InstanceId, parentMuxPort.RuntimeType, childPortType);
		IStorage.SetNewDataConnection(childMuxPort, fixPort, conversion);
		IStorage.CleanupMuxPort(parentMuxPort);
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
#if NEW_RECONNECTION
	// ----------------------------------------------------------------------
	void RebuildDataConnection(iCS_EditorObject outputPort, iCS_EditorObject inputPort) {
#if DEBUG
		Debug.Log("iCanScript: RebuildDataConnection: output= "+outputPort.Name+" input= "+inputPort.Name);
#endif
		// Have we completed rebuilding ... if so return.
		if(inputPort == outputPort) return;
		var inputNode= inputPort.ParentNode;
		var outputNode= outputPort.ParentNode;
		if(inputNode == outputNode) return;
		// outputPort is inside the node with the inputPort.
		var commonParentNode= outputPort.GetCommonParent(inputPort);
		if(inputNode == commonParentNode) {
			// Rebuild moving down from the common parent towards the output port.
			var newInputNode= outputPort.ParentNode;
			while(newInputNode != inputNode && newInputNode.ParentNode != inputNode) {
				newInputNode= newInputNode.ParentNode;
			}
			var existingPort= IStorage.FindPortWithSourceEndPoint(newInputNode, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.ProviderPort;
				if(prevSource != existingPort) {
					inputPort.ProviderPort= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						IStorage.CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(inputPort.Name, newInputNode.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				IStorage.SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.ProviderPort= inputPort.ProviderPort;
				inputPort.ProviderPort= newPort;
				RebuildDataConnection(outputPort, newPort);				
			}			
			return;
		}
		var inputNodeParent= inputNode.ParentNode;
		if(inputNodeParent == commonParentNode) {
			// Rebuild traversing from moving upwards to downwords.
			var newDstNode= outputPort.ParentNode;
			while(newDstNode != commonParentNode && newDstNode.ParentNode != commonParentNode) {
				newDstNode= newDstNode.ParentNode;
			}
			var existingPort= IStorage.FindPortWithSourceEndPoint(newDstNode, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.ProviderPort;
				if(prevSource != existingPort) {
					inputPort.ProviderPort= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						IStorage.CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(inputPort.Name, newDstNode.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicDataPort);
				IStorage.SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.ProviderPort= inputPort.ProviderPort;
				inputPort.ProviderPort= newPort;
				RebuildDataConnection(outputPort, newPort);				
			}
			return;
		} else {
			// Rebuilding moving up from the consumer port towards the common parent.
			var existingPort= IStorage.FindPortWithSourceEndPoint(inputNodeParent, outputPort);
			if(existingPort != null) {
				var prevSource= inputPort.ProviderPort;
				if(prevSource != existingPort) {
					inputPort.ProviderPort= existingPort;
					if(prevSource.IsDynamicDataPort && !inputPort.IsPartOfConnection(prevSource)) {
						IStorage.CleanupHangingConnection(prevSource);
					}					
				}
				RebuildDataConnection(outputPort, existingPort);
			} else {
	            iCS_EditorObject newPort= IStorage.CreatePort(inputPort.Name, inputNodeParent.InstanceId, inputPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicDataPort);
				IStorage.SetBestPositionForAutocreatedPort(newPort, outputPort.LayoutPosition, inputPort.LayoutPosition);
				newPort.ProviderPort= inputPort.ProviderPort;
				inputPort.ProviderPort= newPort;
				RebuildDataConnection(outputPort, newPort);
			}			
		}
	}
#endif
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
                                transitionPackage= p.ProviderPort.Parent;
                            } else {
                                iCS_EditorObject[] connectedPorts= p.ConsumerPorts;
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
                                IStorage.ChangeParent(transitionPackage, newParent);
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
                                IStorage.SetNewDataConnection(port, sourcePort);
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
                                    IStorage.SetNewDataConnection(inPort, port);                                    
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
        iCS_EditorObject sourcePort= IStorage.GetFirstProviderPort(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= inPort.ProviderPort;
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= tmpPort.ConsumerPorts;
            if(connected.Length == 1) {
                iCS_EditorObject t= tmpPort.ProviderPort;
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
        iCS_EditorObject[] connectedPorts= outPort.ConsumerPorts;
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
    void PasteIntoGraph(Vector2 point, iCS_MonoBehaviourImp sourceMonoBehaviour, iCS_EngineObject sourceRoot) {
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
						myContextualMenu.Update(iCS_ContextualMenu.MenuType.SelectedObject, SelectedObject, IStorage, point);
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
        iCS_UserCommands.PasteIntoGraph(sourceMonoBehaviour, sourceRoot, IStorage, validParent, point);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject AutoCreateBehaviourMessage(string messageName, Vector2 globalPos) {
        var updateDesc= P.filter(d => d.DisplayName == iCS_Strings.Update, iCS_LibraryDatabase.GetMessages(typeof(MonoBehaviour)));
        if(updateDesc == null || updateDesc.Length == 0) return null;
        return iCS_UserCommands.CreateMessageHandler(IStorage[0], globalPos, updateDesc[0]);
    }

}
