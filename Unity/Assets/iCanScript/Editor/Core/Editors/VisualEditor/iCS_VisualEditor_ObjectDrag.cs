using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using P=Prelude;

public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortConnection, PortRelocation, NodeDrag, TransitionCreation };


    // ======================================================================
    // Fields.
    // ----------------------------------------------------------------------
    DragTypeEnum     DragType              = DragTypeEnum.None;
    iCS_EditorObject DragObject            = null;
    iCS_EditorObject DragFixPort           = null;
    iCS_EditorObject DragOriginalPort      = null;
    Vector2          MouseDragStartPosition= Vector2.zero;
    Vector2          DragStartPosition     = Vector2.zero;
    bool             IsDragEnabled         = false;
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
            DragStartPosition= port.GlobalDisplayPosition;
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
                IStorage.RegisterUndo("Node Drag");
                node.IsFloating= IsFloatingKeyDown;
                DragType= DragTypeEnum.NodeDrag;
                DragObject= node;
                DragStartPosition= node.GlobalDisplayPosition;                                                                    
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
            DragStartPosition= DragObject.GlobalDisplayPosition;
            DragObject.IsFloating= true;
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
        var newPosition= DragStartPosition + delta;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                iCS_EditorObject node= DragObject;
                node.IsFloating= IsFloatingKeyDown;
                node.UserDragTo(newPosition);
                break;
            case DragTypeEnum.PortRelocation: {
				// We can't relocate a mux port child.
				if(DragObject.IsInMuxPort) {
					CreateDragPort();
					return;
				}
				// Consider port relocation when dragging on parent edge.
                ProcessPortRelocation(newPosition);
                break;
            }
            case DragTypeEnum.PortConnection: {
                // Update port position.
                DragObject.GlobalLayoutPosition= newPosition;
                // Determine if we should go back to port relocation. (IsPositionOnEdge)
                if(!DragOriginalPort.IsInMuxPort && DragOriginalPort.Parent.IsPositionOnEdge(newPosition, DragOriginalPort.Edge)) {
                    // Re-establish original connection if we are aborting a port disconnect drag.
                    iCS_EditorObject dragObjectSource= DragObject.Source;
                    if(dragObjectSource != DragOriginalPort) {
                        IStorage.SetSource(DragOriginalPort, dragObjectSource);
                    }
                    // Delete dynamically created floating drag port.
                    IStorage.DestroyInstance(DragObject);
                    DragObject= DragOriginalPort;
                    DragFixPort= DragOriginalPort;
                    DragObject.IsFloating= false;
                    DragType= DragTypeEnum.PortRelocation;
                    break;
                }
                // Snap to nearby ports
                Vector2 mousePosInGraph= GraphMousePosition;
                iCS_EditorObject closestPort= IStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Vector2 closestPortPos= closestPort.GlobalDisplayPosition;
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < iCS_EditorConfig.PortDiameter) {
                        DragObject.GlobalLayoutPosition= closestPortPos;
                    }                    
                }
                // Special case for module ports.
                if(DragOriginalPort.IsModulePort) {
                    if(IStorage.IsInside(DragOriginalPort.ParentNode, mousePosInGraph)) {
                        if(DragOriginalPort.IsOutputPort) {
                            BreakDataConnectionDrag();
                        } else {
                            MakeDataConnectionDrag();
                        }
                    } else {
                        if(DragOriginalPort.IsInputPort) {
                            BreakDataConnectionDrag();
                        } else {
                            MakeDataConnectionDrag();
                        }
                    }
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
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= node.Parent;
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(GraphMousePosition, node);
                        if(newParent != null) {
                            if(newParent != oldParent && node.IsFloating) {
                                ChangeParent(node, newParent);
                            }
                        } else {
                            node.UserDragTo(DragStartPosition);
                        }
                    }
                    node.IsFloating= false;
                    break;
                }
                case DragTypeEnum.PortRelocation:
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
                        iCS_EditorObject newState= IStorage.GetNodeWithEdgeAt(DragObject.GlobalLayoutPosition);
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
                                DragObject.SetGlobalAnchorAndLayoutPosition(DragStartPosition);
                            }
                            break;
                        }
                        // Relocate transition to the new state.
                        var dragObjectPosition= DragObject.GlobalLayoutPosition;
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
                            var dragPortPos= DragObject.GlobalLayoutPosition;
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
                        IStorage.CreateTransition(outStatePort, destState, DragObject.GlobalLayoutPosition);
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
	void CreateDragPort() {
        // Data port. Create a drag port as appropriate.
        iCS_EditorObject parent= DragOriginalPort.Parent;
        DragObject.IsFloating= false;
        if(DragOriginalPort.IsInputPort) {
            DragObject= IStorage.CreatePort(DragOriginalPort.Name, parent.InstanceId, DragOriginalPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            iCS_EditorObject prevSource= DragOriginalPort.Source;
            if(prevSource != null) {
                if(prevSource == DragObject) {
                    Debug.LogWarning("We are creating a drag port when a drag port already exists !!!");
                    return;
                }
                DragFixPort= prevSource;
                IStorage.SetSource(DragObject, DragFixPort);
                IStorage.SetSource(DragOriginalPort, null);
                DragObject.Name= DragFixPort.Name;
            } else {
                DragFixPort= DragOriginalPort;
                IStorage.SetSource(DragFixPort, DragObject);
            }                    
        } else {
            DragObject= IStorage.CreatePort(DragOriginalPort.Name, parent.InstanceId, DragOriginalPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            DragFixPort= DragOriginalPort;
            IStorage.SetSource(DragObject, DragOriginalPort);
        }
        DragType= DragTypeEnum.PortConnection;
        DragObject.LocalAnchorPosition= DragOriginalPort.LocalAnchorPosition;
        DragObject.GlobalLayoutPosition= GraphMousePosition;
		// Reset initial position if port is being dettached from it original parent.
		if(DragOriginalPort.IsInMuxPort) {
			DragStartPosition= DragOriginalPort.GlobalLayoutPosition - parent.GlobalLayoutPosition;			
		}
        DragObject.IsFloating= true;		
	}

	// ----------------------------------------------------------------------
    void ProcessPortRelocation(Vector2 newPosition) {
		// Consider port relocation when dragging on parent edge.
		var parent= DragObject.ParentNode;
		var parentRect= parent.GlobalDisplayRect;
		if(!iCS_EditorObject.IsPositionOnRectEdge(newPosition, parentRect, DragObject.Edge)) {
            // Determine if we should convert to data port connection drag.
            if(!(DragObject.IsStatePort || DragObject.IsTransitionPort)) {
            	parent.LayoutPorts();
            	CreateDragPort();
            }
            return;
		}                      
        // No reordering necessary if this is the only port on this edge.
        var sameEdgePorts= P.filter(p=> p != DragObject, DragObject.BuildListOfPortsOnSameEdge());
		var nbOfPortsOnEdge= sameEdgePorts.Length;
        if(nbOfPortsOnEdge == 0) {
			DragObject.SetGlobalAnchorAndLayoutPosition(newPosition);
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
		Debug.Log("DragObject index: "+index);
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
		if(Math3D.IsZero(rangePos)) {
			DragObject.SetGlobalAnchorAndLayoutPosition(newPosition);
			return;
		}
		var rangeOffset= newPositionOnEdge-rangePosStart;
		var ratio= Math3D.Lerp(rangeRatioStart, rangeRatioEnd, rangeOffset/rangePos);
//		const float kMinRatioDiff= 0.001f;
//		if(Math3D.IsEqual(ratio, rangeRatioStart)) ratio+= kMinRatioDiff;
//		if(Math3D.IsEqual(ratio, rangeRatioEnd)) ratio-= kMinRatioDiff;
		DragObject.PortPositionRatio= ratio;
		// Cleanup ratio to avoid ordering issues caused by euqal ratios.
		
		Debug.Log("Drag port ratio= "+ratio);

		DragObject.SetGlobalAnchorAndLayoutPosition(newPosition);

//		DragObject.GlobalLayoutPosition= newPosition;
//        // Determine before & after adjacent ports.
//        iCS_EditorObject beforePort= null;
//        iCS_EditorObject samePositionPort= null;
//        iCS_EditorObject afterPort= null;
//        float            beforePortAbsDiff= 0f;
//        float            afterPortAbsDiff = 0f;
//        for(int i= 0; i < sameEdgePorts.Length; ++i) {
//            var iter= sameEdgePorts[i];
//            if(iter == DragObject) continue;
//            var displayPos= iter.GlobalDisplayPosition;
//            if(DragObject.IsOnHorizontalEdge) {
//                var diff= displayPos.x-newPosition.x;
//                var absDiff= Mathf.Abs(diff);
//                if(Math3D.IsZero(diff)) {
//                    samePositionPort= iter;
//                    break;
//                }
//                if(Math3D.IsSmaller(diff, 0f)) {
//                    if(beforePort == null) {
//                        beforePort= iter;                                                        
//                        beforePortAbsDiff= absDiff;
//                    } else {
//                        if(Math3D.IsSmaller(absDiff, beforePortAbsDiff)) {
//                            beforePort= iter;
//                            beforePortAbsDiff= absDiff;
//                        }
//                    }
//                } else if(Math3D.IsGreater(diff, 0f)) {
//                    if(afterPort == null) {
//                        afterPort= iter;
//                        afterPortAbsDiff= absDiff;
//                    } else {
//                        if(Math3D.IsSmaller(absDiff, afterPortAbsDiff)) {
//                            afterPort= iter;
//                            afterPortAbsDiff= absDiff;                                
//                        }
//                    }
//                }
//            } else {
//                var diff= displayPos.y-newPosition.y;
//                var absDiff= Mathf.Abs(diff);
//                if(Math3D.IsZero(diff)) {
//                    samePositionPort= iter;
//                    break;
//                }
//                if(Math3D.IsSmaller(diff, 0f)) {
//                    if(beforePort == null) {
//                        beforePort= iter;
//                        beforePortAbsDiff= absDiff;
//                    } else {
//                        if(Math3D.IsSmaller(absDiff, beforePortAbsDiff)) {
//                            beforePort= iter;
//                            beforePortAbsDiff= absDiff;                                
//                        }
//                    }
//                } else if(Math3D.IsGreater(diff, 0f)) {
//                    if(afterPort == null) {
//                        afterPort= iter;
//                        afterPortAbsDiff= absDiff;
//                    } else {
//                        if(Math3D.IsSmaller(absDiff, afterPortAbsDiff)) {
//                            afterPort= iter;
//                            afterPortAbsDiff= absDiff;                                
//                        }
//                    }
//                }
//            } 
//        }
//        // Update local anchor port ratio to reflect the relocation.
//		// Just position drag port on the other side of the port with exactly the same position.
//		if(samePositionPort != null) {
//			// Give precedence to drag port.
//			var dragObjGlobalPos= DragObject.GlobalDisplayPosition;
//			float direction= Mathf.Sign(
//				DragObject.IsOnHorizontalEdge ? newPosition.x-dragObjGlobalPos.x : newPosition.y-dragObjGlobalPos.y
//			);
//			DragObject.PortPositionRatio= samePositionPort.PortPositionRatio+0.5f*direction*kMinRatioDiff;
//		}
//		// If we don't have a before port, assume that we are the first port on the edge.
//		else if(beforePort == null) {
//			if(afterPort == null) {
//				Debug.LogWarning("iCanScript: Problem with finding before & after ports on same edge.  Contact customer support.");
//	            DragObject.GlobalAnchorPosition= newPosition;                    					
//			} else {
//				float range= 0f;
//				float posInRange = 0f;
//				if(DragObject.IsOnHorizontalEdge) {
//					var rangeStart= parentGloablPos.x+parent.HorizontalPortsLeft;
//					var rangeEnd= afterPort.GlobalDisplayPosition.x;
//					range= rangeEnd-rangeStart;
//					posInRange= newPosition.x-rangeStart;
//				} else {
//					var rangeStart= parentGloablPos.y+parent.VerticalPortsTop;
//					var rangeEnd= afterPort.GlobalDisplayPosition.y;
//					range= rangeEnd-rangeStart;
//					posInRange= newPosition.y-rangeStart;
//				}
//				if(Math3D.IsZero(range)) {
//					DragObject.GlobalAnchorPosition= newPosition;
//				} else {
//					if(posInRange < 0f) posInRange= 0f;
//					DragObject.PortPositionRatio= Math3D.Lerp(0f, afterPort.PortPositionRatio, posInRange/range);						
//				}
//			}
//		}
//		// If we have a before port but no after port, we assume that we are the last port on the edge.
//		else if(afterPort == null) {
//			float range= 0f;
//			float posInRange = 0f;
//			if(DragObject.IsOnHorizontalEdge) {
//				var rangeStart= beforePort.GlobalDisplayPosition.x;
//				var rangeEnd= parentGloablPos.x+parent.HorizontalPortsRight;
//				range= rangeEnd-rangeStart;
//				posInRange= newPosition.x > rangeEnd ? range : newPosition.x-rangeStart;
//			} else {
//				var rangeStart= beforePort.GlobalDisplayPosition.y;
//				var rangeEnd= parentGloablPos.y+parent.VerticalPortsBottom;
//				range= rangeEnd-rangeStart;
//				posInRange= newPosition.y > rangeEnd ? range : newPosition.y-rangeStart;
//			}
//			if(Math3D.IsZero(range)) {
//				DragObject.GlobalAnchorPosition= newPosition;
//			} else {
//				if(posInRange < 0f) posInRange= 0f;
//				DragObject.PortPositionRatio= Math3D.Lerp(beforePort.PortPositionRatio, 1f, posInRange/range);						
//			}
//		}
//		// We have both a before & after port, so lets position ourself between them.
//		else {
//			float range= 0f;
//			float posInRange = 0f;
//			if(DragObject.IsOnHorizontalEdge) {
//				var rangeStart= beforePort.GlobalDisplayPosition.x;
//				var rangeEnd  = afterPort.GlobalDisplayPosition.x;
//				range= rangeEnd-rangeStart;
//				posInRange= newPosition.x-rangeStart;
//			} else {
//				var rangeStart= beforePort.GlobalDisplayPosition.y;
//				var rangeEnd  = afterPort.GlobalDisplayPosition.y;
//				range= rangeEnd-rangeStart;
//				posInRange= newPosition.y-rangeStart;
//			}
//			if(Math3D.IsZero(range)) {
//				DragObject.GlobalAnchorPosition= newPosition;
//			} else {
//				if(posInRange < 0f) posInRange= 0f;
//				DragObject.PortPositionRatio= Math3D.Lerp(beforePort.PortPositionRatio, afterPort.PortPositionRatio, posInRange/range);						
//			}
//		}
//        // Cleanup anchor positions.
//        var sortedPorts= iCS_EditorObject.SortPortsOnAnchor(sameEdgePorts);
//        for(int i= 0; i < sortedPorts.Length-1; ++i) {
//            var ratio= sortedPorts[i].PortPositionRatio;
//            if(ratio < 0) {
//                ratio= 0f;
//                sortedPorts[i].PortPositionRatio= ratio;
//            }
//            var nextRatio= sortedPorts[i+1].PortPositionRatio;
//            if(ratio+kMinRatioDiff > nextRatio) {
//                sortedPorts[i+1].PortPositionRatio= ratio+kMinRatioDiff;
//            }
//        }
//        for(int i= sortedPorts.Length-1; i > 0; --i) {
//            var ratio= sortedPorts[i].PortPositionRatio;
//            if(ratio > 1f) {
//                ratio= 1f;
//                sortedPorts[i].PortPositionRatio= ratio;
//            }
//            var prevRatio= sortedPorts[i-1].PortPositionRatio;
//            if(prevRatio > ratio-kMinRatioDiff) {
//                sortedPorts[i-1].PortPositionRatio= ratio-kMinRatioDiff;
//            }                
//        }
//        // Adjust display position & relayout all other ports.
//        DragObject.IsSticky= true;
//        parent.LayoutPorts();						
//		SetPortRelocationLayoutPosition(newPosition, parent, parentGloablPos, parentRect);
//        DragObject.IsSticky= false;
    }
	// ----------------------------------------------------------------------
	void SetPortRelocationLayoutPosition(Vector2 newPosition, iCS_EditorObject parent, Vector2 parentGloablPos, Rect parentRect) {
		float x,y;
		if(DragObject.IsOnHorizontalEdge) {
			x= newPosition.x;
			if(x < parentGloablPos.x+parent.HorizontalPortsLeft)  x= parentGloablPos.x+parent.HorizontalPortsLeft;
			if(x > parentGloablPos.x+parent.HorizontalPortsRight) x= parentGloablPos.x+parent.HorizontalPortsRight;
			y= DragObject.IsOnTopEdge  ? parentRect.yMin : parentRect.yMax;
			
		} else {
			x= DragObject.IsOnLeftEdge ? parentRect.xMin : parentRect.xMax;
			y= newPosition.y;
			if(y < parentGloablPos.y+parent.VerticalPortsTop)    y= parentGloablPos.y+parent.VerticalPortsTop;
			if(y > parentGloablPos.y+parent.VerticalPortsBottom) y= parentGloablPos.y+parent.VerticalPortsBottom;
		}
		DragObject.GlobalLayoutPosition= new Vector2(x,y);		
	}
	// ----------------------------------------------------------------------
    void AutocreateInstanceNode(Vector2 globalPosition, iCS_EditorObject newParent) {
        var dragPort= DragFixPort;
        Type instanceType= dragPort.RuntimeType;
        if(iCS_Types.IsStaticClass(instanceType)) return;
        if(DragOriginalPort != DragFixPort) return;
        var instance= IStorage.CreateModule(newParent.InstanceId, globalPosition, "", iCS_ObjectTypeEnum.Module, instanceType);
        var thisPort= IStorage.ClassModuleGetInputThisPort(instance);
        IStorage.SetSource(thisPort, dragPort);
    }
}
