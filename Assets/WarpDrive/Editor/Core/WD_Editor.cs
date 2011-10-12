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
    WD_Storage          Storage        = null;
	WD_Inspector        Inspector      = null;
    WD_EditorObject     DisplayRoot    = null;
    WD_DynamicMenu      DynamicMenu    = null;

    // ----------------------------------------------------------------------
    public  WD_Mouse           Mouse           = null;
    private WD_Graphics        Graphics        = null;
    public  WD_ScrollView      ScrollView      = null;
    
    // ----------------------------------------------------------------------
    WD_EditorObject DragObject          = null;
    Vector2         DragStartPosition   = Vector2.zero;
    bool            IsDragEnabled       = true;
    bool            IsDragging          { get { return DragObject != null; }}

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
	public void Activate(WD_Storage storage, WD_Inspector inspector) {
        Storage= storage;
        Inspector= inspector;
        DisplayRoot= null;
        // Assure that the editor data has been properly generated.
        Storage.GenerateEditorData();
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
            WD_Graphics.Init();
			return false;
		}
		
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
    static int refreshCnt= 0;
	void Update() {
		// Force a repaint to allow for snappy controls.
        if((++refreshCnt & 7) == 0) {
    		Repaint();            
        }
	}
	
	// ----------------------------------------------------------------------
	// User GUI function.
	void OnGUI() {
		// Don't do start editor if not properly initialized.
		if( !IsInitialized() ) return;
       	
        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
        // Update scroll view.
        Rect scrollViewPosition= DisplayRoot != null ? Storage.EditorObjects.GetPosition(DisplayRoot) : new Rect(0,0,500,500);
        ScrollView.Update(position, scrollViewPosition);
        
		// Draw editor widgets.
		DrawEditorWidgets();
		
        // Draw Graph.
        DrawGraph();

		// Compute new EMouse position.
		Mouse.Update();

        // Process user inputs
        ProcessEvents();
        
        // Process new accumulated commands.
        if(Storage.EditorObjects.IsDirty) {
            Storage.EditorObjects.IsDirty= false;
            Undo.RegisterUndo(Storage, "WarpDrive");
            EditorUtility.SetDirty(Storage);
        }
	}

    // ======================================================================
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
	// Draws all editor widgets
	void DrawEditorWidgets() {
        DrawEditorToolbar();
	}

	// ----------------------------------------------------------------------
	void DrawEditorToolbar() {
	}
    
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
        if(!DynamicMenu.IsActive) {
            // Update the inspector object.
            DetermineSelectedObject();

            // Process left button state.
            switch(Mouse.LeftButtonState) {
                case WD_Mouse.ButtonStateEnum.Idle:
                    if(PreviousLeftButtonState == WD_Mouse.ButtonStateEnum.Dragging) EndDragging();
                    break;
                case WD_Mouse.ButtonStateEnum.SingleClick:
                    break;
                case WD_Mouse.ButtonStateEnum.DoubleClick:
                    DynamicMenu.Update(SelectedObject, Storage, Mouse.LeftButtonDownPosition);
                    break;
                case WD_Mouse.ButtonStateEnum.Dragging:
                    ProcessDragging();
                    break;
            }
            PreviousLeftButtonState= Mouse.LeftButtonState;

            // Process right button state.
            switch(Mouse.RightButtonState) {
                case WD_Mouse.ButtonStateEnum.SingleClick:
                    DynamicMenu.Update(SelectedObject, Storage, Mouse.RightButtonDownPosition);
                    break;
            }                    
        }
    }
    
	// ----------------------------------------------------------------------
    void ProcessDragging() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Process dragging start.
        WD_EditorObject port;
        WD_EditorObject node;
        Vector2 MousePosition= ScrollView.ScreenToGraph(Mouse.Position);
        if(DragObject == null) {
            Vector2 pos= ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);
            port= Storage.EditorObjects.GetPortAt(pos);
            if(port != null) {
                DragObject= port;
                DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
                port.IsBeingDragged= true;
            }
            else {
                node= Storage.EditorObjects.GetNodeAt(pos);                
                if(node != null) {
                    DragObject= node;
                    Rect position= Storage.EditorObjects.GetPosition(node);
                    DragStartPosition= new Vector2(position.x, position.y);                                                    
                }
                else {
                    // Disable dragging since mouse is not over Node or Port.
                    IsDragEnabled= false;
                    DragObject= null;
                    return;
                }
            }
        }

        // Compute new object position.
        Vector2 delta= MousePosition - ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);
        if(DragObject.IsPort) {
            port= DragObject;
            Vector2 newLocalPos= DragStartPosition+delta;
            port.LocalPosition.x= newLocalPos.x;
            port.LocalPosition.y= newLocalPos.y;
            port.IsDirty= true;
            if(!Storage.EditorObjects.IsNearParent(port)) {
            /*
                TODO : create a temporary port to show new connection.
            */    
            }
        }
        if(DragObject.IsNode) {
            node= DragObject;
            Storage.EditorObjects.MoveTo(node, DragStartPosition+delta);
            node.IsDirty= true;                        
        }
    }    

	// ----------------------------------------------------------------------
    void EndDragging() {
        if(DragObject != null && DragObject.IsPort) {
            WD_EditorObject port= DragObject;
            port.IsBeingDragged= false;
            // Verify for a new connection.
            if(!VerifyNewConnection(port)) {
                // Verify for disconnection.
                if(!Storage.EditorObjects.IsNearParent(port)) {
                    if(port.IsRuntimeA<WD_FieldPort>()) {
//                        (Storage.EditorObjects.GetRuntimeObject(port) as WD_FieldPort).Disconnect();
                    }
                    port.LocalPosition.x= DragStartPosition.x;
                    port.LocalPosition.y= DragStartPosition.y;
                }                    
                else {
                    // Assume port relocation.
                    Storage.EditorObjects.SnapToParent(port);
                    Storage.EditorObjects.Layout(Storage.EditorObjects[port.ParentId]);
                }
            }
            port.IsDirty= true;
        }
    
        // Reset dragging state.
        DragObject= null;
        IsDragEnabled= true;
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
    public WD_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
        WD_EditorObject port= Storage.EditorObjects.GetPortAt(graphPosition);
        if(port != null) return port;
        WD_EditorObject node= Storage.EditorObjects.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(WD_EditorObject port) {
        // No new connection if no overlapping port found.
        WD_EditorObject overlappingPort= Storage.EditorObjects.GetOverlappingPort(port);
        if(overlappingPort == null) return false;
        
        // Connect function & modules ports together.
        if(port.IsDataPort && overlappingPort.IsDataPort) {            
            WD_EditorObject inPort = port.IsInputPort             ? port : overlappingPort;
            WD_EditorObject outPort= overlappingPort.IsOutputPort ? overlappingPort : port;
            if(inPort != outPort) {
                if(inPort.RuntimeType == outPort.RuntimeType) { // No conversion needed.
                    Storage.EditorObjects.SetSource(inPort, outPort);                       
                }
                else {  // A conversion is required.
                    WD_ConversionDesc conversion= WD_DataBase.FindConversion(outPort.RuntimeType, inPort.RuntimeType);
                    if(conversion == null) {
                        Debug.LogWarning("No direct conversion exists from "+outPort.RuntimeType.Name+" to "+inPort.RuntimeType.Name);
                    } else {
                        Storage.EditorObjects.SetSource(inPort, outPort, conversion);
                    }
                }
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
        // Perform layout of modified nodes.
        Storage.EditorObjects.ForEachRecursiveDepthLast(DisplayRoot,
            (obj)=> {
                if(obj.IsDirty) {
                    Storage.EditorObjects.Layout(obj);
                }
            }
        );            
        
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
        Storage.EditorObjects.ForEachRecursiveDepthLast(DisplayRoot,
            (node)=> {
                if(node.IsNode) Graphics.DrawNode(node, SelectedObject, Storage);
            }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        Storage.EditorObjects.ForEachChildRecursive(DisplayRoot, (port)=> { if(port.IsPort) Graphics.DrawConnection(port, SelectedObject, Storage); } );

        // Display ports.
        Storage.EditorObjects.ForEachChildRecursive(DisplayRoot, (port)=> { if(port.IsPort) Graphics.DrawPort(port, SelectedObject, Storage); } );
    }

}
