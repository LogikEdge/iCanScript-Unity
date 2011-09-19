using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the WD_Behaviour.
public class WD_Editor : EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    WD_RootNode     RootNode   = null;
	WD_Inspector    Inspector  = null;
    WD_Node         DisplayRoot= null;

    // ----------------------------------------------------------------------
    public  WD_Mouse           Mouse           = null;
    private WD_Behaviourics        Graphics        = null;
    public  WD_ScrollView      ScrollView      = null;
    
    // ----------------------------------------------------------------------
    bool    IsRootNodeSelected  { get { return SelectedObject is WD_RootNode; }}
    bool    IsNodeSelected      { get { return SelectedObject is WD_Node; }}
    bool    IsPortSelected      { get { return SelectedObject is WD_Port; }}
    
    // ----------------------------------------------------------------------
    WD_Object   DragObject          = null;
    Vector2     DragStartPosition   = Vector2.zero;
    bool        IsDragEnabled       = true;
    bool        IsDragging          { get { return DragObject != null; }}


    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    WD_Object SelectedObject {
        get { return mySelectedObject; }
        set { Inspector.SelectedObject= mySelectedObject= value; }
    }
    WD_Object mySelectedObject= null;

    WD_Behaviour Graph { get { return RootNode.Graph; }}
    
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
        Graphics        = new WD_Behaviourics();
        ScrollView      = new WD_ScrollView();
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the WD_Behaviour editor.
    void OnDisable() {
        // Release all worker objects.
        Mouse           = null;
        Graphics        = null;
        ScrollView      = null;
    }
    
    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(WD_RootNode rootNode, WD_Inspector _inspector) {
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
		if(WD_Behaviourics.IsInitialized == false) {
            WD_Behaviourics.Init();
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
       	
        // Take a snapshot of the command buffer size.
        int commandBufferSize= Graph.CommandBuffer.Count;
        
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
        
        // Process new accumulated commands.
        if(commandBufferSize != Graph.CommandBuffer.Count) {
            Debug.Log("Registering Undo");
            Graph.CommandBuffer.Compress();
            Undo.RegisterUndo(Graph, "WarpDrive");
            EditorUtility.SetDirty(Graph);
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
                if(PreviousLeftButtonState == WD_Mouse.ButtonStateEnum.Dragging) EndDragging();
                break;
            case WD_Mouse.ButtonStateEnum.SingleClick:
                break;
            case WD_Mouse.ButtonStateEnum.DoubleClick:
                ProcessMainMenu(Mouse.LeftButtonDownPosition);
                break;
            case WD_Mouse.ButtonStateEnum.Dragging:
                ProcessDragging();
                break;
        }
        PreviousLeftButtonState= Mouse.LeftButtonState;

        // Process right button state.
        switch(Mouse.RightButtonState) {
            case WD_Mouse.ButtonStateEnum.SingleClick:
                ProcessMainMenu(Mouse.RightButtonDownPosition);
                break;
        }        
    }
    
	// ----------------------------------------------------------------------
    void ProcessMainMenu(Vector2 position) {
        WD_Object selectedObject= GetObjectAtScreenPosition(position);
        if(selectedObject == null) return;
        WD_MenuContext context= WD_MenuContext.CreateInstance(selectedObject, position, ScrollView.ScreenToGraph(position));
        string menuName= "CONTEXT/"+WD_EditorConfig.ProductName;
        if(selectedObject is WD_RootNode) menuName+= "/RootNode";
        else if(selectedObject is WD_StateChart) menuName+= "/StateChart";
        else if(selectedObject is WD_State) menuName+= "/State";
        else if(selectedObject is WD_Module) menuName+= "/Module";
        else if(selectedObject is WD_Function) menuName+= "/Function";
        EditorUtility.DisplayPopupMenu (new Rect (position.x,position.y,0,0), menuName, new MenuCommand(context));
    }
    
	// ----------------------------------------------------------------------
    void ProcessDragging() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Process dragging start.
        WD_Port port;
        WD_Node node;
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
        port= DragObject as WD_Port;
        if(port != null) {
            port.LocalPosition= DragStartPosition+delta;
            if(!port.IsNearParent()) {
            /*
                TODO : create a temporary port to show new connection.
            */    
            }
        }
        node= DragObject as WD_Node;
        if(node != null) {
            node.MoveTo(DragStartPosition+delta);                        
        }
    }    

	// ----------------------------------------------------------------------
    void EndDragging() {
        WD_Port port= DragObject as WD_Port;
        if(port != null) {
            port.IsBeingDragged= false;
            // Verify for a new connection.
            if(!VerifyNewConnection(port)) {
                // Verify for disconnection.
                if(!port.IsNearParent()) {
                    if(port is WD_DataPort) {
                        (port as WD_DataPort).Disconnect();                        
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
    WD_Object DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        if(!Mouse.IsLeftButtonDown && !Mouse.IsRightButtonDown) return SelectedObject;
        WD_Object newSelected= GetObjectAtMousePosition();
        SelectedObject= newSelected;
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public WD_Object GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(Mouse.Position);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public WD_Object GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
        WD_Port port= RootNode.GetPortAt(graphPosition);
        if(port != null) return port;
        WD_Node node= RootNode.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(WD_Port port) {
        // No new connection if no overlapping port found.
        WD_Port overlappingPort= port.GetOverlappingPort();
        if(overlappingPort == null) return false;
        
        // Only connect data ports.
        if(!(port is WD_DataPort)) return false;
        if(!(overlappingPort is WD_DataPort)) return false;
        WD_DataPort dataPort= port as WD_DataPort;
        WD_DataPort overlappingDataPort= overlappingPort as WD_DataPort;
        
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
        DisplayRoot.ForEachRecursiveDepthLast<WD_Node>( (node)=> { Graphics.DrawNode(node, SelectedObject); } );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        DisplayRoot.ForEachRecursive<WD_Port>( (port)=> { Graphics.DrawConnection(port, SelectedObject); } );

        // Display ports.
        DisplayRoot.ForEachRecursive<WD_Port>( (port)=> { Graphics.DrawPort(port, SelectedObject); } );
    }

}
