using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

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
        if(DragFixPort != DragOriginalPort) IStorage.SetSource(DragOriginalPort, DragFixPort);
        IStorage.SetSource(DragObject, DragOriginalPort);
        DragFixPort= DragOriginalPort;
    }
	// ----------------------------------------------------------------------
    void BreakDataConnectionDrag() {
        var originalSource= IStorage.GetSource(DragOriginalPort);
        if(originalSource != null && originalSource != DragObject) {
            DragFixPort= originalSource;
            IStorage.SetSource(DragObject, DragFixPort);
            IStorage.SetSource(DragOriginalPort, null);
        } else {
            if(DragFixPort == DragOriginalPort) {
                IStorage.SetSource(DragFixPort, DragObject);
            }
        }
    }
	// ----------------------------------------------------------------------
    bool StartDrag() {
        // Don't select new drag type if drag already started.
        if(IsDragStarted) return true;

        // Disable graph animation will dragging.
        IStorage.AnimateLayout= false;

        // Use the Left mouse down position has drag start position.
        MouseDragStartPosition= MouseDownPosition;
        Vector2 pos= ViewportToGraph(MouseDragStartPosition);

        // Port drag.
        iCS_EditorObject port= SelectedObject;
        if(port != null && port.IsPort && !IStorage.IsMinimized(port) && !port.IsTransitionPort) {
            IStorage.RegisterUndo("Port Drag");
            IStorage.CleanupDeadPorts= false;
            DragType= DragTypeEnum.PortRelocation;
            DragOriginalPort= port;
            DragFixPort= port;
            DragObject= port;
            DragObject.IsFloating= true;
            DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
            return true;
        }

        // Node drag.
        iCS_EditorObject node= SelectedObject;                
        if(node != null && node.IsNode && (node.IsMinimized || !node.IsState || myGraphics.IsNodeTitleBarPicked(node, pos, IStorage))) {
            if(IsCopyKeyDown) {
                GameObject go= new GameObject(node.Name);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent("iCS_Library");
                iCS_Library library= go.GetComponent<iCS_Library>();
                iCS_IStorage iStorage= new iCS_IStorage(library);
                iStorage.CopyFrom(node, IStorage, null, Vector2.zero);
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
                Rect nodePos= IStorage.GetLayoutPosition(node);
                DragStartPosition= new Vector2(nodePos.x, nodePos.y);                                                                    
            }
            return true;
        }

        // New state transition drag.
        if(node != null && node.IsState) {
            IStorage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            iCS_EditorObject outTransition= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutStatePort);
            iCS_EditorObject inTransition= IStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
            IStorage.SetInitialPosition(outTransition, pos);
            IStorage.SetInitialPosition(inTransition, pos);
            inTransition.Source= outTransition.InstanceId;
            DragFixPort= outTransition;
            DragObject= inTransition;
            DragStartPosition= new Vector2(DragObject.LocalPosition.x, DragObject.LocalPosition.y);
            DragObject.IsFloating= true;
            return true;
        }

        // Disable dragging since mouse is not over Node or Port.
        IStorage.AnimateLayout= true;
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
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                iCS_EditorObject node= DragObject;
                node.IsFloating= IsFloatingKeyDown;
                IStorage.MoveTo(node, DragStartPosition+delta);
                IStorage.SetDirty(node);
                break;
            case DragTypeEnum.PortRelocation: {
				// We can't relocate a mux port child.
				if(DragObject.IsInMuxPort) {
					CreateDragPort();
					return;
				}
                // Update port position.
                Vector2 newLocalPos= DragStartPosition+delta;
                DragObject.LocalPosition.x= newLocalPos.x;
                DragObject.LocalPosition.y= newLocalPos.y;
                if(DragObject.IsStatePort) break;
                // Determine if we should convert to data port connection drag.
                iCS_EditorObject parent= IStorage.GetParentNode(DragOriginalPort);
                if(!IStorage.IsNearParentEdge(DragObject)) {
					CreateDragPort();
                } else {
                    IStorage.PositionOnEdge(DragObject);
                    IStorage.LayoutPorts(parent); 
                }
                break;
            }
            case DragTypeEnum.PortConnection: {
                // Update port position.
                Vector2 newLocalPos= DragStartPosition+delta;
                DragObject.LocalPosition.x= newLocalPos.x;
                DragObject.LocalPosition.y= newLocalPos.y;
                // Determine if we should go back to port relocation.
                if(!DragOriginalPort.IsInMuxPort && IStorage.IsNearParentEdge(DragObject, DragOriginalPort.Edge)) {
                    iCS_EditorObject dragObjectSource= IStorage.GetSource(DragObject);
                    if(dragObjectSource != DragOriginalPort) {
                        IStorage.SetSource(DragOriginalPort, dragObjectSource);
                    }
                    IStorage.DestroyInstance(DragObject);
                    DragObject= DragOriginalPort;
                    DragFixPort= DragOriginalPort;
                    DragObject.IsFloating= true;
                    DragType= DragTypeEnum.PortRelocation;
                    break;
                }
                // Snap to nearby ports
                Vector2 mousePosInGraph= GraphMousePosition;
                iCS_EditorObject closestPort= IStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Rect closestPortRect= IStorage.GetLayoutPosition(closestPort);
                    Vector2 closestPortPos= new Vector2(closestPortRect.x, closestPortRect.y);
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < 4f*iCS_Config.PortRadius) {
                        Rect parentPos= IStorage.GetLayoutPosition(IStorage.GetParent(DragObject));
                        DragObject.LocalPosition.x= closestPortRect.x-parentPos.x;
                        DragObject.LocalPosition.y= closestPortRect.y-parentPos.y;
                    }                    
                }
                // Special case for module ports.
                if(DragOriginalPort.IsModulePort) {
                    if(IStorage.IsInside(IStorage.GetParent(DragOriginalPort), mousePosInGraph)) {
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
                Vector2 newLocalPos= DragStartPosition+delta;
                DragObject.LocalPosition.x= newLocalPos.x;
                DragObject.LocalPosition.y= newLocalPos.y;
                break;
        }
    }    
	// ----------------------------------------------------------------------
    void EndDrag() {
        IStorage.AnimateLayout= true;
		ProcessDrag();
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= IStorage.GetParent(node);
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(GraphMousePosition, node);
                        if(newParent != null) {
                            if(newParent != oldParent) {
                                ChangeParent(node, newParent);
                            }
                        } else {
                            IStorage.MoveTo(node, DragStartPosition);
                        }
                        IStorage.SetDirty(oldParent);                        
                    }
                    node.IsFloating= false;
                    break;
                }
                case DragTypeEnum.PortRelocation:
                    DragObject.IsFloating= false;
                    if(DragObject.IsDataPort) {
                        IStorage.LayoutPorts(IStorage.GetParent(DragObject));
                        // Verify for autocreation of instance node
                        break;
                    }                    
                    if(DragObject.IsStatePort) {
                        // Get original port state & state chart.
                        iCS_EditorObject origState= IStorage.GetParent(DragObject);
                        iCS_EditorObject origStateChart= IStorage.GetParent(origState);
                        while(origStateChart != null && !origStateChart.IsStateChart) {
                            origStateChart= IStorage.GetParent(origStateChart);
                        }
                        // Get new drag port state & state chart.
                        Rect dragObjRect= IStorage.GetLayoutPosition(DragObject);
                        Vector2 dragObjPos= new Vector2(dragObjRect.x, dragObjRect.y);
                        iCS_EditorObject newState= GetStateAt(dragObjPos);
                        iCS_EditorObject newStateChart= null;
                        if(newState != null) {
                            newStateChart= IStorage.GetParent(newState);
                            while(newStateChart != null && !newStateChart.IsStateChart) {
                                newStateChart= IStorage.GetParent(newStateChart);
                            }
                        }
                        // Relocate dragged port if on the same state.
                        if(origState == newState) {
                            IStorage.SetDirty(DragObject);
//                            DragObject.LocalPosition.x= DragStartPosition.x;
//                            DragObject.LocalPosition.y= DragStartPosition.y;
                            break;
                        }
                        // Delete transition if the dragged port is not on a valid state.
                        if(newState == null || origStateChart != newStateChart) {
                            if(EditorUtility.DisplayDialog("Deleting Transition", "Are you sure you want to remove the dragged transition.", "Delete", "Cancel")) {
                                IStorage.DestroyInstance(DragObject);
                            } else {
                                DragObject.LocalPosition.x= DragStartPosition.x;
                                DragObject.LocalPosition.y= DragStartPosition.y;                                    
                            }
                            break;
                        }
                        // Relocate transition to the new state.
                        IStorage.SetParent(DragObject, newState);
                        iCS_EditorObject transitionModule= IStorage.GetTransitionModule(DragObject);
                        iCS_EditorObject otherStatePort= DragObject.IsInputPort ? IStorage.GetFromStatePort(transitionModule) : IStorage.GetToStatePort(transitionModule);
                        iCS_EditorObject otherState= IStorage.GetParent(otherStatePort);
                        iCS_EditorObject moduleParent= IStorage.GetParent(transitionModule);
                        iCS_EditorObject newModuleParent= IStorage.GetTransitionParent(newState, otherState);
                        if(moduleParent != newModuleParent) {
                            IStorage.SetParent(transitionModule, newModuleParent);
                            IStorage.LayoutTransitionModule(transitionModule);
                        }
                        break;
                    }
                    break;
                case DragTypeEnum.PortConnection:                
                    // Verify for a new connection.
                    if(!VerifyNewDragConnection(DragFixPort, DragObject)) {
                        bool isNearParent= IStorage.IsNearParent(DragObject);
                        if(DragFixPort.IsDataPort) {
                            // We don't need the drag port anymore.
                            Rect dragPortPos= IStorage.GetLayoutPosition(DragObject);
                            IStorage.DestroyInstance(DragObject);
                            // Verify for disconnection.
                            if(!isNearParent) {
                                // Let's determine if we want to create a module port.
                                iCS_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent == null) break;
                                if(newPortParent.IsModule) {
                                    iCS_EditorObject portParent= IStorage.GetParent(DragFixPort);
                                    Rect modulePos= IStorage.GetLayoutPosition(newPortParent);
                                    float portSize2= 2f*iCS_Config.PortSize;
                                    if(DragFixPort.IsInputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(!IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;                                                
                                            }
                                        }                                    
                                    }
                                    if(DragFixPort.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!IStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= IStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;
                                            }
                                        }
                                    }
                                    // Determine if we need to create an instance node.
                                    AutocreateInstanceNode(DragFixPort, dragPortPos, newPortParent);                                    
                                }
                                if(DragFixPort.IsOutputPort && (newPortParent.IsState || newPortParent.IsStateChart)) {
									if(IStorage.IsNearNodeEdge(newPortParent, Math3D.ToVector2(dragPortPos), iCS_EditorObject.EdgeEnum.Right)) {
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
                        iCS_EditorObject outStatePort= IStorage[DragObject.Source];
                        outStatePort.IsFloating= false;
                        IStorage.CreateTransition(outStatePort, destState);
                        DragObject.Source= -1;
                        IStorage.DestroyInstance(DragObject);
                    } else {
                        IStorage.DestroyInstance(DragObject.Source);
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
        iCS_EditorObject parent= IStorage.GetParentNode(DragOriginalPort);
        DragObject.IsFloating= false;
        if(DragOriginalPort.IsInputPort) {
            DragObject= IStorage.CreatePort(DragOriginalPort.Name, parent.InstanceId, DragOriginalPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            iCS_EditorObject prevSource= IStorage.GetSource(DragOriginalPort);
            if(prevSource != null) {
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
        Rect portPos= IStorage.GetLayoutPosition(DragOriginalPort);
        IStorage.SetInitialPosition(DragObject, new Vector2(portPos.x, portPos.y));
		IStorage.GetEditorObjectCache(DragObject).AnimatedPosition.Reset(portPos);
		Rect parentPos= IStorage.GetLayoutPosition(parent);
		// Reset initial position if port is being dettached from it original parent.
		if(DragOriginalPort.IsInMuxPort) {
			DragStartPosition= Math3D.ToVector2(portPos)-Math3D.ToVector2(parentPos);			
		}
        DragObject.IsFloating= true;		
	}

	// ----------------------------------------------------------------------
    void AutocreateInstanceNode(iCS_EditorObject dragPort, Rect pos, iCS_EditorObject newParent) {
        Type instanceType= dragPort.RuntimeType;
        if(iCS_Types.IsStaticClass(instanceType)) return;
        var instance= IStorage.CreateModule(newParent.InstanceId, Math3D.ToVector2(pos), "", iCS_ObjectTypeEnum.Module, instanceType);
        var thisPort= IStorage.ClassModuleGetInputThisPort(instance);
        IStorage.SetSource(thisPort, dragPort);
    }
}
