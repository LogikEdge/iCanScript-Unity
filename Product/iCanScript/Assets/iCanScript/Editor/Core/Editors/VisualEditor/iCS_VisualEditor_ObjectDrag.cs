#define NEW_CODE

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
    enum DragTypeEnum {
        None,
        PortConnection,
        PortRelocation,
        NodeDrag,
        NodeRelocation,
        TransitionCreation,
        MultiSelectionNodeDrag,
        MultiSelectionNodeRelocation
     };


    // ======================================================================
    // Fields.
    // ----------------------------------------------------------------------
    DragTypeEnum     DragType                    = DragTypeEnum.None;
    iCS_EditorObject DragObject                  = null;
    iCS_EditorObject DragFixPort                 = null;
    iCS_EditorObject DragOriginalPort            = null;
    Vector2          MouseDragStartPosition      = Vector2.zero;
    Vector2          DragStartDisplayPosition    = Vector2.zero;
    Vector2          DragStartLocalAnchorPosition= Vector2.zero;
    bool             IsDragEnabled               = false;
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
        var originalSource= DragOriginalPort.ProducerPort;
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

        OnStartRelayoutOfTree();

        // Use the Left mouse down position has drag start position.
        MouseDragStartPosition= MouseDownPosition;
        Vector2 pos= ViewportToGraph(MouseDragStartPosition);

        // Port drag.
        iCS_EditorObject port= SelectedObject;
        if(port != null && port.IsPort && port.IsVisibleOnDisplay) {
            iCS_UserCommands.StartPortDrag(port);
            IStorage.CleanupDeadPorts= false;		// Suspend object cleanup.
            DragType= DragTypeEnum.PortRelocation;
            DragOriginalPort= port;
            DragFixPort= port;
            DragObject= port;
            DragStartDisplayPosition= port.GlobalPosition;
            DragObject.IsSticky= true;
            return true;
        }

        // Node drag.
        iCS_EditorObject node= SelectedObject;                
        if(node != null && node.IsNode && (node.IsIconizedOnDisplay || !node.IsState || myGraphics.IsNodeTitleBarPicked(node, pos))) {
            if(IsCopyKeyDown) {
				// Transform into Unity drag & drop protocol. 
                GameObject go= new GameObject(node.DisplayName+iCS_EditorStrings.SnippetTag);
                go.hideFlags = HideFlags.HideAndDontSave;
                iCS_LibraryImp library= iCS_DynamicCall.AddLibrary(go);
                iCS_IStorage iStorage= new iCS_IStorage(library);
                var newNode= iStorage.Copy(node, IStorage, null, Vector2.zero, iStorage);
                newNode.ReduceChildrenAnchorPosition();
                iStorage.SaveStorage();
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(node.DisplayName);
                iCS_AutoReleasePool.AutoRelease(go, 2f);
                // Disable dragging.
                IsDragEnabled= false;
                DragType= DragTypeEnum.None;
            } else {
                DragObject= node;
                DragStartDisplayPosition= node.GlobalPosition;                                                                    
                if(IsFloatingKeyDown && !node.IsTransitionPackage) {
                    if(IStorage.IsMultiSelectionActive) {
                        iCS_UserCommands.StartMultiSelectionNodeRelocation(IStorage);
                        DragType= DragTypeEnum.MultiSelectionNodeRelocation;
                        IStorage.StartMultiSelectionNodeRelocation();
                    }
                    else {
                        iCS_UserCommands.StartNodeRelocation(node);
                        DragType= DragTypeEnum.NodeRelocation;                                        
                        DragStartLocalAnchorPosition= node.LocalAnchorPosition;
    					node.StartNodeRelocate();                            
                    }
                } else {
                    if(IStorage.IsMultiSelectionActive) {
                        iCS_UserCommands.StartMultiSelectionNodeDrag(IStorage);
                        DragType= DragTypeEnum.MultiSelectionNodeDrag;                    
                        IStorage.StartMultiSelectionNodeDrag();                                                
                    }
                    else {
                        iCS_UserCommands.StartNodeDrag(node);
                        DragType= DragTypeEnum.NodeDrag;                    
                        node.StartNodeDrag();                        
                    }
                    
                }
            }
            return true;
        }

        // New state transition drag.
        if(node != null && node.IsState) {
            DragType= DragTypeEnum.TransitionCreation;
            iCS_EditorObject outStatePort= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), VSObjectType.OutStatePort);
            iCS_EditorObject inStatePort= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), VSObjectType.InStatePort);
            outStatePort.LocalAnchorFromGlobalPosition= pos;
            inStatePort.LocalAnchorFromGlobalPosition= pos;
            inStatePort.ProducerPortId= outStatePort.InstanceId;
            DragFixPort= outStatePort;
            DragObject= inStatePort;
            DragStartDisplayPosition= pos;
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
        Vector2 deltaMouse= ViewportMousePosition - MouseDragStartPosition;
        var newPosition= DragStartDisplayPosition + deltaMouse;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                DragObject.NodeDragTo(newPosition);
                break;
            case DragTypeEnum.MultiSelectionNodeDrag:
                IStorage.DragMultiSelectedNodesBy(deltaMouse);
                break;
            case DragTypeEnum.NodeRelocation:
                DragObject.NodeRelocateTo(newPosition);
                break;
            case DragTypeEnum.MultiSelectionNodeRelocation:
                IStorage.RelocateMultiSelectedNodesBy(deltaMouse);
                break;
            case DragTypeEnum.PortRelocation: {
				// Consider port relocation when dragging on parent edge.
                ProcessPortRelocation(DragObject, newPosition);
                break;
            }
            case DragTypeEnum.PortConnection: {
                // Determine if we should go back to port relocation. (IsPositionOnEdge)
                var originalPortParent= DragOriginalPort.ParentNode;
                var localPosition= newPosition-originalPortParent.GlobalPosition;
                var closestEdge= DragOriginalPort.GetClosestEdge(localPosition);
                if(originalPortParent.IsPositionOnEdge(newPosition, closestEdge)) {
                    RemoveDragPort();
                    DragOriginalPort.Edge= closestEdge;
                    ProcessPortRelocation(DragObject, newPosition);
                    break;
                }
                // Snap to nearby ports
                DragObject.CollisionOffsetFromGlobalPosition= newPosition;
                Vector2 mousePosInGraph= GraphMousePosition;
                iCS_EditorObject closestPort= IStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataOrControlPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Vector2 closestPortPos= closestPort.GlobalPosition;
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < iCS_EditorConfig.PortDiameter) {
                        DragObject.CollisionOffsetFromGlobalPosition= closestPortPos;
                    }                    
                }
                // Continously refresh drag port if module port.
                if(DragOriginalPort.IsKindOfPackagePort) {
                    CreateDragPort();
                }
                break;
            }
            case DragTypeEnum.TransitionCreation:
                // Update port position.
                DragObject.CollisionOffsetFromGlobalPosition= newPosition;
				// Update fix port edge & position.
				var fixPortParentNode= DragFixPort.ParentNode;
				var fixPortParentRect= fixPortParentNode.GlobalRect;
				var fixPortParentCenter= Math3D.Middle(fixPortParentRect);
				if(!fixPortParentRect.Contains(newPosition)) {
					var dir= newPosition-fixPortParentCenter;
					dir= Math3D.QuantizeAt90Degrees(dir);
					iCS_EdgeEnum edge= iCS_EdgeEnum.None;
					if(Math3D.IsZero(dir.x)) {
						edge= dir.y > 0 ? iCS_EdgeEnum.Bottom : iCS_EdgeEnum.Top;
					} else {
						edge= dir.x > 0 ? iCS_EdgeEnum.Right : iCS_EdgeEnum.Left;
					}
					if(edge != DragFixPort.Edge) {
						var center= fixPortParentCenter;
						switch(edge) {
						case iCS_EdgeEnum.Top:
							newPosition= new Vector2(center.x, fixPortParentRect.yMin);
							break;
						case iCS_EdgeEnum.Bottom:
							newPosition= new Vector2(center.x, fixPortParentRect.yMax);
							break;
						case iCS_EdgeEnum.Left:
							newPosition= new Vector2(fixPortParentRect.xMin, center.y);
							break;
						default:
							newPosition= new Vector2(fixPortParentRect.xMax, center.y);
							break;
						}
						DragFixPort.LocalAnchorFromGlobalPosition= newPosition;
						fixPortParentNode.LayoutPorts();
					}
				}
                break;
        }
    }    
	// ----------------------------------------------------------------------
    void EndDrag() {
		ProcessDrag();
        OnEndRelayoutOfTree();
        IStorage.CleanupDeadPorts= true;
        
        // End the drag according to the drag type.
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    // Remove sticky on parent nodes.
                    DragObject.EndNodeDrag();
                    iCS_UserCommands.EndNodeDrag(DragObject);
                    break;
                }
                case DragTypeEnum.MultiSelectionNodeDrag: {
                    IStorage.EndMultiSelectionNodeDrag();
                    iCS_UserCommands.EndMultiSelectionDrag(IStorage);
                    break;
                }
                case DragTypeEnum.NodeRelocation: {
                    DragObject.IsFloating= false;
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= node.Parent;
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(GraphMousePosition, node);
                        if(newParent != null) {
                            DragObject.EndNodeRelocate();
                            iCS_UserCommands.EndNodeRelocation(node, oldParent, newParent);
                            break;
                        } else {
                            // Animate node back to its original position.
							node.IsSticky= false;
                            IStorage.AnimateGraph(null,
                                _=> {
                                    node.LocalAnchorPosition= DragStartLocalAnchorPosition;
                                    node.CollisionOffsetFromGlobalPosition= DragStartDisplayPosition;
                                }
                            );
                            node.IsFloating= true;
                        }
                    }
                    iCS_UserCommands.CancelNodeRelocation(node);
                    // Remove sticky on parent nodes.
                    DragObject.EndNodeRelocate();
                    break;
                }
                case DragTypeEnum.MultiSelectionNodeRelocation: {
                    // Verify if new parent can accept all selected nodes
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= node.Parent;
                    if(oldParent != null) {
                        iCS_EditorObject newParent= null;
                        var selectedNodes= IStorage.FilterMultiSelectionForMove();
                        foreach(var n in selectedNodes) {
                            var foundParent= GetValidParentNodeUnder(GraphMousePosition, n);
                            if(foundParent != null) {
                                if(newParent == null) {
                                    newParent= foundParent;
                                }
                                else {
                                    if(foundParent != newParent) {
                                        if(foundParent.IsParentOf(newParent)) {
                                            newParent= foundParent;
                                        }
                                        else if(newParent.IsParentOf(foundParent) == false) {
                                            newParent= null;
                                            break;
                                        } 
                                    }
                                }
                            }
                        }
                        if(newParent != null) {
                            IStorage.EndMultiSelectionNodeRelocation();
                            iCS_UserCommands.EndMultiSelectionNodeRelocation(selectedNodes, oldParent, newParent);
                            break;
                        }
                    }
                    iCS_UserCommands.CancelMultiSelectionNodeRelocation(IStorage);
                    break;
                }
                case DragTypeEnum.PortRelocation:
                    DragObject.IsSticky= false;
                    DragObject.IsFloating= false;
                    if(DragObject.IsDataOrControlPort) {
						DragObject.ParentNode.LayoutPorts();
                        iCS_UserCommands.EndPortDrag(DragOriginalPort);
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
                        iCS_EditorObject newState= IStorage.GetNodeWithEdgeAt(DragObject.GlobalPosition);
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
                            iCS_UserCommands.EndPortDrag(DragOriginalPort);
                            break;
                        }
                        // Delete transition if the dragged port is not on a valid state.
                        if(newState == null || origStateChart != newStateChart) {
                            if(EditorUtility.DisplayDialog("Deleting Transition", "Are you sure you want to remove the dragged transition.", "Delete", "Cancel")) {
                                IStorage.DestroyInstance(DragObject);
                            } else {
                                DragObject.LocalAnchorFromGlobalPosition= DragStartDisplayPosition;
                            }
                            iCS_UserCommands.EndPortDrag(DragOriginalPort);
                            break;
                        }
                        // Relocate transition to the new state.
                        var dragObjectPosition= DragObject.GlobalPosition;
                        DragObject.Parent= newState;
                        DragObject.LocalAnchorFromGlobalPosition= dragObjectPosition;
                        DragObject.UpdatePortEdge();
                        newState.LayoutPorts();
                        iCS_EditorObject transitionPackage= IStorage.GetTransitionPackage(DragObject);
                        iCS_EditorObject otherStatePort= DragObject.IsInputPort ? IStorage.GetFromStatePort(transitionPackage) : IStorage.GetToStatePort(transitionPackage);
                        iCS_EditorObject otherState= otherStatePort.Parent;
                        iCS_EditorObject moduleParent= transitionPackage.Parent;
                        iCS_EditorObject newModuleParent= IStorage.GetTransitionParent(newState, otherState);
                        if(moduleParent != newModuleParent) {
                            transitionPackage.Parent= newModuleParent;
                            IStorage.LayoutTransitionPackage(transitionPackage);
                        }
                        iCS_UserCommands.EndPortDrag(DragOriginalPort);
                        break;
                    }
                    break;
                case DragTypeEnum.PortConnection:                
                    // Attempt new port binding.
                    if(VerifyNewDragConnection()) {
                        iCS_UserCommands.EndPortConnection(DragOriginalPort);
                        break;
                    }
					// Attempt to publish port on nearby package.
                    if(DragFixPort.IsDataOrControlPort) {
                        // We assume a disconnection.
						// The drag port is of no use anymore.  Let's cleanup...
                        var dragPortPos= DragObject.GlobalPosition;
                        DragObject.IsSticky= false;
                        DragObject.IsFloating= false;
						IStorage.DestroyInstance(DragObject);
						DragObject= null;
                        // -- Avoid publishing on the parent node. --
                        iCS_EditorObject newPortParent= GetNodeWithEdgeAtMousePosition();
						if(newPortParent != null && newPortParent == DragOriginalPort.ParentNode) {
			                DragOriginalPort.LocalAnchorFromGlobalPosition= dragPortPos;
                            iCS_UserCommands.EndPortConnection(DragOriginalPort);
							break;
						}
                        // -- Get node with an edge close to the drag position. --
                        if(newPortParent != null && newPortParent.IsKindOfPackage) {
							// -- We found a suitable package.  Let's create the appropriate port. --
                            iCS_EditorObject portParent= DragFixPort.ParentNode;
							bool isFixParentOfNew= portParent.IsParentOf(newPortParent);
							bool isNewParentOfFix= newPortParent.IsParentOf(portParent);
							VSObjectType newPortType;
							bool fixPortIsBindingTo;
							if(isFixParentOfNew) {
								if(DragFixPort.IsInputPort) {
									newPortType= VSObjectType.InDynamicDataPort;
									fixPortIsBindingTo= false;
								}
								else {
									newPortType= VSObjectType.OutDynamicDataPort;
									fixPortIsBindingTo= true;										
								}
							}
							else if(isNewParentOfFix) {
								if(DragFixPort.IsInputPort) {
									newPortType= VSObjectType.InDynamicDataPort;
									fixPortIsBindingTo= true;
								}
								else {
									newPortType= VSObjectType.OutDynamicDataPort;
									fixPortIsBindingTo= false;										
								}
							}
							else {
								if(DragFixPort.IsInputPort) {
									newPortType= VSObjectType.OutDynamicDataPort;
									fixPortIsBindingTo= true;
								}
								else {
									newPortType= VSObjectType.InDynamicDataPort;
									fixPortIsBindingTo= false;																				
								}									
							}
                            // -- Public Function should use "proposed ports" instead of dynamic ports --
                            if(newPortParent.IsFunctionDefinition) {
                                newPortType= newPortType == VSObjectType.InDynamicDataPort ?
                                    VSObjectType.InProposedDataPort :
                                    VSObjectType.OutProposedDataPort;
                            }
                            iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.DisplayName, newPortParent.InstanceId, DragFixPort.RuntimeType, newPortType);
                            newPort.PortSpec= DragFixPort.PortSpec;
                            newPort.LocalAnchorFromGlobalPosition= dragPortPos;
                            newPort.Value= DragFixPort.Value;
                            iCS_EditorObject providerPort= null;
                            iCS_EditorObject consumerPort= null;
							if(fixPortIsBindingTo) {
                                consumerPort= DragFixPort;
                                providerPort= newPort;
							}
							else {
                                consumerPort= newPort;
                                providerPort= DragFixPort;				
							}
                            IStorage.SetNewDataConnection(consumerPort, providerPort);
                            IStorage.AutoLayoutOfPointToPointBindingExclusive(providerPort, consumerPort);
                            iCS_UserCommands.EndPortPublishing(DragOriginalPort);
                            break;
						}
                        // Attempt to quick create node if a disconnection was not performed.
						if(DragFixPort == DragOriginalPort) { // This is not a disconnection
                            newPortParent= GetNodeAtMousePosition();
                            if(newPortParent != null && newPortParent.IsKindOfPackage) {
                                QuickNodeCreate(dragPortPos, newPortParent);
                                iCS_UserCommands.EndPortConnection(DragOriginalPort);
                                break;                                  
                            }								
						}
						/*
							CHANGED : CODE REVIEW NEEDED => Publishing to state chart.
						*/
						// Allow output data connection on state modules.
                        if(DragFixPort.IsOutputPort && newPortParent != null && (newPortParent.IsState || newPortParent.IsStateChart)) {
							if(newPortParent.IsPositionOnEdge(dragPortPos, iCS_EdgeEnum.Right)) {
                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.DisplayName, newPortParent.InstanceId, DragFixPort.RuntimeType, VSObjectType.OutDynamicDataPort);
                                IStorage.SetNewDataConnection(newPort, DragFixPort);
                                iCS_UserCommands.EndPortConnection(DragOriginalPort);
								break;
							}
                        }
						// Cleanup child Mux port if it is disconnected.
						if(DragOriginalPort.IsChildMuxPort && DragOriginalPort.ProducerPort == null) {
                            var muxParent= DragOriginalPort.Parent;
							IStorage.DestroyInstance(DragOriginalPort);
                            if(muxParent != null && muxParent.IsParentMuxPort) {
                                IStorage.CleanupMuxPort(muxParent);                                
                            }
						}
                    }                    
                    iCS_UserCommands.EndPortConnection(DragOriginalPort);
					break;
                case DragTypeEnum.TransitionCreation:
                    iCS_EditorObject destState= GetNodeAtMousePosition();
                    if(destState != null && destState.IsState) {
                        iCS_EditorObject outStatePort= IStorage[DragObject.ProducerPortId];
                        outStatePort.IsFloating= false;
                        outStatePort.IsSticky= false;
                        var toPosition= DragObject.GlobalPosition;
                        DragObject.ProducerPortId= -1;
                        IStorage.DestroyInstance(DragObject);
                        iCS_UserCommands.CreateTransition(outStatePort, destState, toPosition);
                    } else {
                        IStorage.DestroyInstance(DragObject.ProducerPortId);
                        DragObject.ProducerPortId= -1;
                        IStorage.DestroyInstance(DragObject);
                    }
                    break;
            }            
        }

        finally {
            // Validate end drag operations.
            IStorage.ClearUserTransactions();
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
		if(!DragOriginalPort.IsKindOfPackagePort) {
			// Determine if we are already properly connected.
			if(DragObject != DragOriginalPort) return;
			if(DragOriginalPort.IsInputPort) {
				// Create the appropriate drag port.
				var sourcePort= DragOriginalPort.ProducerPort;
				if(sourcePort != null) {	// Disconnect if the port is connected.
					RemoveDragPort();
					var sourceParent= sourcePort.ParentNode;
		            var newPort= IStorage.CreatePort(sourcePort.DisplayName,
													 sourceParent.InstanceId,
													 sourcePort.RuntimeType,
													 VSObjectType.InDynamicDataPort);
                    newPort.PortSpec= sourcePort.PortSpec;
					DragFixPort= sourcePort;
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragFixPort);
					IStorage.SetSource(DragOriginalPort, null);
				} else {					// Input is not connected so simply connect the drag port
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 VSObjectType.OutDynamicDataPort);
                    newPort.PortSpec= DragOriginalPort.PortSpec;
					DragObject= newPort;
					DragFixPort= DragOriginalPort;
					IStorage.SetSource(DragOriginalPort, DragObject);
				}
			} else {	// Output port.
				RemoveDragPort();
	            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
												 parent.InstanceId,
												 DragOriginalPort.RuntimeType,
												 VSObjectType.InDynamicDataPort);
                newPort.PortSpec= DragOriginalPort.PortSpec;
				DragObject= newPort;
				DragFixPort= DragOriginalPort;
				IStorage.SetSource(DragObject, DragFixPort);
			}
		}
		else {	// This is a module port.
			var point= GraphMousePosition;
			bool isInside= DragOriginalPort.ParentNode.GlobalRect.Contains(point);
			if(DragOriginalPort.IsInputPort) {	// Input module port
				if(isInside) {	// Inside parent node
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   DragFixPort == DragOriginalPort &&
					   DragObject.ProducerPort == DragFixPort) {
						return;
					}
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 VSObjectType.InDynamicDataPort);
                    newPort.PortSpec= DragOriginalPort.PortSpec;
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragOriginalPort);
					DragFixPort= DragOriginalPort;
				}
				else {		// Outside parent node
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   ((DragFixPort != DragOriginalPort && DragObject.ProducerPort == DragFixPort) ||
					    (DragFixPort == DragOriginalPort && DragOriginalPort.ProducerPort == DragObject))) {
						return;
					}
					RemoveDragPort();
					var sourcePort= DragOriginalPort.ProducerPort;
					if(sourcePort != null) {
						var sourceParent= sourcePort.ParentNode;
			            var newPort= IStorage.CreatePort(sourcePort.DisplayName,
														 sourceParent.InstanceId,
														 sourcePort.RuntimeType,
														 VSObjectType.InDynamicDataPort);
                        newPort.PortSpec= sourcePort.PortSpec;
						DragFixPort= sourcePort;
						DragObject= newPort;
						IStorage.SetSource(DragObject, DragFixPort);
						IStorage.SetSource(DragOriginalPort, null);
					}
					else {
			            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
														 parent.InstanceId,
														 DragOriginalPort.RuntimeType,
														 VSObjectType.OutDynamicDataPort);
                        newPort.PortSpec= DragOriginalPort.PortSpec;
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
                       (DragFixPort != DragOriginalPort && DragObject.ProducerPort == DragFixPort) ||
                       (DragFixPort == DragOriginalPort && DragOriginalPort.ProducerPort == DragObject)) {
                        return;    
                    }
					RemoveDragPort();
					var sourcePort= DragOriginalPort.ProducerPort;
					if(sourcePort != null) {
						var sourceParent= sourcePort.ParentNode;
			            var newPort= IStorage.CreatePort(sourcePort.DisplayName,
														 sourceParent.InstanceId,
														 sourcePort.RuntimeType,
														 VSObjectType.InDynamicDataPort);
                        newPort.PortSpec= sourcePort.PortSpec;
						DragFixPort= sourcePort;
						DragObject= newPort;
						IStorage.SetSource(DragObject, DragFixPort);
                        IStorage.SetSource(DragOriginalPort, null);
					}
					else {
			            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
														 parent.InstanceId,
														 DragOriginalPort.RuntimeType,
														 VSObjectType.OutDynamicDataPort);
                        newPort.PortSpec= DragOriginalPort.PortSpec;
						DragObject= newPort;
						IStorage.SetSource(DragOriginalPort, DragObject);
						DragFixPort= DragOriginalPort;
					}
				}
				else {		// Outside parent node
					// Nothing to do if already properly connected.
					if(DragObject != DragOriginalPort &&
					   DragFixPort == DragOriginalPort &&
					   DragObject.ProducerPort == DragOriginalPort) {
						return;
					}
					RemoveDragPort();
		            var newPort= IStorage.CreatePort(DragOriginalPort.DisplayName,
													 parent.InstanceId,
													 DragOriginalPort.RuntimeType,
													 VSObjectType.InDynamicDataPort);
                    newPort.PortSpec= DragOriginalPort.PortSpec;
					DragObject= newPort;
					IStorage.SetSource(DragObject, DragOriginalPort);
					DragFixPort= DragOriginalPort;
				}
			}
		}
        DragType= DragTypeEnum.PortConnection;
        DragObject.LocalAnchorFromGlobalPosition= GraphMousePosition;
        DragObject.IsFloating= true;
        DragObject.IsSticky= true;
        SelectedObject= DragObject;
	}

	// ----------------------------------------------------------------------
	// TODO: Move port relocation processing into editor object...
    void ProcessPortRelocation(iCS_EditorObject port, Vector2 newPosition) {
		var parent= port.ParentNode;
		var parentRect= parent.GlobalRect;
        // Determine if we should convert to data port connection drag.
		if(port.IsDataOrControlPort && !iCS_EditorObject.IsPositionOnRectEdge(newPosition, parentRect, port.Edge)) {
            parent.LayoutPorts();
            CreateDragPort();
            return;
		}                      
        // Determine new edge
        var currentEdge= port.Edge;
        var newEdge= port.GetClosestEdge(newPosition-Math3D.Middle(parentRect));
        if(newEdge != currentEdge) {
            port.LocalAnchorFromGlobalPosition= newPosition;            
        }
        // No reordering necessary if this is the only port on this edge.
        var sameEdgePorts= P.filter(p=> p != port, port.BuildListOfPortsOnSameEdge());
		var nbOfPortsOnEdge= sameEdgePorts.Length;
        if(nbOfPortsOnEdge == 0) {
            port.LocalAnchorFromGlobalPosition= newPosition;
			return;
        }
		// Build port related information (sortedPorts, portRatios, and portGlobalPositions).
		var sortedPorts        = iCS_EditorObject.SortPortsOnLayout(sameEdgePorts);
		var portRatios         = P.map(p=> p.PortPositionRatio, sortedPorts);
		var portPositionsOnEdge= P.map(p=> (p.IsOnHorizontalEdge ? p.GlobalPosition.x : p.GlobalPosition.y), sortedPorts);
		var newPositionOnEdge  = port.IsOnHorizontalEdge ? newPosition.x : newPosition.y;
		var delta              = newPosition-port.GlobalPosition;
		var deltaOnEdge        = port.IsOnHorizontalEdge ? delta.x : delta.y;
		var parentGlobalPos    = parent.GlobalPosition;
		var edgePosStart       = port.IsOnHorizontalEdge ?
									parentGlobalPos.x+parent.HorizontalPortsLeft :
									parentGlobalPos.y+parent.VerticalPortsTop;
		var edgePosEnd         = port.IsOnHorizontalEdge ?
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
                // Here...
				break;
			}
		}

        // Use full node range if no collision exists.
        float minSeparation= iCS_EditorConfig.kMinimumPortSeparation;
		float rangeRatioStart= 0f;
        float rangeRatioEnd= 1f;
		float rangePosStart= edgePosStart;
        float rangePosEnd= edgePosEnd;
        bool isColliding= false;
        if(index == 0) {
            if(Mathf.Abs(portPositionsOnEdge[0]-newPositionOnEdge) < minSeparation) {
                isColliding= true;
            }
        }
        else if(index == nbOfPortsOnEdge) {
            if(Mathf.Abs(portPositionsOnEdge[index-1]-newPositionOnEdge) < minSeparation) {
                isColliding= true;
            }
        }
        else {
            if(Mathf.Abs(portPositionsOnEdge[index-1]-newPositionOnEdge) < minSeparation) {
                isColliding= true;
            }
            if(Mathf.Abs(portPositionsOnEdge[index]-newPositionOnEdge) < minSeparation) {
                isColliding= true;
            }            
        }

		// Determine proper anchor ratio when collision is present.
        if(isColliding) {
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
		port.PortPositionRatio= ratio;
		// ... rebuild port lists to include the drag port.
		sortedPorts        = P.insertAt(port,        index, sortedPorts);
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
        newPosition= CleanupPortGlobalPositionOnEdge(port, newPosition, parent, parentGlobalPos, parentRect);
		portPositionsOnEdge[index]= port.IsOnHorizontalEdge ? newPosition.x : newPosition.y;
        for(int i= index; i < nbOfPortsOnEdge-1; ++i) {
            var posDiff= portPositionsOnEdge[i+1]-portPositionsOnEdge[i];
            if(posDiff < minSeparation) {
                var offset= minSeparation-posDiff;
                var pos= sortedPorts[i+1].GlobalPosition;
                if(port.IsOnHorizontalEdge) {
                    pos.x+= offset;
                } else {
                    pos.y+= offset;
                }
                sortedPorts[i+1].CollisionOffsetFromGlobalPosition= pos;
                portPositionsOnEdge[i+1]+= offset;
            }
        }
        for(int i= index; i > 0; --i) {
            var posDiff= portPositionsOnEdge[i]-portPositionsOnEdge[i-1];
            if(posDiff < minSeparation) {
                var offset= minSeparation-posDiff;
                var pos= sortedPorts[i-1].GlobalPosition;
                if(port.IsOnHorizontalEdge) {
                    pos.x-= offset;
                } else {
                    pos.y-= offset;
                }
                sortedPorts[i-1].CollisionOffsetFromGlobalPosition= pos;
                portPositionsOnEdge[i-1]-= offset;
            }
        }
        // Finaly, set the drag port layout position.
        parent.LayoutPorts();
    }
	// ----------------------------------------------------------------------
    Vector2 CleanupPortGlobalPositionOnEdge(iCS_EditorObject port, Vector2 pos, iCS_EditorObject parent, Vector2 parentGloablPos, Rect parentRect) {
		float x,y;
		if(port.IsOnHorizontalEdge) {
			x= pos.x;
			if(x < parentGloablPos.x+parent.HorizontalPortsLeft)  x= parentGloablPos.x+parent.HorizontalPortsLeft;
			if(x > parentGloablPos.x+parent.HorizontalPortsRight) x= parentGloablPos.x+parent.HorizontalPortsRight;
			y= port.IsOnTopEdge  ? parentRect.yMin : parentRect.yMax;
			
		} else {
			x= port.IsOnLeftEdge ? parentRect.xMin : parentRect.xMax;
			y= pos.y;
			if(y < parentGloablPos.y+parent.VerticalPortsTop)    y= parentGloablPos.y+parent.VerticalPortsTop;
			if(y > parentGloablPos.y+parent.VerticalPortsBottom) y= parentGloablPos.y+parent.VerticalPortsBottom;
		}
		return new Vector2(x,y);        
    }
	// ----------------------------------------------------------------------
    void QuickNodeCreate(Vector2 globalPosition, iCS_EditorObject newParent) {
#if NEW_CODE
		var port= DragFixPort;
		var parent= port.ParentNode;
		bool reverseInOut= false;
		if(parent == newParent || parent.IsParentOf(newParent)) {
			reverseInOut= true;
		}
		myContextualMenu.Update(iCS_ContextualMenu.MenuType.ReleaseAfterDrag, port, IStorage, globalPosition, reverseInOut);
#else
        var sourcePort= DragFixPort;
        Type instanceType= iCS_Types.GetElementType(sourcePort.RuntimeType);
        if(iCS_Types.IsStaticClass(instanceType)) return;
        if(DragOriginalPort != DragFixPort) return;
        var instance= IStorage.CreatePackage(newParent.InstanceId, globalPosition, "", VSObjectType.Package, instanceType);
        var thisPort= IStorage.PropertiesWizardGetInputThisPort(instance);
        SetNewDataConnection(thisPort, sourcePort);
#endif
    }
}
}