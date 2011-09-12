using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the AP_Graph.
public class AP_Editor : EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    AP_RootNode     RootNode   = null;
	AP_Inspector    Inspector  = null;
    AP_Node         DisplayRoot= null;

    // ----------------------------------------------------------------------
    public  AP_Mouse           Mouse           = null;
    private AP_Graphics        Graphics        = null;
    public  AP_ScrollView      ScrollView      = null;
    
    // ----------------------------------------------------------------------
    bool    IsRootNodeSelected  { get { return SelectedObject is AP_RootNode; }}
    bool    IsNodeSelected      { get { return SelectedObject is AP_Node; }}
    bool    IsPortSelected      { get { return SelectedObject is AP_Port; }}
    
    // ----------------------------------------------------------------------
    AP_Object   DragObject          = null;
    Vector2     DragStartPosition   = Vector2.zero;
    bool        IsDragEnabled       = true;
    bool        IsDragging          { get { return DragObject != null; }}


    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    AP_Object SelectedObject {
        get { return mySelectedObject; }
        set { Inspector.SelectedObject= mySelectedObject= value; }
    }
    AP_Object mySelectedObject= null;

    AP_Graph Graph { get { return RootNode.Graph; }}
    
    // ======================================================================
    // INITIALIZATION
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Not that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	void OnEnable() {        
        Debug.Log("Editor.OnEnable()");
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        Mouse           = new AP_Mouse(this);
        Graphics        = new AP_Graphics();
        ScrollView      = new AP_ScrollView();
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the AP_Graph editor.
    void OnDisable() {
        Debug.Log("Editor.OnDisable()");
        // Release all worker objects.
        Mouse           = null;
        Graphics        = null;
        ScrollView      = null;
    }
    void OnDestroy() {
        Debug.Log("Editor.OnDestroy()");
    }
    
    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(AP_RootNode rootNode, AP_Inspector _inspector) {
        RootNode= rootNode;
        DisplayRoot= rootNode;
        Inspector= _inspector;
    }
    
    // ----------------------------------------------------------------------
    public void Deactivate() {
        Inspector  = null;
		DisplayRoot= null;
		RootNode   = null;
    }

	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	bool ShouldRun() {
        // Nothing to do if we don't have a Graph to edit...
        if(DisplayRoot == null) return false;
        
		// Don't run if graphic sub-system did not initialise.
		if(Graphics.IsInitialized == false) {
            Graphics.Init();
			return false;
		}
		
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	void Update() {
		// Force a repaint to allow for snappy controls.
		Repaint();
	}
	
	// ----------------------------------------------------------------------
	// User GUI function.
	void OnGUI() {
		// Don't do start editor if not properly initialized.
		if( !ShouldRun() ) return;
       	
        // Load Editor Skin.
        GUI.skin= EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        
        // Update scroll view.
        ScrollView.Update(position, DisplayRoot.Position);
        
        // Draw editor grid.
        DrawGrid();
        
		// Draw editor widgets.
		DrawEditorWidgets();
		
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
	// Draws all editor widgets
	void DrawEditorWidgets() {
        DrawEditorToolbar();
	}

	// ----------------------------------------------------------------------
	void DrawEditorToolbar() {
//    	GUILayout.BeginHorizontal(EditorStyles.toolbar);
//    	
//        // Display root node selection.
//        string selected= SelectedObject != null ? SelectedObject.Name : "(No Object Selected)";
//        EditorGUILayout.TextField("Selected Node= ", selected);
//        
//		// Show display depth configuration.
//    	EditorGUILayout.Separator();
//		
//		GUILayout.EndHorizontal();
	}
    
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*AP_EditorConfig.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*AP_EditorConfig.EditorWindowGutterSize+AP_EditorConfig.EditorWindowToolbarHeight;
    }
    

#region User Interaction
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    public enum UserCommandStateEnum { Idle, Dragging, LeftButtonMenu, RightButtonMenu };
    public UserCommandStateEnum UserCommandState= UserCommandStateEnum.Idle;
    AP_Mouse.ButtonStateEnum   PreviousLeftButtonState= AP_Mouse.ButtonStateEnum.Idle;    
    public void ProcessEvents() {
        // Update the inspector object.
        DetermineSelectedObject();

        // Process left button state.
        switch(Mouse.LeftButtonState) {
            case AP_Mouse.ButtonStateEnum.Idle:
                if(PreviousLeftButtonState == AP_Mouse.ButtonStateEnum.Dragging) EndDragging();
                break;
            case AP_Mouse.ButtonStateEnum.SingleClick:
                break;
            case AP_Mouse.ButtonStateEnum.DoubleClick:
                ProcessMainMenu(Mouse.LeftButtonDownPosition);
                break;
            case AP_Mouse.ButtonStateEnum.Dragging:
                ProcessDragging();
                break;
        }
        PreviousLeftButtonState= Mouse.LeftButtonState;

        // Process right button state.
        switch(Mouse.RightButtonState) {
            case AP_Mouse.ButtonStateEnum.SingleClick:
                ProcessMainMenu(Mouse.RightButtonDownPosition);
                break;
        }
    }
    
	// ----------------------------------------------------------------------
    void ProcessMainMenu(Vector2 position) {
        AP_Object selectedObject= GetObjectAtScreenPosition(position);
        if(selectedObject == null) return;
        AP_MenuContext context= AP_MenuContext.CreateInstance(selectedObject, position, ScrollView.ScreenToGraph(position));
        string menuName= "CONTEXT/"+AP_EditorConfig.ProductName;
        if(selectedObject is AP_RootNode) menuName+= "/RootNode";
        else if(selectedObject is AP_StateChart) menuName+= "/StateChart";
        else if(selectedObject is AP_State) menuName+= "/State";
        else if(selectedObject is AP_Module) menuName+= "/Module";
        else if(selectedObject is AP_Function) menuName+= "/Function";
        EditorUtility.DisplayPopupMenu (new Rect (position.x,position.y,0,0), menuName, new MenuCommand(context));
    }
    
	// ----------------------------------------------------------------------
    void ProcessDragging() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Process dragging start.
        AP_Port port;
        AP_Node node;
        Vector2 MousePosition= ScrollView.ScreenToGraph(Mouse.Position);
        if(DragObject == null) {
            Vector2 pos= ScrollView.ScreenToGraph(Mouse.LeftButtonDownPosition);
            port= RootNode.GetPortAt(pos);
            if(port != null) {
                DragObject= port;
                DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
                port.IsBeingDragged= true;
            }
            else {
                node= RootNode.GetNodeAt(pos);                
                if(node != null) {
                    DragObject= node;
                    DragStartPosition= new Vector2(node.Position.x, node.Position.y);                                                    
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
        port= DragObject as AP_Port;
        if(port != null) {
            port.LocalPosition= DragStartPosition+delta;
            if(!port.IsNearParent()) {
            /*
                TODO : create a temporary port to show new connection.
            */    
            }
        }
        node= DragObject as AP_Node;
        if(node != null) {
            node.MoveTo(DragStartPosition+delta);                        
        }
    }    

	// ----------------------------------------------------------------------
    void EndDragging() {
        AP_Port port= DragObject as AP_Port;
        if(port != null) {
            port.IsBeingDragged= false;
            // Verify for a new connection.
            if(!VerifyNewConnection(port)) {
                // Verify for disconnection.
                if(!port.IsNearParent()) {
                    if(port is AP_DataPort) {
                        (port as AP_DataPort).Disconnect();                        
                    }
                    port.LocalPosition= DragStartPosition;
                }                    
                else {
                    // Assume port relocation.
                    port.SnapToParent();
                    port.Parent.Layout();                    
                }
            }
        }
    
        // Reset dragging state.
        DragObject= null;
        IsDragEnabled= true;
    }
#endregion User Interaction
    
	// ----------------------------------------------------------------------
    // Manages the object selection.
    AP_Object DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        if(!Mouse.IsLeftButtonDown && !Mouse.IsRightButtonDown) return SelectedObject;
        AP_Object newSelected= GetObjectAtMousePosition();
        SelectedObject= newSelected;
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public AP_Object GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(Mouse.Position);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public AP_Object GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
        AP_Port port= RootNode.GetPortAt(graphPosition);
        if(port != null) return port;
        AP_Node node= RootNode.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(AP_Port port) {
        // No new connection if no overlapping port found.
        AP_Port overlappingPort= port.GetOverlappingPort();
        if(overlappingPort == null) return false;
        
        // Only connect data ports.
        if(!(port is AP_DataPort)) return false;
        if(!(overlappingPort is AP_DataPort)) return false;
        AP_DataPort dataPort= port as AP_DataPort;
        AP_DataPort overlappingDataPort= overlappingPort as AP_DataPort;
        
        // We have a new connection so lets determine direction.
        dataPort.LocalPosition= DragStartPosition;
        if(dataPort.IsInput) {
            Debug.Log("5");
            if(overlappingDataPort.IsOutput) {
                dataPort.Source= overlappingDataPort;
                return true;
            }
            if(dataPort.IsVirtual == false && overlappingDataPort.IsVirtual == false) {
                Debug.Log("8");
                return true;
            }
            if(dataPort.IsVirtual == true && overlappingDataPort.IsVirtual == false) {
                overlappingDataPort.Source= dataPort;
                return true;
            }
            if(dataPort.IsVirtual == false && overlappingDataPort.IsVirtual == true) {
                dataPort.Source= overlappingDataPort;
                return true;
            }
            dataPort.Source= overlappingDataPort;
        }
        else {
            if(overlappingDataPort.IsInput) {
                overlappingDataPort.Source= dataPort;
                return true;
            }            
            if(dataPort.IsVirtual == false && overlappingDataPort.IsVirtual == false) {
                return true;
            }
            if(dataPort.IsVirtual == true && overlappingDataPort.IsVirtual == false) {
                dataPort.Source= overlappingDataPort;
                return true;
            }
            if(dataPort.IsVirtual == false && overlappingDataPort.IsVirtual == true) {
                overlappingDataPort.Source= dataPort;
                return true;
            }
            dataPort.Source= overlappingDataPort;
        }
        return true;
    }
    
    
    // ======================================================================
    // NODE GRAPH DISPLAY
	// ----------------------------------------------------------------------
    void DrawGrid() {
        Graphics.DrawGrid(position,
                          Graph.Preferences.Grid.BackgroundColor,
                          Graph.Preferences.Grid.GridColor,
                          Graph.Preferences.Grid.GridSpacing,
                          ScrollView.ScreenToGraph(Vector2.zero));
    }
    
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Perform layout of modified nodes.
        DisplayRoot.ForEachRecursiveDepthLast( (obj)=> { if(obj.IsEditorDirty) obj.Layout(); } );            
        
        // Draw editor window.
        ScrollView.Begin();
    	DrawNodes();
        DrawConnections();            
        ScrollView.End();
	}

	// ----------------------------------------------------------------------
    void DrawNodes() {
        // Display node starting from the root node.
        DisplayRoot.ForEachRecursiveDepthLast<AP_Node>( (node)=> { Graphics.DrawNode(node, SelectedObject); } );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        DisplayRoot.ForEachRecursive<AP_Port>( (port)=> { Graphics.DrawConnection(port, SelectedObject); } );

        // Display ports.
        DisplayRoot.ForEachRecursive<AP_Port>( (port)=> { Graphics.DrawPort(port, SelectedObject); } );
    }

}
