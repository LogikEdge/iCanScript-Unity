using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the UK_Behaviour.
public class UK_Editor : EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    UK_IStorage         myStorage      = null;
	UK_Inspector        Inspector      = null;
    UK_EditorObject     DisplayRoot    = null;
    UK_DynamicMenu      DynamicMenu    = null;

    // ----------------------------------------------------------------------
    private UK_Graphics        Graphics        = null;
    public  UK_ScrollView      ScrollView      = null;
    
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortDrag, NodeDrag, TransitionCreation };
    DragTypeEnum    DragType              = DragTypeEnum.None;
    UK_EditorObject DragObject            = null;
    Vector2         MouseDragStartPosition= Vector2.zero;
    Vector2         DragStartPosition     = Vector2.zero;
    bool            IsDragEnabled         = true;
    bool            IsDragStarted         { get { return DragObject != null; }}

    // ----------------------------------------------------------------------
    static bool                 ourAlreadyParsed  = false;
     
    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    UK_EditorObject SelectedObject {
        get { return mySelectedObject; }
        set { mySelectedObject= value; if(Inspector != null) Inspector.SelectedObject= value; }
    }
    UK_EditorObject mySelectedObject= null;
    public UK_IStorage Storage { get { return myStorage; } set { myStorage= value; }}
	// ----------------------------------------------------------------------
    bool    IsCommandKeyDown       { get { return Event.current.command; }}
    bool    IsControlKeyDown       { get { return Event.current.control; }}
    Vector2 MousePosition          {
        get {
            Vector2 pos= Event.current.mousePosition;
            if(Event.current.type == EventType.MouseDrag) pos+= Event.current.delta;
            return pos;
        }
    }
    
    // ======================================================================
    // INITIALIZATION
	// ----------------------------------------------------------------------
    // Prepares the editor for editing a graph.  Note that the graph to edit
    // is not configured at this point.  We must wait for an activate from
    // the graph inspector to know which graph to edit. 
	void OnEnable() {        
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        Graphics        = new UK_Graphics();
        ScrollView      = new UK_ScrollView();
        DynamicMenu     = new UK_DynamicMenu();
        
        // Inspect the assemblies for components.
        if(!ourAlreadyParsed) {
            ourAlreadyParsed= true;
            UK_Reflection.ParseAppDomain();
        }
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the UK_Behaviour editor.
    void OnDisable() {
        // Release all worker objects.
        Graphics    = null;
        ScrollView  = null;
        DynamicMenu = null;
    }
    
    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(UK_IStorage storage, UK_Inspector inspector) {
//        Debug.Log("Editor Activated");
        Storage= storage;
        Inspector= inspector;
        DisplayRoot= null;
    }
    
    // ----------------------------------------------------------------------
    public void Deactivate() {
//        Debug.Log("Editor Deactivated");
        Inspector      = null;
		DisplayRoot    = null;
		Storage        = null;
    }

    // ----------------------------------------------------------------------
    public void SetInspector(UK_Inspector inspector) {
        Inspector= inspector;
    }
    
	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	public bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(Storage == null) { return false; }
        
		// Don't run if graphic sub-system did not initialise.
		if(UK_Graphics.IsInitialized == false) {
            UK_Graphics.Init(Storage);
			return false;
		}
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
    static float ourLastDirtyUpdateTime;
    static int   ourRefreshCnt;
	void Update() {
        // Determine repaint rate.
        if(Storage != null) {
            if(Storage.IsDirty) {
                ourLastDirtyUpdateTime= Time.realtimeSinceStartup;
            }
            float timeSinceDirty= Time.realtimeSinceStartup-ourLastDirtyUpdateTime;
            if(timeSinceDirty < 5.0f) {
                Repaint();
            } else {
                if(++ourRefreshCnt > 2 || ourRefreshCnt < 0) {
                    ourRefreshCnt= 0;
                    Repaint();
                }
            }
        }
        // Cleanup objects.
        UK_AutoReleasePool.Update();
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

        // Process user inputs
        ProcessEvents();
	}

    // ======================================================================
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*UK_EditorConfig.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*UK_EditorConfig.EditorWindowGutterSize+UK_EditorConfig.EditorWindowToolbarHeight;
    }
    

#region User Interaction
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    void ProcessEvents() {
        // Process window events.
        switch(Event.current.type) {
            case EventType.MouseMove: {
                ResetDrag();
                break;
            }
            case EventType.MouseDrag: {
                ProcessDrag();
                Event.current.Use();
                break;
            }
            case EventType.MouseDown: {
                // Update the selected object.
                DetermineSelectedObject();
                switch(Event.current.button) {
                    case 0: { // Left mouse button
                        if(IsDragStarted) {
                            ProcessDrag();
                        }
                        Event.current.Use();
                        break;
                    }
                    case 1: { // Right mouse button
                        DynamicMenu.Update(SelectedObject, Storage, ScrollView.ScreenToGraph(MousePosition));
                        Event.current.Use();
                        break;
                    }
                }
                break;
            }
            case EventType.MouseUp: {
                if(IsDragStarted) {
                    EndDrag();
                } else {
                    if(SelectedObject != null) {
                        // Process fold/unfold click.
                        Vector2 graphMousePos= ScrollView.ScreenToGraph(MousePosition);
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
                }
                Event.current.Use();
                break;
            }

            // Unity DragAndDrop events.
            case EventType.DragPerform: {
                DragAndDropPerform();
                Event.current.Use();                
                break;
            }
            case EventType.DragUpdated: {
                DragAndDropUpdated();
                Event.current.Use();            
                break;
            }
            case EventType.DragExited: {
                if(position.Contains(MousePosition)) {
                    DragAndDropExited();
                    Event.current.Use();
                }
                break;
            }
        }
    }
	// ----------------------------------------------------------------------
    void DragAndDropPerform() {
        // Show a copy icon on the drag
        UK_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            DragAndDrop.AcceptDrag();                                                            
        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropUpdated() {
        // Show a copy icon on the drag
        UK_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
        UK_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            switch(EditorUtility.DisplayDialogComplex("Importing Library",
                                                      "Libraries can be either copied into the graph or linked to the graph.  A copied library is decoupled from its originating library and can be modified directly inside the iCanScript graph.  A linked library references the original library and cannot be modified inplace.",
                                                      "Copy Library",
                                                      "Cancel",
                                                      "Link Library")) {
                case 0: {
                    PasteIntoGraph(ScrollView.ScreenToGraph(MousePosition), storage, storage.EditorObjects[0]);
                    break;
                }
                case 1: {
                    break;
                }
                case 2: {
                    Debug.LogWarning("Linked library not supported in this version.");
                    break;
                }
            }
        }
    }
	// ----------------------------------------------------------------------
    UK_Storage GetDraggedLibrary() {
        UnityEngine.Object[] draggedObjects= DragAndDrop.objectReferences;
        if(draggedObjects.Length >= 1) {
            GameObject go= draggedObjects[0] as GameObject;
            if(go != null) {
                UK_Storage storage= go.GetComponent<UK_Library>();
                return storage;
            }
        }
        return null;
    }
    
	// ----------------------------------------------------------------------
    void ProcessDrag() {
        // Return if dragging is not enabled.
        if(!IsDragEnabled) return;

        // Start a new drag (if not already started).
        if(!StartDrag()) return;

        // Compute new object position.
        Vector2 mousePosition= ScrollView.ScreenToGraph(MousePosition);
        Vector2 delta= mousePosition - MouseDragStartPosition;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                UK_EditorObject node= DragObject;
                Storage.MoveTo(node, DragStartPosition+delta);
                Storage.SetDirty(node);                        
                node.IsFloating= IsCommandKeyDown;
                break;
            case DragTypeEnum.PortDrag:
            case DragTypeEnum.TransitionCreation:
                UK_EditorObject port= DragObject;
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
        MouseDragStartPosition= ScrollView.ScreenToGraph(MousePosition);
        Vector2 pos= MouseDragStartPosition;

        // Port drag.
        UK_EditorObject port= Storage.GetPortAt(pos);
        if(port != null && !Storage.IsMinimized(port)) {
            Storage.RegisterUndo("Port Drag");
            DragType= DragTypeEnum.PortDrag;
            DragObject= port;
            DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
            port.IsFloating= true;
            return true;
        }

        // Node drag.
        UK_EditorObject node= Storage.GetNodeAt(pos);                
        if(node != null && (node.IsMinimized || !node.IsState || Graphics.IsNodeTitleBarPicked(node, pos, Storage))) {
            if(IsControlKeyDown) {
                GameObject go= new GameObject(node.Name);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent("UK_Library");
                UK_Library library= go.GetComponent<UK_Library>();
                UK_IStorage iStorage= new UK_IStorage(library);
                iStorage.CloneInstance(node, Storage, null, Vector2.zero);
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(node.Name);
                UK_AutoReleasePool.AutoRelease(go, 60f);
                // Disable dragging.
                IsDragEnabled= false;
                DragType= DragTypeEnum.None;
            } else {
                Storage.RegisterUndo("Node Drag");
                node.IsFloating= IsCommandKeyDown;
                DragType= DragTypeEnum.NodeDrag;
                DragObject= node;
                Rect position= Storage.GetPosition(node);
                DragStartPosition= new Vector2(position.x, position.y);                                                                    
            }
            return true;
        }
        
        // New state transition drag.
        if(node != null && node.IsState) {
            Storage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            UK_EditorObject outTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), UK_ObjectTypeEnum.OutStatePort);
            UK_EditorObject inTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), UK_ObjectTypeEnum.InStatePort);
            Storage.SetInitialPosition(outTransition, pos);
            Storage.SetInitialPosition(inTransition, pos);
            inTransition.Source= outTransition.InstanceId;
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
    void EndDrag() {
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    UK_EditorObject node= DragObject;
                    UK_EditorObject oldParent= Storage.GetParent(node);
                    UK_EditorObject newParent= GetValidParentNodeUnder(node);
                    if(newParent != null && newParent != oldParent) {
                        ChangeParent(node, newParent);
                    }
                    if(oldParent != null) Storage.SetDirty(oldParent);
                    node.IsFloating= false;
                    break;
                }
                case DragTypeEnum.PortDrag:
                    UK_EditorObject port= DragObject;
                    port.IsFloating= false;
                    // Verify for a new connection.
                    if(!VerifyNewConnection(port)) {
                        // Verify for disconnection.
                        Storage.SetDirty(port);
                        if(!Storage.IsNearParent(port)) {
                            if(port.IsDataPort) {
                                UK_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    UK_EditorObject portParent= Storage.GetParent(port);
                                    Rect portPos= Storage.GetPosition(port);
                                    Rect modulePos= Storage.GetPosition(newPortParent);
                                    float portSize2= 2f*UK_EditorConfig.PortSize;
                                    if(port.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!Storage.IsChildOf(newPortParent, portParent)) {
                                                UK_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, UK_ObjectTypeEnum.InDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(newPort, port);                               
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent)) {
                                                UK_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, UK_ObjectTypeEnum.OutDynamicModulePort);
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
                                                UK_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, UK_ObjectTypeEnum.OutDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(port, newPort);                               
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent) || Storage.IsChildOf(newPortParent, portParent)) {
                                                UK_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, UK_ObjectTypeEnum.InDynamicModulePort);
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
                    UK_EditorObject destState= GetNodeAtMousePosition();
                    if(destState != null && destState.IsState) {
                        UK_EditorObject outStatePort= Storage[DragObject.Source];
                        outStatePort.IsFloating= false;
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
            ResetDrag();
        }
    }
	// ----------------------------------------------------------------------
    void ResetDrag() {
        DragType= DragTypeEnum.None;
        DragObject= null;
        IsDragEnabled= true;                    
    }
#endregion User Interaction
    
	// ----------------------------------------------------------------------
    // Manages the object selection.
    UK_EditorObject DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        UK_EditorObject newSelected= GetObjectAtMousePosition();
        SelectedObject= newSelected;
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public UK_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(MousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public UK_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ScrollView.ScreenToGraph(MousePosition);
        return Storage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public UK_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
        UK_EditorObject port= Storage.GetPortAt(graphPosition);
        if(port != null) {
            if(Storage.IsMinimized(port)) return Storage.GetParent(port);
            return port;
        }
        UK_EditorObject node= Storage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(UK_EditorObject port) {
        // No new connection if no overlapping port found.
        UK_EditorObject overlappingPort= Storage.GetOverlappingPort(port);
        if(overlappingPort == null) return false;
        
        // Reestablish port position.
        port.LocalPosition.x= DragStartPosition.x;
        port.LocalPosition.y= DragStartPosition.y;
        
        // Connect function & modules ports together.
        if(port.IsDataPort && overlappingPort.IsDataPort) {            
            UK_EditorObject inPort = null;
            UK_EditorObject outPort= null;

            UK_EditorObject portParent= Storage.EditorObjects[port.ParentId];
            UK_EditorObject overlappingPortParent= Storage.EditorObjects[overlappingPort.ParentId];
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
                if(UK_Types.CanBeConnectedWithoutConversion(outPort.RuntimeType, inPort.RuntimeType)) { // No conversion needed.
                    SetNewDataConnection(inPort, outPort);                       
                }
                else {  // A conversion is required.
                    UK_ReflectionDesc conversion= UK_DataBase.FindConversion(outPort.RuntimeType, inPort.RuntimeType);
                    if(conversion == null) {
                        Debug.LogWarning("No direct conversion exists from "+outPort.RuntimeType.Name+" to "+inPort.RuntimeType.Name);
                    } else {
                        SetNewDataConnection(inPort, outPort, conversion);
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
    void SetNewDataConnection(UK_EditorObject inPort, UK_EditorObject outPort, UK_ReflectionDesc conversion= null) {
        UK_EditorObject inNode= Storage.GetParent(inPort);
        UK_EditorObject outNode= Storage.GetParent(outPort);
        UK_EditorObject inParent= GetParentModule(inNode);
        UK_EditorObject outParent= GetParentModule(outNode);
        // No need to create module ports if both connected nodes are under the same parent.
        if(inParent == outParent) {
            Storage.SetSource(inPort, outPort, conversion);
            return;
        }
        if(inParent == null) {
            UK_EditorObject newPort= Storage.CreatePort(inPort.Name, inParent.InstanceId, inPort.RuntimeType, UK_ObjectTypeEnum.InDynamicModulePort);
            Storage.SetSource(inPort, newPort, conversion);
            SetNewDataConnection(newPort, outPort);
            return;           
        }
        if(outParent == null) {
            UK_EditorObject newPort= Storage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, UK_ObjectTypeEnum.OutDynamicModulePort);
            Storage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            return;                       
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(UK_EditorObject op= GetParentModule(outParent); op != null; op= GetParentModule(op)) {
            if(inParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen) {
            UK_EditorObject newPort= Storage.CreatePort(inPort.Name, inParent.InstanceId, inPort.RuntimeType, UK_ObjectTypeEnum.InDynamicModulePort);
            Storage.SetSource(inPort, newPort, conversion);
            SetNewDataConnection(newPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(UK_EditorObject ip= GetParentModule(inParent); ip != null; ip= GetParentModule(ip)) {
            if(outParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen) {
            UK_EditorObject newPort= Storage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, UK_ObjectTypeEnum.OutDynamicModulePort);
            Storage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        Storage.SetSource(inPort, outPort, conversion);
    }
	// ----------------------------------------------------------------------
    UK_EditorObject GetParentModule(UK_EditorObject edObj) {
        UK_EditorObject parentModule= Storage.GetParent(edObj);
        for(; parentModule != null && !parentModule.IsModule; parentModule= Storage.GetParent(parentModule));
        return parentModule;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject GetValidParentNodeUnder(Vector2 point, UK_ObjectTypeEnum objType) {
        UK_EditorObject parent= Storage.GetNodeAt(point);
        if(parent == null) return null;
        switch(objType) {
            case UK_ObjectTypeEnum.StateChart: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;                
            }
            case UK_ObjectTypeEnum.State: {
                while(parent != null && !parent.IsState && !parent.IsStateChart) parent= Storage.GetParent(parent);
                break;
            }
            case UK_ObjectTypeEnum.Module: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;
            }
            case UK_ObjectTypeEnum.InstanceMethod:
            case UK_ObjectTypeEnum.StaticMethod:
            case UK_ObjectTypeEnum.InstanceField:
            case UK_ObjectTypeEnum.StaticField:
            case UK_ObjectTypeEnum.Conversion: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;
            }
            default: {
                parent= null;
                break;
            }
        }
        return parent;
    }
	// ----------------------------------------------------------------------
    UK_EditorObject GetValidParentNodeUnder(UK_EditorObject node) {
        if(!node.IsNode) return null;
        Vector2 point= Math3D.Middle(Storage.GetPosition(node));
        UK_EditorObject parent= Storage.GetNodeAt(point, node);
        if(parent == null) return null;
        switch(node.ObjectType) {
            case UK_ObjectTypeEnum.StateChart: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;                
            }
            case UK_ObjectTypeEnum.State: {
                while(parent != null && !parent.IsState && !parent.IsStateChart) parent= Storage.GetParent(parent);
                break;
            }
            case UK_ObjectTypeEnum.Module: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;
            }
            case UK_ObjectTypeEnum.InstanceMethod:
            case UK_ObjectTypeEnum.StaticMethod:
            case UK_ObjectTypeEnum.InstanceField:
            case UK_ObjectTypeEnum.StaticField:
            case UK_ObjectTypeEnum.Conversion: {
                while(parent != null && !parent.IsModule) parent= Storage.GetParent(parent);
                break;
            }
            default: {
                parent= null;
                break;
            }
        }
        return parent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(UK_EditorObject node, UK_EditorObject newParent) {
        UK_EditorObject oldParent= Storage.GetParent(node);
        if(newParent == null || newParent == oldParent) return;
        Storage.SetParent(node, newParent);
        CleanupConnections(node);
    }
	// ----------------------------------------------------------------------
    void CleanupConnections(UK_EditorObject node) {
        switch(node.ObjectType) {
            case UK_ObjectTypeEnum.StateChart: {
                List<UK_EditorObject> childNodes= new List<UK_EditorObject>();
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;                
            }
            case UK_ObjectTypeEnum.State: {
                // Attempt to relocate transition modules.
                Storage.ForEachChildPort(node,
                    p=> {
                        if(p.IsStatePort) {
                            UK_EditorObject transitionModule= null;
                            if(p.IsInStatePort) {
                                transitionModule= Storage.GetParent(Storage.GetSource(p));
                            } else {
                                UK_EditorObject[] connectedPorts= Storage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= Storage.GetParent(cp);
                                        break;
                                    }
                                }
                            }
                            UK_EditorObject outState= Storage.GetParent(Storage.GetOutStatePort(transitionModule));
                            UK_EditorObject inState= Storage.GetParent(Storage.GetInStatePort(transitionModule));
                            UK_EditorObject newParent= Storage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != Storage.GetParent(transitionModule)) {
                                ChangeParent(transitionModule, newParent);
                                Storage.LayoutTransitionModule(transitionModule);
                                Storage.SetDirty(Storage.GetParent(node));
                            }
                        }
                    }
                );
                // Ask our children to cleanup their connections.
                List<UK_EditorObject> childNodes= new List<UK_EditorObject>();
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case UK_ObjectTypeEnum.TransitionModule:
            case UK_ObjectTypeEnum.TransitionGuard:
            case UK_ObjectTypeEnum.TransitionAction:
            case UK_ObjectTypeEnum.Module: {
                List<UK_EditorObject> childNodes= new List<UK_EditorObject>();
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case UK_ObjectTypeEnum.InstanceMethod:
            case UK_ObjectTypeEnum.StaticMethod:
            case UK_ObjectTypeEnum.InstanceField:
            case UK_ObjectTypeEnum.StaticField:
            case UK_ObjectTypeEnum.Conversion: {
                Storage.ForEachChildPort(node,
                    port=> {
                        if(port.IsInDataPort) {
                            UK_EditorObject sourcePort= RemoveConnection(port);
                            // Rebuild new connection.
                            if(sourcePort != port) {
                                SetNewDataConnection(port, sourcePort);
                            }
                        }
                        if(port.IsOutDataPort) {
                            UK_EditorObject[] allInPorts= FindAllConnectedInDataPorts(port);
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
    UK_EditorObject RemoveConnection(UK_EditorObject inPort) {
        UK_EditorObject sourcePort= Storage.GetDataConnectionSource(inPort);
        // Tear down previous connection.
        UK_EditorObject tmpPort= Storage.GetSource(inPort);
        List<UK_EditorObject> toDestroy= new List<UK_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            UK_EditorObject[] connected= Storage.FindConnectedPorts(tmpPort);
            if(connected.Length == 1) {
                UK_EditorObject t= Storage.GetSource(tmpPort);
                toDestroy.Add(tmpPort);
                tmpPort= t;
            } else {
                break;
            }
        }
        foreach(var byebye in toDestroy) {
            Storage.DestroyInstance(byebye.InstanceId);
        }
        return sourcePort;        
    }
    // ----------------------------------------------------------------------
    UK_EditorObject[] FindAllConnectedInDataPorts(UK_EditorObject outPort) {
        List<UK_EditorObject> allInDataPorts= new List<UK_EditorObject>();
        FillConnectedInDataPorts(outPort, allInDataPorts);
        return allInDataPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    void FillConnectedInDataPorts(UK_EditorObject outPort, List<UK_EditorObject> result) {
        if(outPort == null) return;
        UK_EditorObject[] connectedPorts= Storage.FindConnectedPorts(outPort);
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
    void PasteIntoGraph(Vector2 point, UK_Storage pasteStorage, UK_EditorObject pasteRoot) {
        if(pasteRoot == null) return;
        UK_EditorObject parent= GetValidParentNodeUnder(point, pasteRoot.ObjectType);
        if(parent == null) {
            EditorUtility.DisplayDialog("Operation Aborted", "Unable to find a suitable parent to paste into !!!", "Cancel");
            return;
        }
        UK_EditorObject pasted= Storage.CloneInstance(pasteRoot, new UK_IStorage(pasteStorage), parent, point);
        Storage.Fold(pasted);
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
    	DrawNormalNodes();
        DrawConnections();
        DrawMinimizedNodes();           
        ScrollView.End();
	}

	// ----------------------------------------------------------------------
    void DrawNormalNodes() {
        List<UK_EditorObject> floatingNodes= new List<UK_EditorObject>();
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode) {
                    if(node.IsFloating) {
                        floatingNodes.Add(node);
                    } else {
                        Graphics.DrawNormalNode(node, SelectedObject, Storage);                        
                    }
                }
            }
        );
        foreach(var node in floatingNodes) {
            Graphics.DrawNormalNode(node, SelectedObject, Storage);            
        }
    }	
	// ----------------------------------------------------------------------
    void DrawMinimizedNodes() {
        List<UK_EditorObject> floatingNodes= new List<UK_EditorObject>();
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode) {
                    if(node.IsFloating) {
                        floatingNodes.Add(node);
                    } else {
                        Graphics.DrawMinimizedNode(node, SelectedObject, Storage);                        
                    }
                }
            }
        );
        foreach(var node in floatingNodes) {
            Graphics.DrawMinimizedNode(node, SelectedObject, Storage);            
        }
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawConnection(port, Storage); });

        // Display ports.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawPort(port, SelectedObject, Storage); });
    }

}
