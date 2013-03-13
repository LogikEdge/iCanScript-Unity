using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortConnection, PortRelocation, NodeDrag, NodeRelocation, TransitionCreation };


    // ======================================================================
    // Fields.
    // ----------------------------------------------------------------------
    DragTypeEnum     DragType                = DragTypeEnum.None;
    iCS_EditorObject DragObject              = null;
    iCS_EditorObject DragFixPort             = null;
    iCS_EditorObject DragOriginalPort        = null;
    Vector2          MouseDragStartPosition  = Vector2.zero;
    Vector2          DragStartDisplayPosition= Vector2.zero;
    Vector2          DragStartAnchorPosition = Vector2.zero;
    bool             IsDragEnabled           = false;
    bool             IsDragStarted         { get { return IsDragEnabled && DragObject != null; }}

    // ======================================================================
    // Functions.
	// ----------------------------------------------------------------------
    void MakeDataConnectionDrag() {
        if(DragFixPort != DragOriginalPort) {
            IStorage.SetSource(DragOriginalPort, DragFixPort);
        } else {
            IStorage.SetSource(DragOriginalPort, null);            
        }
        IStorage.SetSource(DragObject, DragOriginalPort);
        DragFixPort= DragOriginalPort;
    }
	// ----------------------------------------------------------------------
    void BreakDataConnectionDrag() {
        var originalSource= DragOriginalPort.Source;
        if(originalSource != null && originalSource != DragObject) {
            DragFixPort= originalSource;
            IStorage.SetSource(DragObject, DragFixPort);
            IStorage.SetSource(DragOriginalPort, null);
        } else {
            if(DragFixPort == DragOriginalPort) {
                IStorage.SetSource(DragObject, null);
                IStorage.SetSource(DragFixPort, DragObject);
            }
        }
    }
	// ----------------------------------------------------------------------
    bool StartDrag() {
        // Don't select new drag type if drag already started.
        if(IsDragStarted) return true;

        // Use the Left mouse down position has drag start position.
        MouseDragStartPosition= MouseDownPosition;
        Vector2 pos= ViewportToGraph(MouseDragStartPosition);

        // Port drag.
        iCS_EditorObject port= SelectedObject;
        if(port != null && port.IsPort && port.IsVisibleOnDisplay) {
            IStorage.RegisterUndo("Port Drag");
            IStorage.CleanupDeadPorts= false;		// Suspend object cleanup.
            DragType= DragTypeEnum.PortRelocation;
            DragOriginalPort= port;
            DragFixPort= port;
            DragObject= port;
            DragStartDisplayPosition= port.GlobalDisplayPosition;
            DragObject.IsSticky= true;
            return true;
        }

        // Node drag.
        iCS_EditorObject node= SelectedObject;                
        if(node != null && node.IsNode && (node.IsIconizedOnDisplay || !node.IsState || myGraphics.IsNodeTitleBarPicked(node, pos))) {
            if(IsCopyKeyDown) {
				// Transform into Unity drag & drop protocol. 
                GameObject go= new GameObject(node.Name);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent("iCS_Library");
                iCS_Library library= go.GetComponent<iCS_Library>();
                iCS_IStorage iStorage= new iCS_IStorage(library);
                iStorage.Copy(node, IStorage, null, Vector2.zero, iStorage);
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(node.Name);
                iCS_AutoReleasePool.AutoRelease(go, 60f);
                // Disable dragging.
                IsDragEnabled= false;
                DragType= DragTypeEnum.None;
            } else {
                if(IsFloatingKeyDown) {
                    IStorage.RegisterUndo("Node Relocation");
                    DragType= DragTypeEnum.NodeRelocation;                                        
                    DragStartAnchorPosition= node.GlobalAnchorPosition;
                    node.IsFloating= true;
                } else {
                    IStorage.RegisterUndo("Node Drag");
                    DragType= DragTypeEnum.NodeDrag;                    
                    node.IsFloating= false;
    				node.ForEachParentNode(p=> { p.IsSticky= true; });
                }
				node.IsSticky= true;
                DragObject= node;
                DragStartDisplayPosition= node.GlobalDisplayPosition;                                                                    
            }
            return true;
        }

        // New state transition drag.
        if(node != null && node.IsState) {
            IStorage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            iCS_EditorObject outTransition= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutStatePort);
            iCS_EditorObject inTransition= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
            outTransition.SetGlobalAnchorAndLayoutPosition(pos);
            inTransition.SetGlobalAnchorAndLayoutPosition(pos);
            inTransition.SourceId= outTransition.InstanceId;
            DragFixPort= outTransition;
            DragObject= inTransition;
            DragStartDisplayPosition= DragObject.GlobalDisplayPosition;
            DragObject.IsFloating= true;
            DragObject.IsSticky= true;
            return true;
        }

        // Disable dragging since mouse is not over Node or Port.
        DragType= DragTypeEnum.None;
        DragObject= null;
        IsDragEnabled= false;
        return false;
    }
	// ----------------------------------------------------------------------
    void ProcessDrag() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Start a new drag (if not already started).
        if(!StartDrag()) return;

        // Compute new object position.
        Vector2 delta= ViewportMousePosition - MouseDragStartPosition;
        var newPosition= DragStartDisplayPosition + delta;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
            case DragTypeEnum.NodeRelocation:
                DragObject.UserDragTo(newPosition);
                break;
            case DragTypeEnum.PortRelocation: {
				// Consider port relocation when dragging on parent edge.
                ProcessPortRelocation(newPosition);
                break;
            }
            case DragTypeEnum.PortConnection: {
                // Update port position.
                DragObject.GlobalDisplayPosition= newPosition;
                // Determine if we should go back to port relocation. (IsPositionOnEdge)
                if(!DragOriginalPort.IsChildMuxPort && DragOriginalPort.Parent.IsPositionOnEdge(newPosition, DragOriginalPort.Edge)) {
                    RemoveDragPort();
                    break;
                }
                // Snap to nearby ports
                Vector2 mousePosInGraph= GraphMousePosition;
                iCS_EditorObject closestPort= IStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Vector2 closestPortPos= closestPort.GlobalDisplayPosition;
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < iCS_EditorConfig.PortDiameter) {
                        DragObject.GlobalDisplayPosition= closestPortPos;
                    }                    
                }
                // Continously refresh drag port if module port.
                if(DragOriginalPort.IsModulePort) {
                    CreateDragPort();
                }
                break;
            }
            case DragTypeEnum.TransitionCreation:
                // Update port position.
                DragObject.SetGlobalAnchorAndLayoutPosition(newPosition);
                break;
        }
    }    
	// ----------------------------------------------------------------------
    void EndDrag() {
		ProcessDrag();
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    // Remove sticky on parent nodes.
                    DragObject.IsFloating= false;
                    DragObject.IsSticky= false;
					DragObject.ForEachParentNode(p=> { p.IsSticky= false; });
                    break;
                }
                case DragTypeEnum.NodeRelocation: {
                    DragObject.IsFloating= false;
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= node.Parent;
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(GraphMousePosition, node);
                        if(newParent != null) {
                            if(newParent != oldParent) {
                                ChangeParent(node, newParent);
                            } else {
                                oldParent.LayoutNode(iCS_AnimationControl.Always);
                                oldParent.LayoutParentNodesUntilTop(iCS_AnimationControl.Always);
                            }
                        } else {
                            node.IsFloating= true;
                            var pos= node.GlobalDisplayPosition;
                            node.GlobalAnchorPosition= DragStartAnchorPosition;
                            node.GlobalDisplayPosition= pos;
                            node.AnimatePosition(DragStartDisplayPosition);                            
                        }
                    }
                    // Remove sticky on parent nodes.
                    DragObject.IsSticky= false;
                    break;
                }
                case DragTypeEnum.PortRelocation:
                    DragObject.IsSticky= false;
                    DragObject.IsFloating= false;
                    if(DragObject.IsDataPort) {
						DragObject.ParentNode.LayoutPorts();
                        break;
                    }                    
                    if(DragObject.IsStatePort) {
                        // Get original port state & state chart.
                        iCS_EditorObject origState= DragObject.Parent;
                        iCS_EditorObject origStateChart= origState.Parent;
                        while(origStateChart != null && !origStateChart.IsStateChart) {
                            origStateChart= origStateChart.Parent;
                        }
                        // Get new drag port state & state chart.
                        iCS_EditorObject newState= IStorage.GetNodeWithEdgeAt(DragObject.GlobalDisplayPosition);
                        if(newState == null || !newState.IsState) {
                            newState= GetStateAt(GraphMousePosition);                            
                        }
                        iCS_EditorObject newStateChart= null;
                        if(newState != null) {
                            newStateChart= newState.Parent;
                            while(newStateChart != null && !newStateChart.IsStateChart) {
                                newStateChart= newStateChart.Parent;
                            }
                        }
                        // Relocate dragged port if on the same state.
                        if(origState == newState) {
                            DragObject.UpdatePortEdge();
							origState.LayoutPorts();
                            break;
                        }
                        // Delete transition if the dragged port is not on a valid state.
                        if(newState == null || origStateChart != newStateChart) {
                            if(EditorUtility.DisplayDialog("Deleting Transition", "Are you sure you want to remove the dragged transition.", "Delete", "Cancel")) {
                                IStorage.DestroyInstance(DragObject);
                            } else {
                                DragObject.SetGlobalAnchorAndLayoutPosition(DragStartDisplayPosition);
                            }
                            break;
                        }
                        // Relocate transition to the new state.
                        var dragObjectPosition= DragObject.GlobalDisplayPosition;
                        DragObject.Parent= newState;
                        DragObject.SetGlobalAnchorAndLayoutPosition(dragObjectPosition);
                        DragObject.UpdatePortEdge();
                        newState.LayoutPorts();
                        iCS_EditorObject transitionModule= IStorage.GetTransitionModule(DragObject);
                        iCS_EditorObject otherStatePort= DragObject.IsInputPort ? IStorage.GetFromStatePort(transitionModule) : IStorage.GetToStatePort(transitionModule);
                        iCS_EditorObject otherState= otherStatePort.Parent;
                        iCS_EditorObject moduleParent= transitionModule.Parent;
                        iCS_EditorObject newModuleParent= IStorage.GetTransitionParent(newState, otherState);
                        if(moduleParent != newModuleParent) {
                            transitionModule.Parent= newModuleParent;
                            IStorage.LayoutTransitionModule(transitionModule);
                        }
                        break;
                    }
                    break;
                case DragTypeEnum.PortConnection:                
                    // Verify for a new connection.
                    if(!VerifyNewDragConnection()) {
                        bool isNearParent= DragObject.IsPortOnParentEdge;
                        if(DragFixPort.IsDataPort) {
                            // We don't need the drag port anymore.
                            var dragPortPos= DragObject.GlobalDisplayPosition;
                            DragObject.IsSticky= false;
                            DragObject.IsFloating= false;
							IStorage.DestroyInstance(DragObject);
							DragObject= null;
                            // Verify for disconnection.
                            if(!isNearParent) {
                                // Let's determine if we want to create a module port.
                                iCS_EditorObject newPortParent= GetNodeWithEdgeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    iCS_EditorObject portParent= DragFixPort.Parent;
                                    if(DragFixPort.IsInputPort) {
                                        if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Left)) {
                                            if(IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                newPort.SetGlobalAnchorAndLayoutPosition(dragPortPos);
                                                newPort.PortValue= DragFixPort.PortValue;
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;
                                            }
                                        }
                                        if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Right)) {
                                            if(!IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                newPort.SetGlobalAnchorAndLayoutPosition(dragPortPos);
                                                newPort.PortValue= DragFixPort.PortValue;
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;                                                
                                            }
                                        }                                    
                                    }
                                    if(DragFixPort.IsOutputPort) {
                                        if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Right)) {
                                            if(IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                newPort.SetGlobalAnchorAndLayoutPosition(dragPortPos);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;                                                                                                    
                                            }
                                        }
                                        if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Left)) {
                                            if(!IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                newPort.SetGlobalAnchorAndLayoutPosition(dragPortPos);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;
                                            }
                                        }
                                    }
                                }
                                // Autocreate instance node if inside a composite module.
                                newPortParent= GetNodeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    // Determine if we need to create an instance node.
                                    AutocreateInstanceNode(dragPortPos, newPortParent);
                                    break;                                  
                                }
                                if(DragFixPort.IsOutputPort && newPortParent != null && (newPortParent.IsState || newPortParent.IsStateChart)) {
									if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Right)) {
	                                    iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
	                                    SetNewDataConnection(newPort, DragFixPort);
									}
                                    break;
                                }
                            }
                        }                    
                    }
                    break;
                case DragTypeEnum.TransitionCreation:
                    iCS_EditorObject destState= GetNodeAtMousePosition();
                    if(destState != null && destState.IsState) {
                        iCS_EditorObject outStatePort= IStorage[DragObject.SourceId];
                        outStatePort.IsFloating= false;
                        outStatePort.IsSticky= false;
                        IStorage.CreateTransition(outStatePort, destState, DragObject.GlobalDisplayPosition);
                        DragObject.SourceId= -1;
                        IStorage.DestroyInstance(DragObject);
                    } else {
                        IStorage.DestroyInstance(DragObject.SourceId);
                        IStorage.DestroyInstance(DragObject);
                    }
                    break;
            }            
        }

        finally {
            // Reset dragging state.
            ResetDrag();
        }
    }
	// ----------------------------------------------------------------------
    void ResetDrag() {
        DragType        = DragTypeEnum.None;
        DragObject      = null;
        DragOriginalPort= null;
        DragFixPort     = null;
        IsDragEnabled   = false;
        IStorage.CleanupDeadPorts= true;                    
    }

	// ----------------------------------------------------------------------
    void RemoveDragPort() {
        if(DragFixPort != DragOriginalPort) {
            IStorage.SetSource(DragOriginalPort, DragFixPort);
        }
		if(DragObject != DragOriginalPort) {
			IStorage.DestroyInstance(DragObject);
		}
        DragObject= DragOriginalPort;
        DragFixPort= DragOriginalPort;
        DragType= DragTypeEnum.PortRelocation;
        DragObject.IsFloating= false;
        SelectedObject= DragOriginalPort;
    }
	// ----------------------------------------------------------------------
	void CreateDragPort() {
        // Data port. Create a drag port as appropriate.
        iCS_EditorObject parent= DragOriginalPort.ParentNode;
		// The simple case is for non-module data ports.
		if(!DragOriginalPort.IsModulePort) {
			// Determine if we are already properly connected.
			if(DragObject != DragOriginalPort) return;
			if(DragOriginalPort.IsInputPort) {
				// Create the appropriate drag port.
				var sourcePort= DragOriginalPort.Source;
				if(sourcePort != null) {	// Disconnect if the port is connected.
					RemoveDragPort();
					var sourceParent= sourcePort.ParentNode;
		            var newPort= IStorage.CreatePort(sourcePort.Name,
													 sourceParent.InstanceId,
													 sourcePort.RuntimeType,
													 iCS_ObjectTypeEnum.InDynamicModulePort);
					DragFixPort= sourcePort;
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragFixPort);
					IStorage.SetSource(DragOriginalPort, null);
				} else {					// Input is not connected so simply connect the drag port
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 iCS_ObjectTypeEnum.OutDynamicModulePort);
					DragObject= newPort;
					DragFixPort= DragOriginalPort;
					IStorage.SetSource(DragOriginalPort, DragObject);
				}
			} else {	// Output port.
				RemoveDragPort();
	            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
												 parent.InstanceId,
												 DragOriginalPort.RuntimeType,
												 iCS_ObjectTypeEnum.InDynamicModulePort);
				DragObject= newPort;
				DragFixPort= DragOriginalPort;
				IStorage.SetSource(DragObject, DragFixPort);
			}
		}
		else {	// This is a module port.
			var point= GraphMousePosition;
			bool isInside= DragOriginalPort.ParentNode.GlobalDisplayRect.Contains(point);
			if(DragOriginalPort.IsInputPort) {	// Input module port
				if(isInside) {	// Inside parent node
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   DragFixPort == DragOriginalPort &&
					   DragObject.Source == DragFixPort) {
						return;
					}
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 iCS_ObjectTypeEnum.InDynamicModulePort);
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragOriginalPort);
					DragFixPort= DragOriginalPort;
				}
				else {		// Outside parent node
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   ((DragFixPort != DragOriginalPort && DragObject.Source == DragFixPort) ||
					    (DragFixPort == DragOriginalPort && DragOriginalPort.Source == DragObject))) {
						return;
					}
					RemoveDragPort();
					var sourcePort= DragOriginalPort.Source;
					if(sourcePort != null) {
						var sourceParent= sourcePort.ParentNode;
			            var newPort= IStorage.CreatePort(sourcePort.Name,
														 sourceParent.InstanceId,
														 sourcePort.RuntimeType,
														 iCS_ObjectTypeEnum.InDynamicModulePort);
						DragFixPort= sourcePort;
						DragObject= newPort;
						IStorage.SetSource(DragObject, DragFixPort);
						IStorage.SetSource(DragOriginalPort, null);
					}
					else {
			            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
														 parent.InstanceId,
														 DragOriginalPort.RuntimeType,
														 iCS_ObjectTypeEnum.OutDynamicModulePort);
						DragObject= newPort;
						IStorage.SetSource(DragOriginalPort, DragObject);
						DragFixPort= DragOriginalPort;
					}
				}
			}
			else {				// Output module port
				if(isInside) {	// Inside parent node
					// Nothing to do if already properly connected.
                    if(DragObject != DragOriginalPort &&
                       (DragFixPort != DragOriginalPort && DragObject.Source == DragFixPort) ||
                       (DragFixPort == DragOriginalPort && DragOriginalPort.Source == DragObject)) {
                        return;    
                    }
					RemoveDragPort();
					var sourcePort= DragOriginalPort.Source;
					if(sourcePort != null) {
						var sourceParent= sourcePort.ParentNode;
			            var newPort= IStorage.CreatePort(sourcePort.Name,
														 sourceParent.InstanceId,
														 sourcePort.RuntimeType,
														 iCS_ObjectTypeEnum.InDynamicModulePort);
						DragFixPort= sourcePort;
						DragObject= newPort;
						IStorage.SetSource(DragObject, DragFixPort);
                        IStorage.SetSource(DragOriginalPort, null);
					}
					else {
			            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
														 parent.InstanceId,
														 DragOriginalPort.RuntimeType,
														 iCS_ObjectTypeEnum.OutDynamicModulePort);
						DragObject= newPort;
						IStorage.SetSource(DragOriginalPort, DragObject);
						DragFixPort= DragOriginalPort;
					}
				}
				else {		// Outside parent node
					// FIXME: Must connect to MuxParent.
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   DragFixPort == DragOriginalPort &&
					   DragObject.Source == DragOriginalPort) {
						return;
					}
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.Name,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 iCS_ObjectTypeEnum.InDynamicModulePort);
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragOriginalPort);
					DragFixPort= DragOriginalPort;
				}
			}
		}
        DragType= DragTypeEnum.PortConnection;
        DragObject.GlobalAnchorPosition= GraphMousePosition;
		DragObject.GlobalDisplayPosition= GraphMousePosition;
        DragObject.IsFloating= true;
        DragObject.IsSticky= true;
        SelectedObject= DragObject;
	}

	// ----------------------------------------------------------------------
	// TODO: Move port relocation processing into editor object...
    void ProcessPortRelocation(Vector2 newPosition) {
		var parent= DragObject.ParentNode;
		var parentRect= parent.GlobalDisplayRect;
        // Determine if we should convert to data port connection drag.
		if(DragObject.IsDataPort && !iCS_EditorObject.IsPositionOnRectEdge(newPosition, parentRect, DragObject.Edge)) {
            parent.LayoutPorts();
            CreateDragPort();
            return;
		}                      
        // No reordering necessary if this is the only port on this edge.
        DragObject.SetGlobalAnchorAndLayoutPosition(newPosition);
        var sameEdgePorts= P.filter(p=> p != DragObject, DragObject.BuildListOfPortsOnSameEdge());
		var nbOfPortsOnEdge= sameEdgePorts.Length;
        if(nbOfPortsOnEdge == 0) {
			return;
        }
		// Build port related information (sortedPorts, portRatios, and portGlobalPositions).
		parent.LayoutPortsOnSameEdge(sameEdgePorts);
		var sortedPorts        = iCS_EditorObject.SortPortsOnLayout(sameEdgePorts);		
		var portRatios         = P.map(p=> p.PortPositionRatio, sortedPorts);
		var portPositionsOnEdge= P.map(p=> (p.IsOnHorizontalEdge ? p.GlobalDisplayPosition.x : p.GlobalDisplayPosition.y), sortedPorts);
		var newPositionOnEdge  = DragObject.IsOnHorizontalEdge ? newPosition.x : newPosition.y;
		var delta              = newPosition-DragObject.GlobalDisplayPosition;
		var deltaOnEdge        = DragObject.IsOnHorizontalEdge ? delta.x : delta.y;
		var parentGlobalPos    = parent.GlobalDisplayPosition;
		var edgePosStart       = DragObject.IsOnHorizontalEdge ?
									parentGlobalPos.x+parent.HorizontalPortsLeft :
									parentGlobalPos.y+parent.VerticalPortsTop;
		var edgePosEnd         = DragObject.IsOnHorizontalEdge ?
									parentGlobalPos.x+parent.HorizontalPortsRight :
									parentGlobalPos.y+parent.VerticalPortsBottom;
		// Determine index of drag object according to display position.
		int index;
		for(index= 0; index < nbOfPortsOnEdge; ++index) {
			var a= portPositionsOnEdge[index];
			if(Math3D.IsSmaller(newPositionOnEdge, a)) {
				break;
			}
			if(Math3D.IsEqual(newPositionOnEdge, a) && Math3D.IsSmaller(deltaOnEdge, 0f)) {
				break;
			}
		}
		// Determine proper anchor ratio for drag port.
		float rangeRatioStart, rangeRatioEnd;
		float rangePosStart,   rangePosEnd;
		if(index == 0) {
			rangeRatioStart= 0f;
			rangeRatioEnd  = portRatios[index];
			rangePosStart  = edgePosStart;
			rangePosEnd    = portPositionsOnEdge[index];
		} else if(index == nbOfPortsOnEdge) {
			rangeRatioStart= portRatios[index-1];
			rangeRatioEnd  = 1f;
			rangePosStart  = portPositionsOnEdge[index-1];
			rangePosEnd    = edgePosEnd;			
		} else {
			rangeRatioStart= portRatios[index-1];
			rangeRatioEnd  = portRatios[index];
			rangePosStart  = portPositionsOnEdge[index-1];
			rangePosEnd    = portPositionsOnEdge[index];			
		}
		var rangePos   = rangePosEnd-rangePosStart;
        float ratio;
		if(Math3D.IsZero(rangePos)) {
            ratio= rangePosStart;
		} else {
    		var rangeOffset= newPositionOnEdge-rangePosStart;
    		ratio= Math3D.Lerp(rangeRatioStart, rangeRatioEnd, rangeOffset/rangePos);		    
		}
		// Cleanup ratio to avoid ordering issues caused by equal ratios.
        const float kMinRatioDiff= 0.001f;
        // ... first we arrange any overlap with the drag port.
		if(index == 0) {
		    var ratioDiff= portRatios[0]-ratio;
		    if(ratioDiff < kMinRatioDiff) {
		        ratio= portRatios[0]-kMinRatioDiff;
		    }
		} else if(index == nbOfPortsOnEdge) {
		    var ratioDiff= ratio-portRatios[nbOfPortsOnEdge-1];
		    if(ratioDiff < kMinRatioDiff) {
		        ratio= portRatios[nbOfPortsOnEdge-1]+kMinRatioDiff;
		    }
		} else {
		    var ratioDiff= ratio-portRatios[index-1];
		    if(ratioDiff < kMinRatioDiff) {
		        ratio= portRatios[index-1]+kMinRatioDiff;
		    }
		    ratioDiff= portRatios[index]-ratio;
		    if(ratioDiff < kMinRatioDiff) {
		        ratio= portRatios[index]-kMinRatioDiff;
		    }
		}
		DragObject.PortPositionRatio= ratio;
		// ... rebuild port lists to include the drag port.
		sortedPorts        = P.insertAt(DragObject,        index, sortedPorts);
		portRatios         = P.insertAt(ratio,             index, portRatios);
		portPositionsOnEdge= P.insertAt(newPositionOnEdge, index, portPositionsOnEdge);
        nbOfPortsOnEdge    = sortedPorts.Length;
        // ... then we offset any remaining overlaps or out of bounds.
        for(int i= 0; i < nbOfPortsOnEdge-1; ++i) {
            var r= sortedPorts[i].PortPositionRatio;
            if(Math3D.IsSmallerOrEqual(r, 0f)) {
                r= 0f;
                sortedPorts[i].PortPositionRatio= r;
            }
            var nextRatio= sortedPorts[i+1].PortPositionRatio;
            if(r+kMinRatioDiff > nextRatio) {
                sortedPorts[i+1].PortPositionRatio= r+kMinRatioDiff;
            }
        }
        for(int i= nbOfPortsOnEdge-1; i > 0; --i) {
            var r= sortedPorts[i].PortPositionRatio;
            if(Math3D.IsGreaterOrEqual(r, 1f)) {
                r= 1f;
                sortedPorts[i].PortPositionRatio= r;
            }
            var prevRatio= sortedPorts[i-1].PortPositionRatio;
            if(prevRatio > r-kMinRatioDiff) {
                sortedPorts[i-1].PortPositionRatio= r-kMinRatioDiff;
            }                
        }
        // Rearrange layout position to give priority to the drag port.
        newPosition= CleanupPortGlobalPositionOnEdge(newPosition, parent, parentGlobalPos, parentRect);
		portPositionsOnEdge[index]= DragObject.IsOnHorizontalEdge ? newPosition.x : newPosition.y;
        float minSeparation= iCS_EditorConfig.MinimumPortSeparation;
        for(int i= index; i < nbOfPortsOnEdge-1; ++i) {
            var posDiff= portPositionsOnEdge[i+1]-portPositionsOnEdge[i];
            if(posDiff < minSeparation) {
                var offset= minSeparation-posDiff;
                var pos= sortedPorts[i+1].GlobalDisplayPosition;
                if(DragObject.IsOnHorizontalEdge) {
                    pos.x+= offset;
                } else {
                    pos.y+= offset;
                }
                sortedPorts[i+1].GlobalDisplayPosition= pos;
                portPositionsOnEdge[i+1]+= offset;
            }
        }
        for(int i= index; i > 0; --i) {
            var posDiff= portPositionsOnEdge[i]-portPositionsOnEdge[i-1];
            if(posDiff < minSeparation) {
                var offset= minSeparation-posDiff;
                var pos= sortedPorts[i-1].GlobalDisplayPosition;
                if(DragObject.IsOnHorizontalEdge) {
                    pos.x-= offset;
                } else {
                    pos.y-= offset;
                }
                sortedPorts[i-1].GlobalDisplayPosition= pos;
                portPositionsOnEdge[i-1]-= offset;
            }
        }
        // Finaly, set the drag port layout position.
        DragObject.GlobalDisplayPosition= newPosition;
    }
	// ----------------------------------------------------------------------
    Vector2 CleanupPortGlobalPositionOnEdge(Vector2 pos, iCS_EditorObject parent, Vector2 parentGloablPos, Rect parentRect) {
		float x,y;
		if(DragObject.IsOnHorizontalEdge) {
			x= pos.x;
			if(x < parentGloablPos.x+parent.HorizontalPortsLeft)  x= parentGloablPos.x+parent.HorizontalPortsLeft;
			if(x > parentGloablPos.x+parent.HorizontalPortsRight) x= parentGloablPos.x+parent.HorizontalPortsRight;
			y= DragObject.IsOnTopEdge  ? parentRect.yMin : parentRect.yMax;
			
		} else {
			x= DragObject.IsOnLeftEdge ? parentRect.xMin : parentRect.xMax;
			y= pos.y;
			if(y < parentGloablPos.y+parent.VerticalPortsTop)    y= parentGloablPos.y+parent.VerticalPortsTop;
			if(y > parentGloablPos.y+parent.VerticalPortsBottom) y= parentGloablPos.y+parent.VerticalPortsBottom;
		}
		return new Vector2(x,y);        
    }
	// ----------------------------------------------------------------------
    void AutocreateInstanceNode(Vector2 globalPosition, iCS_EditorObject newParent) {
        var sourcePort= DragFixPort;
        Type instanceType= sourcePort.RuntimeType;
        if(iCS_Types.IsStaticClass(instanceType)) return;
        if(DragOriginalPort != DragFixPort) return;
        var instance= IStorage.CreateModule(newParent.InstanceId, globalPosition, "", iCS_ObjectTypeEnum.Module, instanceType);
        var thisPort= IStorage.ClassModuleGetInputThisPort(instance);
        SetNewDataConnection(thisPort, sourcePort);
    }
}
