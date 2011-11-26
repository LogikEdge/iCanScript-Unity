using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the WD_Behaviour.
public class WD_Editor : EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    WD_IStorage         Storage        = null;
	WD_Inspector        Inspector      = null;
    WD_EditorObject     DisplayRoot    = null;
    WD_DynamicMenu      DynamicMenu    = null;

    // ----------------------------------------------------------------------
    public  WD_Mouse           Mouse           = null;
    private WD_Graphics        Graphics        = null;
    public  WD_ScrollView      ScrollView      = null;
    
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortDrag, NodeDrag, TransitionCreation };
    DragTypeEnum    DragType            = DragTypeEnum.None;
    WD_EditorObject DragObject          = null;
    Vector2         DragStartPosition   = Vector2.zero;
    bool            IsDragEnabled       = true;
    bool            IsDragStarted       { get { return DragObject != null; }}

    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    WD_EditorObject SelectedObject {
        get { return mySelectedObject; }
        set { Inspector.SelectedObject= mySelectedObject= value; }
    }
    WD_EditorObject mySelectedObject= null;

    // ======================================================================
    // INITIALIZATION
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Not that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	void OnEnable() {        
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        Mouse           = new WD_Mouse(this);
        Graphics        = new WD_Graphics();
        ScrollView      = new WD_ScrollView();
        DynamicMenu     = new WD_DynamicMenu();
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the WD_Behaviour editor.
    void OnDisable() {
        // Release all worker objects.
        Mouse       = null;
        Graphics    = null;
        ScrollView  = null;
        DynamicMenu = null;
    }
    
    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(WD_IStorage storage, WD_Inspector inspector) {
        Storage= storage;
        Inspector= inspector;
        DisplayRoot= null;
    }
    
    // ----------------------------------------------------------------------
    public void Deactivate() {
        Inspector      = null;
		DisplayRoot    = null;
		Storage        = null;
    }

	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	public bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(Storage == null || Inspector == null) { return false; }
        
		// Don't run if graphic sub-system did not initialise.
		if(WD_Graphics.IsInitialized == false) {
            WD_Graphics.Init(Storage);
			return false;
		}
		
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	void Update() {
        if(Storage != null && Storage.IsDirty) {
//            Debug.Log("Repaint needed");
            Repaint();
        }
	}
	
	// ----------------------------------------------------------------------
    void OnSelectionChanged() {
        Update();
    }
    
	// ----------------------------------------------------------------------
	// User GUI function.
	void OnGUI() {
		// Don't do start editor if not properly initialized.
		if( !IsInitialized() ) return;
       	
        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
        // Update scroll view.
        Rect scrollViewPosition= DisplayRoot != null ? Storage.GetPosition(DisplayRoot) : new Rect(0,0,500,500);
        ScrollView.Update(position, scrollViewPosition);
        
        // Draw Graph.
        DrawGraph();

		// Compute new EMouse position.
		Mouse.Update();

        // Process user inputs
        ProcessEvents();
	}

    // ======================================================================
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*WD_EditorConfig.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*WD_EditorConfig.EditorWindowGutterSize+WD_EditorConfig.EditorWindowToolbarHeight;
    }
    

#region User Interaction
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    public enum UserCommandStateEnum { Idle, Dragging, LeftButtonMenu, RightButtonMenu };
    public UserCommandStateEnum UserCommandState= UserCommandStateEnum.Idle;
    WD_Mouse.ButtonStateEnum   PreviousLeftButtonState= WD_Mouse.ButtonStateEnum.Idle;    
    public void ProcessEvents() {
        // Update the inspector object.
        DetermineSelectedObject();

        // Process left button state.
        switch(Mouse.LeftButtonState) {
            case WD_Mouse.ButtonStateEnum.Idle:
                if(PreviousLeftButtonState == WD_Mouse.ButtonStateEnum.Dragging) EndDrag();
                break;
            case WD_Mouse.ButtonStateEnum.SingleClick:
                if(SelectedObject != null) {
                    // Process fold/unfold click.
                    Vector2 graphMousePos= ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);
                    if(Graphics.IsFoldIconPicked(SelectedObject, graphMousePos, Storage)) {
                        if(Storage.IsFolded(SelectedObject)) {
                            Storage.RegisterUndo("Unfold");
                            Storage.Unfold(SelectedObject);
                        } else {
                            Storage.RegisterUndo("Fold");
                            Storage.Fold(SelectedObject);
                        }
                    }
                    // Process maximize/minimize click.
                    if(Graphics.IsMinimizeIconPicked(SelectedObject, graphMousePos, Storage)) {
                        Storage.RegisterUndo("Minimize");
                        Storage.Minimize(SelectedObject);
                    } else if(Graphics.IsMaximizeIconPicked(SelectedObject, graphMousePos, Storage)) {
                        Storage.RegisterUndo("Maximize");
                        Storage.Maximize(SelectedObject);
                    }
                }
                break;
            case WD_Mouse.ButtonStateEnum.DoubleClick:
                DynamicMenu.Update(SelectedObject, Storage, ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition));
                break;
            case WD_Mouse.ButtonStateEnum.Dragging:
                ProcessDrag();
                break;
        }
        PreviousLeftButtonState= Mouse.LeftButtonState;

        // Process right button state.
        switch(Mouse.RightButtonState) {
            case WD_Mouse.ButtonStateEnum.SingleClick:
                DynamicMenu.Update(SelectedObject, Storage, ScrollView.ScreenToGraph(Mouse.RightButtonDownPosition));
                break;
        }                    
    }
    
	// ----------------------------------------------------------------------
    void ProcessDrag() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Start a new drag (if not already started).
        if(!StartDrag()) return;

        // Compute new object position.
        Vector2 MousePosition= ScrollView.ScreenToGraph(Mouse.Position);
        Vector2 delta= MousePosition - ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                WD_EditorObject node= DragObject;
                Storage.MoveTo(node, DragStartPosition+delta);
                Storage.SetDirty(node);                        
                break;
            case DragTypeEnum.PortDrag:
            case DragTypeEnum.TransitionCreation:
                WD_EditorObject port= DragObject;
                Vector2 newLocalPos= DragStartPosition+delta;
                port.LocalPosition.x= newLocalPos.x;
                port.LocalPosition.y= newLocalPos.y;
                Storage.SetDirty(port);
                if(!Storage.IsNearParent(port)) {
                /*
                    TODO : create a temporary port to show new connection.
                */    
                }
                break;
        }
    }    
	// ----------------------------------------------------------------------
    bool StartDrag() {
        // Don't select new drag type if drag already started.
        if(IsDragStarted) return true;
        
        // Use the Left mouse down position has drag start position.
        Vector2 pos= ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);

        // Port drag.
        WD_EditorObject port= Storage.GetPortAt(pos);
        if(port != null && !Storage.IsMinimized(port)) {
            Storage.RegisterUndo("Port Drag");
            DragType= DragTypeEnum.PortDrag;
            DragObject= port;
            DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
            port.IsBeingDragged= true;
            return true;
        }

        // Node drag.
        WD_EditorObject node= Storage.GetNodeAt(pos);                
        if(node != null && (node.IsMinimized || !node.IsState || Graphics.IsNodeTitleBarPicked(node, pos, Storage))) {
            Storage.RegisterUndo("Node Drag");
            DragType= DragTypeEnum.NodeDrag;
            DragObject= node;
            Rect position= Storage.GetPosition(node);
            DragStartPosition= new Vector2(position.x, position.y);                                                    
            return true;
        }
        
        // New state transition drag.
        if(node != null && node.IsState) {
            Storage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            WD_EditorObject outTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), WD_ObjectTypeEnum.OutStatePort);
            WD_EditorObject inTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), WD_ObjectTypeEnum.InStatePort);
            Storage.SetInitialPosition(outTransition, pos);
            Storage.SetInitialPosition(inTransition, pos);
            inTransition.Source= outTransition.InstanceId;
            DragObject= inTransition;
            DragStartPosition= new Vector2(DragObject.LocalPosition.x, DragObject.LocalPosition.y);
            DragObject.IsBeingDragged= true;
            return true;
        }
        
        // Disable dragging since mouse is not over Node or Port.
        DragType= DragTypeEnum.None;
        DragObject= null;
        IsDragEnabled= false;
        return false;
    }
	// ----------------------------------------------------------------------
    void EndDrag() {
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: break;
                case DragTypeEnum.PortDrag:
                    WD_EditorObject port= DragObject;
                    port.IsBeingDragged= false;
                    // Verify for a new connection.
                    if(!VerifyNewConnection(port)) {
                        // Verify for disconnection.
                        Storage.SetDirty(port);
                        if(!Storage.IsNearParent(port)) {
                            if(port.IsDataPort) {
                                WD_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    WD_EditorObject portParent= Storage.GetParent(port);
                                    Rect portPos= Storage.GetPosition(port);
                                    Rect modulePos= Storage.GetPosition(newPortParent);
                                    float portSize2= 2f*WD_EditorConfig.PortSize;
                                    if(port.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!Storage.IsChildOf(newPortParent, portParent)) {
                                                WD_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, WD_ObjectTypeEnum.InDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(newPort, port);                               
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent)) {
                                                WD_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, WD_ObjectTypeEnum.OutDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(newPort, port);                               
                                                break;                                                
                                            }
                                        }                                    
                                    }
                                    if(port.IsInputPort) {
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(!Storage.IsChildOf(portParent, newPortParent)) {
                                                WD_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, WD_ObjectTypeEnum.OutDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(port, newPort);                               
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent) || Storage.IsChildOf(newPortParent, portParent)) {
                                                WD_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, WD_ObjectTypeEnum.InDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                if(Storage.IsChildOf(portParent, newPortParent)) {
                                                    Storage.SetSource(port, newPort);
                                                } else {
                                                    Storage.SetSource(newPort, port);
                                                }
                                                break;
                                            }
                                        }
                                    }                                    
                                }
                                port.LocalPosition.x= DragStartPosition.x;
                                port.LocalPosition.y= DragStartPosition.y;
                                Storage.DisconnectPort(port);                                    
                                break;
                            }
                            if(port.IsStatePort) {
                                if(EditorUtility.DisplayDialog("Deleting Transition", "Are you sure you want to remove the dragged transition.", "Delete", "Cancel")) {
                                    Storage.DestroyInstance(port);
                                } else {
                                    port.LocalPosition.x= DragStartPosition.x;
                                    port.LocalPosition.y= DragStartPosition.y;                                
                                }
                                break;
                            }
                        }                    
                    }
                    break;
                case DragTypeEnum.TransitionCreation:
                    WD_EditorObject destState= GetNodeAtMousePosition();
                    if(destState != null && destState.IsState) {
                        WD_EditorObject outStatePort= Storage[DragObject.Source];
                        outStatePort.IsBeingDragged= false;
                        Storage.CreateTransition(outStatePort, destState);
                        DragObject.Source= -1;
                        Storage.DestroyInstance(DragObject);
                    } else {
                        Storage.DestroyInstance(DragObject.Source);
                        Storage.DestroyInstance(DragObject);
                    }
                    break;
            }            
        }
    
        finally {
            // Reset dragging state.
            DragType= DragTypeEnum.None;
            DragObject= null;
            IsDragEnabled= true;            
        }
    }
#endregion User Interaction
    
	// ----------------------------------------------------------------------
    // Manages the object selection.
    WD_EditorObject DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        if(!Mouse.IsLeftButtonDown && !Mouse.IsRightButtonDown) return SelectedObject;
        WD_EditorObject newSelected= GetObjectAtMousePosition();
        SelectedObject= newSelected;
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public WD_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(Mouse.Position);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public WD_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ScrollView.ScreenToGraph(Mouse.Position);
        return Storage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public WD_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
        WD_EditorObject port= Storage.GetPortAt(graphPosition);
        if(port != null) {
            if(Storage.IsMinimized(port)) return Storage.GetParent(port);
            return port;
        }
        WD_EditorObject node= Storage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(WD_EditorObject port) {
        // No new connection if no overlapping port found.
        WD_EditorObject overlappingPort= Storage.GetOverlappingPort(port);
        if(overlappingPort == null) return false;
        
        // Reestablish port position.
        port.LocalPosition.x= DragStartPosition.x;
        port.LocalPosition.y= DragStartPosition.y;
        
        // Connect function & modules ports together.
        if(port.IsDataPort && overlappingPort.IsDataPort) {            
            WD_EditorObject inPort = null;
            WD_EditorObject outPort= null;

            WD_EditorObject portParent= Storage.EditorObjects[port.ParentId];
            WD_EditorObject overlappingPortParent= Storage.EditorObjects[overlappingPort.ParentId];
            bool portIsChildOfOverlapping= Storage.IsChildOf(portParent, overlappingPortParent);
            bool overlappingIsChildOfPort= Storage.IsChildOf(overlappingPortParent, portParent);
            if(portIsChildOfOverlapping || overlappingIsChildOfPort) {
                if(port.IsInputPort && overlappingPort.IsInputPort) {
                    if(portIsChildOfOverlapping) {
                        inPort= port;
                        outPort= overlappingPort;
                    } else {
                        inPort= overlappingPort;
                        outPort= port;
                    }
                } else if(port.IsOutputPort && overlappingPort.IsOutputPort) {
                    if(portIsChildOfOverlapping) {
                        inPort= overlappingPort;
                        outPort= port;
                    } else {
                        inPort= port;
                        outPort= overlappingPort;
                    }                    
                } else {
                    Debug.LogWarning("Cannot connect nested node ports from input to output !!!");
                    return true;
                }
            } else {
                inPort = port.IsInputPort             ? port : overlappingPort;
                outPort= overlappingPort.IsOutputPort ? overlappingPort : port;
            }
            if(inPort != outPort) {
                if(WD_Types.CanBeConnectedWithoutConversion(outPort.RuntimeType, inPort.RuntimeType)) { // No conversion needed.
                    Storage.SetSource(inPort, outPort);                       
                }
                else {  // A conversion is required.
                    WD_ReflectionDesc conversion= WD_DataBase.FindConversion(outPort.RuntimeType, inPort.RuntimeType);
                    if(conversion == null) {
                        Debug.LogWarning("No direct conversion exists from "+outPort.RuntimeType.Name+" to "+inPort.RuntimeType.Name);
                    } else {
                        Storage.SetSource(inPort, outPort, conversion);
                    }
                }
            } else {
                Debug.LogWarning("Ports are both either inputs or outputs !!!");
            }
            return true;
        }

        // Connect transition port together.
        if(port.IsStatePort && overlappingPort.IsStatePort) {
            return true;
        }
        
        Debug.LogWarning("Trying to connect incompatible port types: "+port.TypeName+"<=>"+overlappingPort.TypeName);
        return true;
    }
    
    // ======================================================================
    // NODE GRAPH DISPLAY
	// ----------------------------------------------------------------------
    void DrawGrid() {
        Graphics.DrawGrid(position,
                          Storage.Preferences.Grid.BackgroundColor,
                          Storage.Preferences.Grid.GridColor,
                          Storage.Preferences.Grid.GridSpacing,
                          ScrollView.ScreenToGraph(Vector2.zero));
    }
    
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        Storage.Update();
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw editor window.
        ScrollView.Begin();
    	DrawNodes();
        DrawConnections();            
        ScrollView.End();
	}

	// ----------------------------------------------------------------------
    void DrawNodes() {
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> { if(node.IsNode && !Storage.IsMinimized(node)) Graphics.DrawNode(node, SelectedObject, Storage); }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawConnection(port, Storage); });

        // Display ports.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawPort(port, SelectedObject, Storage); });

        // Display minimized nodes.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> { if(node.IsNode && Storage.IsMinimized(node)) Graphics.DrawNode(node, SelectedObject, Storage); }
        );
    }

}
