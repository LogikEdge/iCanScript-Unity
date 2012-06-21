using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

/*
    TODO: Should show message saying that no ICS script is selected.
*/
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the iCS_Behaviour.
public partial class iCS_GraphEditor : iCS_EditorBase {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortConnection, PortRelocation, NodeDrag, TransitionCreation };

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    iCS_EditorObject    myDisplayRoot= null;
    iCS_DynamicMenu     myDynamicMenu= null;
    iCS_Graphics        myGraphics   = null;
    iCS_IStorage        myPreviousIStorage= null;
    
    // ----------------------------------------------------------------------
    int   myRefreshCounter= 0;
    float myCurrentTime   = 0;
    float myDeltaTime     = 0;
    
    // ----------------------------------------------------------------------
    Prelude.Animate<Vector2>    myAnimatedScrollPosition= new Prelude.Animate<Vector2>();
    Prelude.Animate<float>      myAnimatedScale         = new Prelude.Animate<float>();
    
    // ----------------------------------------------------------------------
    DragTypeEnum     DragType              = DragTypeEnum.None;
    iCS_EditorObject DragObject            = null;
    iCS_EditorObject DragFixPort           = null;
    iCS_EditorObject DragOriginalPort      = null;
    Vector2          MouseDragStartPosition= Vector2.zero;
    Vector2          DragStartPosition     = Vector2.zero;
    bool             IsDragEnabled         = false;
    bool             IsDragStarted         { get { return IsDragEnabled && DragObject != null; }}

    // ----------------------------------------------------------------------
    iCS_EditorObject SelectedObjectBeforeMouseDown= null;
    iCS_EditorObject myBookmark= null;
	bool			 ShouldRotateMuxPort= false;
    
    // ----------------------------------------------------------------------
    Rect    ClipingArea    { get { return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height); }}
    Vector2 ViewportCenter { get { return new Vector2(0.5f/Scale*position.width, 0.5f/Scale*position.height); } }
    Rect    Viewport       { get { return new Rect(0,0,position.width/Scale, position.height/Scale); }}
    Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;
     
    // ======================================================================
    // Properties
	// ----------------------------------------------------------------------
    iCS_EditorObject StorageRoot {
        get {
            if(IStorage == null || Prelude.length(IStorage.EditorObjects) == 0) return null;
            return IStorage.EditorObjects[0];
        }
    }
	// ----------------------------------------------------------------------
    Vector2     ScrollPosition {
        get { return IStorage != null ? IStorage.ScrollPosition : Vector2.zero; }
        set { if(IStorage != null) IStorage.ScrollPosition= value; }
    }
    float       Scale {
        get { return IStorage != null ? IStorage.GuiScale : 1.0f; }
        set {
            if(value > 2f) value= 2f;
            if(value < 0.15f) value= 0.15f;
            if(IStorage != null) IStorage.GuiScale= value;
        }
    }

	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus    { get { return EditorWindow.focusedWindow == MyWindow; }}
    bool    IsFloatingKeyDown	{ get { return Event.current.control; }}
    bool    IsCopyKeyDown       { get { return Event.current.shift; }}
    bool    IsScaleKeyDown      { get { return Event.current.alt; }}
    bool    IsShiftKeyDown      { get { return Event.current.shift; }}
    
	// ----------------------------------------------------------------------
	void UpdateMouse() {
        myMousePosition= Event.current.mousePosition;
        if(Event.current.type == EventType.MouseDrag) myMousePosition+= Event.current.delta;
	}
    Vector2 RealMousePosition  { get { return myMousePosition; }}
    Vector2 MousePosition      { get { return myMousePosition/Scale; } }
    Vector2 MouseGraphPosition { get { return ViewportToGraph(MousePosition); }}
	Vector2 myMousePosition  = Vector2.zero;
	Vector2 MouseDownPosition= Vector2.zero;
	float   MouseUpTime      = 0f;
	
    // ======================================================================
    // INITIALIZATION
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Note that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	public new void OnEnable() {        
        base.OnEnable();
        
		// Tell Unity we want to be informed of move drag events
		MyWindow.wantsMouseMove= true;

        // Create worker objects.
        myGraphics   = new iCS_Graphics();
        myDynamicMenu= new iCS_DynamicMenu();
        
        // Inspect the assemblies for components.
        if(!ourAlreadyParsed) {
            ourAlreadyParsed= true;
            iCS_Reflection.ParseAppDomain();
        }
        
        // Get snapshot for realtime clock.
        myCurrentTime= Time.realtimeSinceStartup;	    
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_Behaviour editor.
    public new void OnDisable() {
        base.OnDisable();
        
        // Release all worker objects.
        myGraphics   = null;
        myDynamicMenu= null;
    }

	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(IStorage == null) {
            myDisplayRoot= null;
            myBookmark= null;
            DragType= DragTypeEnum.None;
		    return false;
		}
        if(IStorage != myPreviousIStorage) {
            myPreviousIStorage= IStorage;
            myDisplayRoot= StorageRoot;
            myBookmark= null;
            DragType= DragTypeEnum.None;
            return false;            
        }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(IStorage);
			return false;
		}
        return true;
	}

    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	public void Update() {
        // Update storage selection.
        UpdateMgr();
        if(!IsInitialized()) return;
        // Determine repaint rate.
        if(IStorage != null) {
            // Repaint window
            if(IStorage.IsDirty || IStorage.IsAnimationPlaying || myAnimatedScrollPosition.IsActive || myAnimatedScale.IsActive) {
                IStorage.IsAnimationPlaying= false;
                MyWindow.Repaint();
            }
            float refreshFactor= (Application.isPlaying || EditorWindow.mouseOverWindow == MyWindow ? 8f : 1f);
            int newRefreshCounter= (int)(Time.realtimeSinceStartup*refreshFactor);
            if(newRefreshCounter != myRefreshCounter) {
                myRefreshCounter= newRefreshCounter;
                MyWindow.Repaint();
            }
            // Update DisplayRoot
            if(myDisplayRoot == null && IStorage.IsValid(0)) {
                myDisplayRoot= IStorage[0];
            }
        }
        // Cleanup objects.
        iCS_AutoReleasePool.Update();
	}
	
	// ----------------------------------------------------------------------
	// User GUI function.
//    static int frameCount= 0;
//    static int seconds= 0;
	public override void OnGUI() {
        // Show that we can display because we don't have a behavior or library.
        UpdateMgr();
        if(IStorage == null) {
            MyWindow.ShowNotification(new GUIContent("No iCanScript component selected !!!"));
            return;
        } else {
            MyWindow.RemoveNotification();
        }
		// Don't do start editor if not properly initialized.
		if( !IsInitialized() ) return;
       	
        // Update GUI time.
        myDeltaTime= Time.realtimeSinceStartup-myCurrentTime;
        myCurrentTime= Time.realtimeSinceStartup;
        
//        ++frameCount;
//       	int newTime= (int)Time.realtimeSinceStartup;
//       	if(newTime != seconds) {
//       	    seconds= newTime;
//       	    Debug.Log("GUI calls/seconds: "+frameCount);
//            frameCount= 0;
//       	}
       	
        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
		// Update mouse info.
		UpdateMouse();
		
        // Draw Graph.
        DrawGraph();

        // Process user inputs
        ProcessEvents();

		// Process scroll zone.
		ProcessScrollZone();
	}

    // ======================================================================
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*iCS_Config.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*iCS_Config.EditorWindowGutterSize+iCS_Config.EditorWindowToolbarHeight;
    }
    

#region User Interaction    
    // ======================================================================
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
                Rect nodePos= IStorage.GetPosition(node);
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
        Vector2 delta= MousePosition - MouseDragStartPosition;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                iCS_EditorObject node= DragObject;
                IStorage.MoveTo(node, DragStartPosition+delta);
                IStorage.SetDirty(node);                        
                node.IsFloating= IsFloatingKeyDown;
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
                Vector2 mousePosInGraph= ViewportToGraph(MousePosition);
                iCS_EditorObject closestPort= IStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Rect closestPortRect= IStorage.GetPosition(closestPort);
                    Vector2 closestPortPos= new Vector2(closestPortRect.x, closestPortRect.y);
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < 4f*iCS_Config.PortRadius) {
                        Rect parentPos= IStorage.GetPosition(IStorage.GetParent(DragObject));
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
		ProcessDrag();
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= IStorage.GetParent(node);
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(node);
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
                        Rect dragObjRect= IStorage.GetPosition(DragObject);
                        Vector2 dragObjPos= new Vector2(dragObjRect.x, dragObjRect.y);
                        iCS_EditorObject newState= GetStateAt(dragObjPos);
                        iCS_EditorObject newStateChart= null;
                        if(newState != null) {
                            newStateChart= IStorage.GetParent(newState);
                            while(newStateChart != null && !newStateChart.IsStateChart) {
                                newStateChart= IStorage.GetParent(newStateChart);
                            }
                        }
                        // Reset port drag if the port is on the same state.
                        if(origState == newState) {
                            DragObject.LocalPosition.x= DragStartPosition.x;
                            DragObject.LocalPosition.y= DragStartPosition.y;
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
                        iCS_EditorObject otherStatePort= DragObject.IsInputPort ? IStorage.GetOutStatePort(transitionModule) : IStorage.GetInStatePort(transitionModule);
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
                            Rect dragPortPos= IStorage.GetPosition(DragObject);
                            IStorage.DestroyInstance(DragObject);
                            // Verify for disconnection.
                            if(!isNearParent) {
                                // Let's determine if we want to create a module port.
                                iCS_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent == null) break;
                                if(newPortParent.IsModule) {
                                    iCS_EditorObject portParent= IStorage.GetParent(DragFixPort);
                                    Rect modulePos= IStorage.GetPosition(newPortParent);
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
#endregion User Interaction
    
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
        Rect portPos= IStorage.GetPosition(DragOriginalPort);
        IStorage.SetInitialPosition(DragObject, new Vector2(portPos.x, portPos.y));
        IStorage.SetDisplayPosition(DragObject, portPos);
		Rect parentPos= IStorage.GetPosition(parent);
		// Reset initial position if port is being dettached from it original parent.
		if(DragOriginalPort.IsInMuxPort) {
			DragStartPosition= Math3D.ToVector2(portPos)-Math3D.ToVector2(parentPos);			
		}
        DragObject.IsFloating= true;		
	}
	
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
        ShowClassWizard();
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(MousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the node at the given mouse position.
    iCS_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(MousePosition);
        return IStorage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the port at the given mouse position.
    iCS_EditorObject GetPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(MousePosition);
        return IStorage.GetPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the closest port at the given mouse position.
    iCS_EditorObject GetClosestPortAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(MousePosition);
        return IStorage.GetClosestPortAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= IStorage.GetPortAt(graphPosition);
        if(port != null) {
            if(IStorage.IsMinimized(port)) return IStorage.GetParent(port);
            return port;
        }
        iCS_EditorObject node= IStorage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
	// ----------------------------------------------------------------------
    void RotateSelectedMuxPort() {
		if(SelectedObject == null || !SelectedObject.IsDataPort) return;
		if(SelectedObject.IsOutMuxPort) {
			IStorage.ForEachChild(SelectedObject, 
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
		iCS_EditorObject muxPort= IStorage.GetParent(SelectedObject);
		if(!muxPort.IsDataPort) return;
		bool takeNext= false;
		bool found= IStorage.ForEachChild(muxPort,
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
    bool VerifyNewDragConnection(iCS_EditorObject fixPort, iCS_EditorObject dragPort) {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= IStorage.GetOverlappingPort(dragPort);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        // Destroy drag port since it is not needed anymore.
        IStorage.DestroyInstance(dragPort);
        dragPort= null;
        return VerifyNewConnection(fixPort, overlappingPort);
    }
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject overlappingPort) {
        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        iCS_EditorObject portParent= IStorage.GetParent(fixPort);
        iCS_EditorObject overlappingPortParent= IStorage.GetParent(overlappingPort);
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
            iCS_ReflectionDesc conversion= null;
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
    bool VerifyConnectionTypes(iCS_EditorObject inPort, iCS_EditorObject outPort, out iCS_ReflectionDesc typeCast) {
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
        iCS_ReflectionDesc conversion= null;
        if(!VerifyConnectionTypes(stateMuxPort, fixPort, out conversion)) return;
		var source= IStorage.GetSource(stateMuxPort);
		// Simply connect a disconnected mux state port.
		if(source == null && IStorage.NbOfChildren(stateMuxPort, c=> c.IsDataPort) == 0) {
			SetNewDataConnection(stateMuxPort, fixPort, conversion);
			return;
		}
		// Convert source port to child port.
		if(source != null) {
			stateMuxPort.ObjectType= iCS_ObjectTypeEnum.OutMuxPort;
			var firstMuxInput= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
			IStorage.SetSource(firstMuxInput, source);
			IStorage.SetSource(stateMuxPort, null);
		}
		// Create new mux input port.
		var inMuxPort= IStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
		SetNewDataConnection(inMuxPort, fixPort, conversion);
	}
	// ----------------------------------------------------------------------
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionDesc conversion= null) {
		iCS_EditorObject inNode= IStorage.GetParent(inPort);
        iCS_EditorObject outNode= IStorage.GetParent(outPort);
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
            SetNewDataConnection(inPort, newPort);
            IStorage.OptimizeDataConnection(inPort, outPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        IStorage.SetSource(inPort, outPort, conversion);
        IStorage.OptimizeDataConnection(inPort, outPort);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsState) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= IStorage.GetNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= IStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= IStorage.GetParent(edObj);
        for(; parentModule != null && !parentModule.IsModule; parentModule= IStorage.GetParent(parentModule));
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentNode(iCS_EditorObject edObj) {
        iCS_EditorObject parentNode= IStorage.GetParent(edObj);
        for(; parentNode != null && !parentNode.IsNode; parentNode= IStorage.GetParent(parentNode));
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
    iCS_EditorObject GetValidParentNodeUnder(iCS_EditorObject node) {
        if(!node.IsNode) return null;
        Vector2 point= Math3D.Middle(IStorage.GetPosition(node));
        iCS_EditorObject newParent= IStorage.GetNodeAt(point, node);
        if(newParent == IStorage.GetParent(node)) return newParent;
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(node.Name, node.ObjectType, newParent, IStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= IStorage.GetParent(node);
        if(newParent == null || newParent == oldParent) return;
        IStorage.SetParent(node, newParent);
		if(node.IsState) CleanupEntryState(node, oldParent);
        CleanupConnections(node);
    }
	// ----------------------------------------------------------------------
	void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= IStorage.GetParent(state);
		bool anEntryExists= false;
		IStorage.ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		IStorage.ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			IStorage.ForEachChild(prevParent,
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
                                transitionModule= IStorage.GetParent(IStorage.GetSource(p));
                            } else {
                                iCS_EditorObject[] connectedPorts= IStorage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= IStorage.GetParent(cp);
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= IStorage.GetParent(IStorage.GetOutStatePort(transitionModule));
                            iCS_EditorObject inState= IStorage.GetParent(IStorage.GetInStatePort(transitionModule));
                            iCS_EditorObject newParent= IStorage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != IStorage.GetParent(transitionModule)) {
                                ChangeParent(transitionModule, newParent);
                                IStorage.LayoutTransitionModule(transitionModule);
                                IStorage.SetDirty(IStorage.GetParent(node));
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
                break;
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
        iCS_EditorObject tmpPort= IStorage.GetSource(inPort);
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= IStorage.FindConnectedPorts(tmpPort);
            if(connected.Length == 1) {
                iCS_EditorObject t= IStorage.GetSource(tmpPort);
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
    void PasteIntoGraph(Vector2 point, iCS_Storage sourceStorage, iCS_EditorObject sourceRoot) {
        if(sourceRoot == null) return;
        iCS_EditorObject parent= GetValidParentNodeUnder(point, sourceRoot.ObjectType, sourceRoot.Name);
        if(parent == null) {
            EditorUtility.DisplayDialog("Operation Aborted", "Unable to find a suitable parent to paste into !!!", "Cancel");
            return;
        }
        iCS_EditorObject pasted= IStorage.CopyFrom(sourceRoot, new iCS_IStorage(sourceStorage), parent, point);
        if(IStorage.IsMaximized(pasted)) {
            IStorage.Fold(pasted);            
        }
    }

    // ======================================================================
    // Graph Navigation
	// ----------------------------------------------------------------------
    public void CenterOnRoot() {
        CenterOn(myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnRoot() {
        CenterAndScaleOn(myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOnSelected() {
        CenterOn(SelectedObject ?? myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOnSelected() {
        CenterAndScaleOn(SelectedObject ?? myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        CenterAt(Math3D.Middle(IStorage.GetPosition(obj)));
    }
	// ----------------------------------------------------------------------
    public void CenterAndScaleOn(iCS_EditorObject obj) {
        if(obj == null || IStorage == null) return;
        while(obj != null && !IStorage.IsVisible(obj)) obj= IStorage.GetParent(obj);
        if(obj == null) return;
        Rect objectArea= IStorage.GetPosition(obj);
        float newScale= 1.0f;
        if(obj.IsNode) {
            float widthScale= position.width/(1.1f*objectArea.width);
            float heightScale= position.height/(1.1f*objectArea.height);
            newScale= Mathf.Min(1.0f, Mathf.Min(widthScale, heightScale));
        }
        CenterAtWithScale(Math3D.Middle(objectArea), newScale);
    }
	// ----------------------------------------------------------------------
    public void CenterAt(Vector2 point) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/Scale*new Vector2(position.width, position.height);
        float distance= Vector2.Distance(ScrollPosition, newScrollPosition);
        float deltaTime= distance/3500f;
        if(deltaTime < IStorage.Preferences.ControlOptions.AnimationTime) deltaTime= IStorage.Preferences.ControlOptions.AnimationTime;
        if(deltaTime > 0.5f) deltaTime= 0.5f+(0.5f*(deltaTime-0.5f));
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
    }
	// ----------------------------------------------------------------------
    public void CenterAtWithScale(Vector2 point, float newScale) {
        if(IStorage == null) return;
        Vector2 newScrollPosition= point-0.5f/newScale*new Vector2(position.width, position.height);
        float distance= Vector2.Distance(ScrollPosition, newScrollPosition);
        float deltaTime= distance/3500f;
        if(deltaTime < IStorage.Preferences.ControlOptions.AnimationTime) deltaTime= IStorage.Preferences.ControlOptions.AnimationTime;
        if(deltaTime > 0.5f) deltaTime= 0.5f+(0.5f*(deltaTime-0.5f));
        myAnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
        myAnimatedScale.Start(Scale, newScale, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        Scale= newScale;
    }
    // ======================================================================
    // NODE GRAPH DISPLAY
	// ----------------------------------------------------------------------
    void DrawGrid() {
        myGraphics.DrawGrid(position,
                            IStorage.Preferences.Grid.BackgroundColor,
                            IStorage.Preferences.Grid.GridColor,
                            IStorage.Preferences.Grid.GridSpacing);
    }                       
    
	// ----------------------------------------------------------------------
	void Heading() {
		// Build standard toolbar at top of editor window.
		Rect r= iCS_ToolbarUtility.BuildToolbar(position.width, -1f);

		// Insert an initial spacer.
		float spacer= 8f;
		r.x+= spacer;
		r.width-= spacer;
		
//        // Adjust toolbar styles
//		Vector2 test= EditorStyles.toolbar.contentOffset;
//		test.y=3f;
//		EditorStyles.toolbar.contentOffset= test;
//		test= EditorStyles.toolbarTextField.contentOffset;
//		test.y=2f;
//		EditorStyles.toolbarTextField.contentOffset= test;
		
        // Show mouse coordinates.
        string mouseValue= ViewportToGraph(MousePosition).ToString();
        iCS_ToolbarUtility.Label(ref r, new GUIContent(mouseValue), 0, 0, true);
        
		// Show zoom control at the end of the toolbar.
        Scale= iCS_ToolbarUtility.Slider(ref r, 120f, Scale, 2f, 0.15f, spacer, spacer, true);
        iCS_ToolbarUtility.Label(ref r, new GUIContent("Zoom"), 0, 0, true);
		
		// Show current bookmark.
		string bookmarkString= "myBookmark: ";
		if(myBookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= myBookmark.Name;
		}
		iCS_ToolbarUtility.Label(ref r, 150f, new GUIContent(bookmarkString),0,0,true);
	}
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        IStorage.Update();

		// Start graphics
        myGraphics.Begin(UpdateScrollPosition(), UpdateScale(), ClipingArea, SelectedObject, ViewportToGraph(MousePosition), IStorage);
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw nodes and their connections.
    	DrawNormalNodes();
        DrawConnections();
        DrawMinimizedNodes();           

        myGraphics.End();

        // Show scroll zone (is applicable).
        if(IsDragStarted) DrawScrollZone();

		// Show header
		Heading();
	}

	// ----------------------------------------------------------------------
	Vector2 UpdateScrollPosition() {
        Vector2 graphicScrollPosition= ScrollPosition;
        if(myAnimatedScrollPosition.IsActive) {
            myAnimatedScrollPosition.Update();
            graphicScrollPosition= myAnimatedScrollPosition.CurrentValue;
        }
		return graphicScrollPosition;
	}
	// ----------------------------------------------------------------------
    float UpdateScale() {
        float scale= Scale;
        if(myAnimatedScale.IsActive) {
            myAnimatedScale.Update();
            scale= myAnimatedScale.CurrentValue;
        }
        return scale;
    }
	// ----------------------------------------------------------------------
    void DrawNormalNodes() {
        // Display node starting from the root node.
        IStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating && !node.IsBehaviour) {
                	myGraphics.DrawNormalNode(node, IStorage);                        
                }
            }
        );
        IStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating && !node.IsBehaviour) {
                	myGraphics.DrawNormalNode(node, IStorage);                        
                }
            }
        );
    }	
	// ----------------------------------------------------------------------
    void DrawMinimizedNodes() {
        // Display node starting from the root node.
        IStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating) {
                	myGraphics.DrawMinimizedNode(node, IStorage);                        
                }
            }
        );
        IStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating) {
                	myGraphics.DrawMinimizedNode(node, IStorage);                        
                }
            }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        IStorage.ForEachChildRecursive(myDisplayRoot, port=> { if(port.IsPort) myGraphics.DrawConnection(port, IStorage); });

        // Display ports.
        IStorage.ForEachChildRecursive(myDisplayRoot, port=> { if(port.IsPort) myGraphics.DrawPort(port, IStorage); });
    }

    // ======================================================================
    // SCROLL ZONE
	// ----------------------------------------------------------------------
    void ProcessScrollZone() {
        // Compute the amount of scroll needed.
        var dir= CanScrollInDirection(DetectScrollZone());
        if(Math3D.IsZero(dir)) return;
        dir*= IStorage.Preferences.ControlOptions.EdgeScrollSpeed*myDeltaTime;

        // Adjust according to scroll zone.
        switch(DragType) {
            case DragTypeEnum.PortConnection:
            case DragTypeEnum.TransitionCreation: {
                MouseDragStartPosition-= dir;
                ScrollPosition= ScrollPosition+dir;
                ProcessDrag();
                break;
            }
            default: break;
        }
    }
	// ----------------------------------------------------------------------
    void DrawScrollZone() {
        var dir= CanScrollInDirection(DetectScrollZone());
        if(Math3D.IsZero(dir)) return;
        ShowScrollButton(dir);
    }
	// ----------------------------------------------------------------------
    bool IsInScrollZone() {
        return Math3D.IsNotZero(DetectScrollZone());
    }
	// ----------------------------------------------------------------------
    const float scrollButtonSize=24f;
    Vector2 DetectScrollZone() {
        Vector2 direction= Vector2.zero;
        float headerHeight= iCS_ToolbarUtility.GetHeight();
        Rect rect= new Rect(0,headerHeight,position.width,position.height-headerHeight);
        if(!rect.Contains(MousePosition)) return direction;
        if(position.width < 3f*scrollButtonSize || position.height < 3f*scrollButtonSize) return direction;
        if(MousePosition.x < scrollButtonSize) {
            direction.x= -(scrollButtonSize-MousePosition.x)/scrollButtonSize;
        }
        if(MousePosition.x > position.width-scrollButtonSize) {
            direction.x= (MousePosition.x-position.width+scrollButtonSize)/scrollButtonSize;
        }
        if(MousePosition.y < scrollButtonSize+headerHeight) {
            direction.y= -(scrollButtonSize+headerHeight-MousePosition.y)/scrollButtonSize;
        }
        if(MousePosition.y > position.height-scrollButtonSize) {
            direction.y= (MousePosition.y-position.height+scrollButtonSize)/scrollButtonSize;
        }
        return direction;        
    }
	// ----------------------------------------------------------------------
    Vector2 CanScrollInDirection(Vector2 dir) {
        Rect rootRect= IStorage.GetPosition(myDisplayRoot);
        var rootCenter= Math3D.Middle(rootRect);
        var topLeftCorner= ViewportToGraph(new Vector2(0, iCS_ToolbarUtility.GetHeight()));
        var bottomRightCorner= ViewportToGraph(new Vector2(position.width, position.height));
        if(Math3D.IsSmaller(dir.x, 0f)) {
            if(!rootRect.Contains(new Vector2(topLeftCorner.x, rootCenter.y))) {
                dir.x= 0f;
            }
        }
        if(Math3D.IsGreater(dir.x,0f)) {
            if(!rootRect.Contains(new Vector2(bottomRightCorner.x, rootCenter.y))) {
                dir.x= 0f;
            }
        }
        if(Math3D.IsSmaller(dir.y, 0f)) {
            if(!rootRect.Contains(new Vector2(rootCenter.x, topLeftCorner.y))) {
                dir.y= 0f;
            }
        }
        if(Math3D.IsGreater(dir.y,0f)) {
            if(!rootRect.Contains(new Vector2(rootCenter.x, bottomRightCorner.y))) {
                dir.y= 0f;
            }
        }
        return dir;
    }
	// ----------------------------------------------------------------------
    void ShowScrollButton(Vector2 direction) {
        if(Math3D.IsZero(direction)) return;
        float headerHeight= iCS_ToolbarUtility.GetHeight();
        Rect rect= new Rect(0,headerHeight,position.width,position.height-headerHeight);
        if(Math3D.IsSmaller(direction.x, 0f)) {
            rect= Math3D.Intersection(rect, new Rect(0, 0, scrollButtonSize, position.height-1f));            
        }
        if(Math3D.IsGreater(direction.x, 0f)) {
            rect= Math3D.Intersection(rect, new Rect(position.width-scrollButtonSize, 0, scrollButtonSize-2f, position.height-1f));            
        }
        if(Math3D.IsSmaller(direction.y, 0f)) {
            rect= Math3D.Intersection(rect, new Rect(0, headerHeight, position.width-2f, scrollButtonSize-1f));
        }
        if(Math3D.IsGreater(direction.y, 0f)) {
            rect= Math3D.Intersection(rect, new Rect(0, position.height-scrollButtonSize, position.width-2f, scrollButtonSize-1f));
        }
        Color backgroundColor= new Color(1f,1f,1f,0.06f);
        iCS_Graphics.DrawRect(rect, backgroundColor, backgroundColor);
        // Draw arrow head
        direction.Normalize();
        Vector3[] tv= new Vector3[4];
        tv[0]= 0.4f*scrollButtonSize * direction;            
        Quaternion q1= Quaternion.AngleAxis(90f, Vector3.forward);
        tv[1]= q1*tv[0];
        Quaternion q2= Quaternion.AngleAxis(270f, Vector3.forward);
        tv[2]= q2*tv[0];
        tv[3]= tv[0];
        var center= Math3D.Middle(rect);
        for(int i= 0; i < 4; ++i) {
            tv[i].x+= center.x-0.2f*scrollButtonSize*direction.x;
            tv[i].y+= center.y-0.2f*scrollButtonSize*direction.y;
        }
        Color arrowColor= new Color(1f,1f,1f,0.5f);
        Handles.DrawSolidRectangleWithOutline(tv, arrowColor, arrowColor);
    }
}
