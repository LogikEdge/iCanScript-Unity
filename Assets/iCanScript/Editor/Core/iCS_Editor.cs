using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the iCS_Behaviour.
public class iCS_Editor : EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    iCS_IStorage         myStorage      = null;
	iCS_Inspector        Inspector      = null;
    iCS_EditorObject     DisplayRoot    = null;
    iCS_DynamicMenu      DynamicMenu    = null;
    
    // ----------------------------------------------------------------------
    float LastDirtyUpdateTime= 0f;
    int   RefreshCnt         = 0;
    
    // ----------------------------------------------------------------------
    private iCS_Graphics    Graphics  = null;
    
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortDrag, NodeDrag, TransitionCreation };
    DragTypeEnum     DragType              = DragTypeEnum.None;
    iCS_EditorObject DragObject            = null;
    iCS_EditorObject DragFixPort           = null;
    iCS_EditorObject DragOriginalPort      = null;
    Vector2          MouseDragStartPosition= Vector2.zero;
    Vector2          DragStartPosition     = Vector2.zero;
    bool             IsDragEnabled         = false;
    bool             IsDragStarted         { get { return IsDragEnabled && DragObject != null; }}

    // ----------------------------------------------------------------------
    iCS_EditorObject Bookmark= null;
    
    // ----------------------------------------------------------------------
    Rect    ClipingArea { get { return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height); }}
    Vector2 ViewportCenter { get { return new Vector2(0.5f/Scale*position.width, 0.5f/Scale*position.height); } }
    Rect    Viewport { get { return new Rect(0,0,position.width/Scale, position.height/Scale); }}
    Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;
     
    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    iCS_EditorObject StorageRoot {
        get {
            if(Storage == null || Prelude.length(Storage.EditorObjects) == 0) return null;
            return Storage.EditorObjects[0];
        }
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject SelectedObject {
        get { return mySelectedObject; }
        set {
			if(mySelectedObject != value) {
				mySelectedObject= value;
				if(Inspector != null) Inspector.SelectedObject= value;				
			}
		}
    }
    iCS_EditorObject mySelectedObject= null;
    public iCS_IStorage Storage { get { return myStorage; } set { myStorage= value; }}
	// ----------------------------------------------------------------------
    Vector2     ScrollPosition { get { return Storage.ScrollPosition; } set { Storage.ScrollPosition= value; }}
    float       Scale {
        get { return Storage.GuiScale; }
        set {
            if(value > 1f) value= 1f;
            if(value < 0.15f) value= 0.15f;
            Storage.GuiScale= value;
        }
    }
    Prelude.Animate<Vector2>    AnimatedScrollPosition= new Prelude.Animate<Vector2>();

	// ----------------------------------------------------------------------
    bool    IsFloatingKeyDown	{ get { return Event.current.control; }}
    bool    IsCopyKeyDown       { get { return Event.current.shift; }}
    bool    IsScaleKeyDown      { get { return Event.current.alt; }}
    
	// ----------------------------------------------------------------------
	// Mouse services
	void UpdateMouse() {
        myMousePosition= Event.current.mousePosition;
        if(Event.current.type == EventType.MouseDrag) myMousePosition+= Event.current.delta;
	}
    Vector2 MousePosition { get { return myMousePosition/Scale; } }
	Vector2 myMousePosition= Vector2.zero;
	Vector2 MouseDownPosition= Vector2.zero;
	
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
        Graphics        = new iCS_Graphics();
        DynamicMenu     = new iCS_DynamicMenu();

        // Reset selected object.
        SelectedObject= null;
        
        // Inspect the assemblies for components.
        if(!ourAlreadyParsed) {
            ourAlreadyParsed= true;
            iCS_Reflection.ParseAppDomain();
        }
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_Behaviour editor.
    void OnDisable() {
        // Release all worker objects.
        Graphics    = null;
        DynamicMenu = null;
    }
    
    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(iCS_IStorage storage, iCS_Inspector inspector) {
        Storage= storage;
        Inspector= inspector;
        // Set the graph root.
        DisplayRoot= storage.IsValid(0) ? storage[0] : null;
		
//        Debug.Log("AppContentPath: "+EditorApplication.applicationContentsPath);
//        Debug.Log("AppPath: "+EditorApplication.applicationPath);
//        if(!iCS_LicenseFile.Exists) {
//            Debug.Log("Generating license file.");
//            iCS_LicenseFile.FillCustomerInformation("Michel Launier", "11-22-33-44-55-66-77-88-99-aa-bb-cc-dd-ee-ff-00", iCS_LicenseFile.LicenseTypeEnum.Pro);            
//            iCS_LicenseFile.SetUnlockKey(iCS_UnlockKeyGenerator.Pro);            
//        }
//        if(iCS_LicenseFile.IsCorrupted) {
//            EditorApplication.Beep();
//            EditorUtility.DisplayDialog("Corrupted iCanScript License File", "The iCanScript license file has been corrupted.  Disruptive Software will be advise of the situation and your serial number may be revoqued. iCanScript will go back to Demo mode.", "Clear License File");
//            iCS_LicenseFile.Reset();
//            iCS_LicenseFile.FillCustomerInformation("Michel Launier", "11-22-33-44-55-66-77-88-99-aa-bb-cc-dd-ee-ff-00", iCS_LicenseFile.LicenseTypeEnum.Pro);            
//            iCS_LicenseFile.SetUnlockKey(iCS_UnlockKeyGenerator.Pro);            
//        }

    }
    
    // ----------------------------------------------------------------------
    public void Deactivate() {
        Inspector      = null;
		DisplayRoot    = null;
		Storage        = null;
    }

    // ----------------------------------------------------------------------
    public void SetInspector(iCS_Inspector inspector) {
        Inspector= inspector;
    }
    
	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	public bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(Storage == null) { return false; }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(Storage);
			return false;
		}
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	void Update() {
        // Determine repaint rate.
        if(Storage != null) {
            // Repaint window
            if(Storage.IsDirty) {
                LastDirtyUpdateTime= Time.realtimeSinceStartup;
            }
            float timeSinceDirty= Time.realtimeSinceStartup-LastDirtyUpdateTime;
            if(timeSinceDirty < 5.0f) {
                Repaint();
            } else {
                if(++RefreshCnt > 4 || RefreshCnt < 0) {
                    RefreshCnt= 0;
                    Repaint();
                }
            }
            // Update DisplayRoot
            if(DisplayRoot == null && Storage.IsValid(0)) {
                DisplayRoot= Storage[0];
            }
        }
        // Cleanup objects.
        iCS_AutoReleasePool.Update();
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
        
		// Update mouse info.
		UpdateMouse();

        // Draw Graph.
        DrawGraph();

        // Process user inputs
        ProcessEvents();
	}

    // ======================================================================
    // EDITOR WINDOW MAIN LAYOUT
	// ----------------------------------------------------------------------
    float UsableWindowWidth() {
        return position.width-2*iCS_EditorConfig.EditorWindowGutterSize;
    }
    
	// ----------------------------------------------------------------------
    float UsableWindowHeight() {
        return position.height-2*iCS_EditorConfig.EditorWindowGutterSize+iCS_EditorConfig.EditorWindowToolbarHeight;
    }
    

#region User Interaction
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    void ProcessEvents() {
        // Process window events.
        switch(Event.current.type) {
            case EventType.MouseMove: {
                switch(Event.current.button) {
                    case 0: { // Left mouse button
                        ResetDrag();
                        break;
                    }
                    case 2: { // Middle mouse button
                        Vector2 diff= MousePosition-MouseDragStartPosition;
                        ScrollPosition= DragStartPosition-diff;
                        break;
                    }
                }
                break;
            }
            case EventType.MouseDrag: {
                switch(Event.current.button) {
                    case 0: { // Left mouse button
                        ProcessDrag();                            
                        break;
                    }
                    case 2: { // Middle mouse button
                        Vector2 diff= MousePosition-MouseDragStartPosition;
                        ScrollPosition= DragStartPosition-diff;
                        break;
                    }
					default: {
						break;
					}
                }
                Event.current.Use();
                break;
            }
            case EventType.MouseDown: {
				MouseDownPosition= MousePosition;
                // Update the selected object.
                DetermineSelectedObject();
                switch(Event.current.button) {
                    case 0: { // Left mouse button
                        if(SelectedObject != null && !SelectedObject.IsBehaviour) {
                            IsDragEnabled= true;                            
                        }
                        Event.current.Use();
                        break;
                    }
                    case 1: { // Right mouse button
                        ShowDynamicMenu();
                        Event.current.Use();
                        break;
                    }
                    case 2: { // Middle mouse button
                        MouseDragStartPosition= MousePosition;
                        DragStartPosition= ScrollPosition;
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
                        Vector2 graphMousePos= ViewportToGraph(MousePosition);
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
                            Storage.Fold(SelectedObject);
                        }
                    }                                                
                }
                Event.current.Use();
                break;
            }
            case EventType.ScrollWheel: {
                Vector2 delta= Event.current.delta;
                if(IsScaleKeyDown) {
                    Vector2 pivot= ViewportToGraph(MousePosition);
					float zoomDirection= Storage.Preferences.ControlOptions.InverseZoom ? -1f : 1f;
                    Scale= Scale+(delta.y > 0 ? -0.05f : 0.05f)*zoomDirection;
                    Vector2 offset= pivot-ViewportToGraph(MousePosition);
                    ScrollPosition+= offset;
                } else {
                    delta*= Storage.Preferences.ControlOptions.ScrollSpeed*(1f/Scale); 
                    ScrollPosition+= delta;                    
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
                if(mouseOverWindow == this) {
                    DragAndDropExited();
                    Event.current.Use();
                }
                break;
            }
			case EventType.KeyDown: {
				var ev= Event.current;
				if(ev.keyCode == KeyCode.None) break;
//				Debug.Log("KeyCode: "+ev.keyCode);
                switch(ev.keyCode) {
                    // Tree navigation
                    case KeyCode.UpArrow: {
                        if(SelectedObject != null) {
                            SelectedObject= Storage.GetParent(SelectedObject);
                            CenterOnSelected();
                        } 
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.DownArrow: {
                        if(SelectedObject == null) SelectedObject= DisplayRoot;
                        SelectedObject= iCS_EditorUtility.GetFirstChild(SelectedObject, Storage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.RightArrow: {
                        SelectedObject= iCS_EditorUtility.GetNextSibling(SelectedObject, Storage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.LeftArrow: {
                        SelectedObject= iCS_EditorUtility.GetPreviousSibling(SelectedObject, Storage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    // Fold/Minimize/Maximize.
                    case KeyCode.Return: {
                        if(SelectedObject != null) {
                            if(!ev.shift) {
                                if(SelectedObject.IsMinimized) {
                                    Storage.Maximize(SelectedObject);
                                    Storage.Fold(SelectedObject);
                                } else if(SelectedObject.IsFolded) {
                                    Storage.Maximize(SelectedObject);                                    
                                }
                            } else {
                                if(SelectedObject.IsDisplayedNormally) {
                                    Storage.Fold(SelectedObject);
                                } else if(SelectedObject.IsFolded) {
                                    Storage.Minimize(SelectedObject);
                                }
                            }                            
                        }
                        Event.current.Use();
                        break;
                    }
                    // Bookmarks
                    case KeyCode.B: {
                        if(SelectedObject != null) {
                            Bookmark= SelectedObject;                            
                        }
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.G: {
                        if(Bookmark != null) {
                            SelectedObject= Bookmark;
                            CenterOnSelected();
                        }
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.S: {
                        if(Bookmark != null && SelectedObject != null) {
                            iCS_EditorObject prevBookmark= Bookmark;
                            Bookmark= SelectedObject;
                            SelectedObject= prevBookmark;
                            CenterOnSelected();
                        } else if(Bookmark != null && SelectedObject == null) {
                            SelectedObject= Bookmark;
                            CenterOnSelected();
                        } else if(Bookmark == null && SelectedObject != null) {
                            Bookmark= SelectedObject;
                        }
                        Event.current.Use();
                        break;
                    }
                    // Object deletion
                    case KeyCode.Delete:
                    case KeyCode.Backspace: {
                        if(SelectedObject != null && SelectedObject != DisplayRoot && SelectedObject != StorageRoot) {
                            iCS_EditorObject parent= Storage.GetParent(SelectedObject);
                            if(ev.shift) {
                                Storage.RegisterUndo("Delete");
                                Storage.DestroyInstance(SelectedObject.InstanceId);                                                        
                            } else {
                                iCS_EditorUtility.DestroyObject(SelectedObject, Storage);                                
                            }
                            SelectedObject= parent;
                        }
                        Event.current.Use();
                        break;
                    }
                    // Object creation.
                    case KeyCode.KeypadEnter: // fnc+return on Mac
                    case KeyCode.Insert: {
                        if(SelectedObject == null) SelectedObject= DisplayRoot;
                        // Don't use mouse position if it is too far from selected node.
                        Vector2 graphPos= ViewportToGraph(MousePosition);
                        Rect parentRect= Storage.GetPosition(SelectedObject);
                        Vector2 parentOrigin= new Vector2(parentRect.x, parentRect.y);
                        Vector2 parentCenter= Math3D.Middle(parentRect);
                        float radius= Vector2.Distance(parentCenter, parentOrigin);
                        float distance= Vector2.Distance(parentCenter, graphPos);
                        if(distance > (radius+250f)) {
                            graphPos= parentOrigin; 
                        }
                        // Auto-insert on behaviour.
                        if(SelectedObject.IsBehaviour) {
                            iCS_EditorObject newObj= null;
                            if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.BehaviourChildUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.BehaviourChildUpdate);  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.BehaviourChildLateUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.BehaviourChildLateUpdate);                                  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.BehaviourChildFixedUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.BehaviourChildFixedUpdate);                                  
                            }
                            if(newObj != null) {
                                CenterAt(graphPos);
                                if(ev.control) {
                                    SelectedObject= newObj;
                                }
                            }
                            Event.current.Use();
                            break;
                        }
                        // Auto-insert on module.
                        if(SelectedObject.IsModule) {
                            iCS_EditorObject newObj= null;
                            if(!ev.shift) {
                                Storage.RegisterUndo("Create Module");
                                newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, null);                                
                            } else {
                                Storage.RegisterUndo("Create State Chart");
                                newObj= Storage.CreateStateChart(SelectedObject.InstanceId, graphPos, null);
                            }
                            if(ev.control && newObj != null) {
                                SelectedObject= newObj;
                            }
                            Event.current.Use();
                            break;
                        }
                        // Auto-insert on state chart.
                        if(SelectedObject.IsStateChart) {
                            Storage.RegisterUndo("Create State");
                            iCS_EditorObject newObj= Storage.CreateState(SelectedObject.InstanceId, graphPos);
                            if(ev.control && newObj != null) {
                                SelectedObject= newObj;
                            }
                            Event.current.Use();
                            break;
                        }
                        // Auto-insert on state.
                        if(SelectedObject.IsState) {
                            iCS_EditorObject newObj= null;
                            if(!ev.shift) {
                                if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.StateChildOnUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                    newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.StateChildOnUpdate);  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.StateChildOnEntry, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                    newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.StateChildOnEntry);                                  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_EngineStrings.StateChildOnExit, iCS_ObjectTypeEnum.Module, SelectedObject, Storage)) {
                                    newObj= Storage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_EngineStrings.StateChildOnExit);                                  
                                }                                
                            } else {
                                Storage.RegisterUndo("Create State");
                                newObj= Storage.CreateState(SelectedObject.InstanceId, graphPos);
                            }
                            if(ev.control && newObj != null) {
                                SelectedObject= newObj;
                            }
                            Event.current.Use();
                            break;
                        }
                        Event.current.Use();
                        break;
                    }
                }
                break;
			}
        }
    }
	// ----------------------------------------------------------------------
    void ShowDynamicMenu() {
        if(SelectedObject == null && DisplayRoot.IsBehaviour) {
            SelectedObject= DisplayRoot;
        }
        DynamicMenu.Update(SelectedObject, Storage, ViewportToGraph(MousePosition));        
    }
	// ----------------------------------------------------------------------
    void DragAndDropPerform() {
        // Show a copy icon on the drag
        iCS_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            DragAndDrop.AcceptDrag();                                                            
        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropUpdated() {
        // Show a copy icon on the drag
        iCS_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }        
    }
	// ----------------------------------------------------------------------
    void DragAndDropExited() {
        iCS_Storage storage= GetDraggedLibrary();
        if(storage != null) {
            PasteIntoGraph(ViewportToGraph(MousePosition), storage, storage.EditorObjects[0]);
        }
    }
	// ----------------------------------------------------------------------
    iCS_Storage GetDraggedLibrary() {
        UnityEngine.Object[] draggedObjects= DragAndDrop.objectReferences;
        if(draggedObjects.Length >= 1) {
            GameObject go= draggedObjects[0] as GameObject;
            if(go != null) {
                iCS_Storage storage= go.GetComponent<iCS_Library>();
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
        Vector2 delta= MousePosition - MouseDragStartPosition;
        switch(DragType) {
            case DragTypeEnum.None: break;
            case DragTypeEnum.NodeDrag:
                iCS_EditorObject node= DragObject;
                Storage.MoveTo(node, DragStartPosition+delta);
                Storage.SetDirty(node);                        
                node.IsFloating= IsFloatingKeyDown;
                break;
            case DragTypeEnum.PortDrag:
            case DragTypeEnum.TransitionCreation:
                // Update port position.
                Vector2 newLocalPos= DragStartPosition+delta;
                DragObject.LocalPosition.x= newLocalPos.x;
                DragObject.LocalPosition.y= newLocalPos.y;
                if(DragObject.IsStatePort) break;
                // Snap to nearby ports
                Vector2 mousePosInGraph= ViewportToGraph(MousePosition);
                iCS_EditorObject closestPort= Storage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null) {
                    Rect closestPortRect= Storage.GetPosition(closestPort);
                    Vector2 closestPortPos= new Vector2(closestPortRect.x, closestPortRect.y);
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < 4f*iCS_EditorConfig.PortRadius) {
                        Rect parentPos= Storage.GetPosition(Storage.GetParent(DragObject));
                        DragObject.LocalPosition.x= closestPortRect.x-parentPos.x;
                        DragObject.LocalPosition.y= closestPortRect.y-parentPos.y;
                    }                    
                }
                Storage.SetDirty(DragObject);
                // Special case for dynamic module ports.
                if(DragOriginalPort != null && DragOriginalPort.IsModulePort) {
                    if(Storage.IsInside(Storage.GetParent(DragOriginalPort), mousePosInGraph)) {
                        if(DragOriginalPort.IsOutputPort && (Storage.GetSource(DragOriginalPort) != null || DragFixPort != DragOriginalPort)) {
                            BreakDataConnectionDrag();
                        } else {
                            MakeDataConnectionDrag();
                        }
                    } else {
                        if(DragOriginalPort.IsInputPort && (Storage.GetSource(DragOriginalPort) != null || DragFixPort != DragOriginalPort)) {
                            BreakDataConnectionDrag();
                        } else {
                            MakeDataConnectionDrag();
                        }
                    }
                }
                break;
        }
    }    
	// ----------------------------------------------------------------------
    void MakeDataConnectionDrag() {
        if(DragFixPort != DragOriginalPort) {
            Storage.SetSource(DragOriginalPort, DragFixPort);
            Storage.SetSource(DragObject, DragOriginalPort);
            DragFixPort= DragOriginalPort;
        }
    }
	// ----------------------------------------------------------------------
    void BreakDataConnectionDrag() {
        if(DragFixPort == DragOriginalPort) {
            DragFixPort= Storage.GetSource(DragOriginalPort);
            Storage.SetSource(DragObject, DragFixPort);
            Storage.SetSource(DragOriginalPort, null);
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
        iCS_EditorObject port= Storage.GetPortAt(pos);
        if(port != null && !Storage.IsMinimized(port) && !port.IsTransitionPort) {
            Storage.RegisterUndo("Port Drag");
            Storage.CleanupDeadPorts= false;
            DragType= DragTypeEnum.PortDrag;
            DragOriginalPort= port;
            DragFixPort= port;
            // State port can be moved to new parent.
            if(port.IsStatePort) {
                DragObject= port;
                DragObject.IsFloating= true;
                DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
                return true;
            }
            // Data port. Create a drag port as appropriate.
            iCS_EditorObject parent= Storage.GetParent(port);
            if(port.IsInputPort) {
                DragObject= Storage.CreatePort(port.Name, parent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                if(port.IsModulePort) {
                    Storage.SetSource(DragObject, port);
                } else {
                    iCS_EditorObject prevSource= Storage.GetSource(port);
                    if(prevSource != null) {
                        DragFixPort= prevSource;
                        Storage.SetSource(DragObject, prevSource);
                        Storage.DisconnectPort(port);
                    } else {
                        Storage.SetSource(port, DragObject);
                    }                    
                }
            } else {
                DragObject= Storage.CreatePort(port.Name, parent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                Storage.SetSource(DragObject, port);
            }
            Rect portPos= Storage.GetPosition(port);
            Storage.SetInitialPosition(DragObject, new Vector2(portPos.x, portPos.y));
            Storage.SetDisplayPosition(DragObject, portPos);
            DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
            DragObject.IsFloating= true;
            return true;
        }

        // Node drag.
        iCS_EditorObject node= Storage.GetNodeAt(pos);                
        if(node != null && (node.IsMinimized || !node.IsState || Graphics.IsNodeTitleBarPicked(node, pos, Storage))) {
            if(IsCopyKeyDown) {
                GameObject go= new GameObject(node.Name);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent("iCS_Library");
                iCS_Library library= go.GetComponent<iCS_Library>();
                iCS_IStorage iStorage= new iCS_IStorage(library);
                iStorage.CopyFrom(node, Storage, null, Vector2.zero);
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(node.Name);
                iCS_AutoReleasePool.AutoRelease(go, 60f);
                // Disable dragging.
                IsDragEnabled= false;
                DragType= DragTypeEnum.None;
            } else {
                Storage.RegisterUndo("Node Drag");
                node.IsFloating= IsFloatingKeyDown;
                DragType= DragTypeEnum.NodeDrag;
                DragObject= node;
                Rect nodePos= Storage.GetPosition(node);
                DragStartPosition= new Vector2(nodePos.x, nodePos.y);                                                                    
            }
            return true;
        }
        
        // New state transition drag.
        if(node != null && node.IsState) {
            Storage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            iCS_EditorObject outTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutStatePort);
            iCS_EditorObject inTransition= Storage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
            Storage.SetInitialPosition(outTransition, pos);
            Storage.SetInitialPosition(inTransition, pos);
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
    void EndDrag() {
		ProcessDrag();
        try {
            switch(DragType) {
                case DragTypeEnum.None: break;
                case DragTypeEnum.NodeDrag: {
                    iCS_EditorObject node= DragObject;
                    iCS_EditorObject oldParent= Storage.GetParent(node);
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(node);
                        if(newParent != null) {
                            if(newParent != oldParent) {
                                ChangeParent(node, newParent);
                            }
                        } else {
                            Storage.MoveTo(node, DragStartPosition);
                        }
                        Storage.SetDirty(oldParent);                        
                    }
                    node.IsFloating= false;
                    break;
                }
                case DragTypeEnum.PortDrag:
                    // Verify for a new connection.
                    if(!VerifyNewConnection(DragFixPort, DragObject)) {
                        bool isNearParent= Storage.IsNearParent(DragObject);
                        if(DragFixPort.IsDataPort) {
                            // We don't need the drag port anymore.
                            Rect dragPortPos= Storage.GetPosition(DragObject);
                            Storage.DestroyInstance(DragObject);
                            // Verify for disconnection.
                            if(!isNearParent) {
                                // Let's determine if we want to create a module port.
                                iCS_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    iCS_EditorObject portParent= Storage.GetParent(DragFixPort);
                                    Rect modulePos= Storage.GetPosition(newPortParent);
                                    float portSize2= 2f*iCS_EditorConfig.PortSize;
                                    if(DragFixPort.IsInputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(!Storage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;                                                
                                            }
                                        }                                    
                                    }
                                    if(DragFixPort.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!Storage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;
                                            }
                                        }
                                    }                                    
                                }
                                break;
                            }
                        }                    
                        if(DragObject.IsStatePort) {
                            DragObject.IsFloating= false;
                            // Get original port state & state chart.
                            iCS_EditorObject origState= Storage.GetParent(DragObject);
                            iCS_EditorObject origStateChart= Storage.GetParent(origState);
                            while(origStateChart != null && !origStateChart.IsStateChart) {
                                origStateChart= Storage.GetParent(origStateChart);
                            }
                            // Get new drag port state & state chart.
                            Rect dragObjRect= Storage.GetPosition(DragObject);
                            Vector2 dragObjPos= new Vector2(dragObjRect.x, dragObjRect.y);
                            iCS_EditorObject newState= GetStateAt(dragObjPos);
                            iCS_EditorObject newStateChart= null;
                            if(newState != null) {
                                newStateChart= Storage.GetParent(newState);
                                while(newStateChart != null && !newStateChart.IsStateChart) {
                                    newStateChart= Storage.GetParent(newStateChart);
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
                                    Storage.DestroyInstance(DragObject);
                                } else {
                                    DragObject.LocalPosition.x= DragStartPosition.x;
                                    DragObject.LocalPosition.y= DragStartPosition.y;                                    
                                }
                                break;
                            }
                            // Relocate transition to the new state.
                            Storage.SetParent(DragObject, newState);
                            iCS_EditorObject transitionModule= Storage.GetTransitionModule(DragObject);
                            iCS_EditorObject otherStatePort= DragObject.IsInputPort ? Storage.GetOutStatePort(transitionModule) : Storage.GetInStatePort(transitionModule);
                            iCS_EditorObject otherState= Storage.GetParent(otherStatePort);
                            iCS_EditorObject moduleParent= Storage.GetParent(transitionModule);
                            iCS_EditorObject newModuleParent= Storage.GetTransitionParent(newState, otherState);
                            if(moduleParent != newModuleParent) {
                                Storage.SetParent(transitionModule, newModuleParent);
                                Storage.LayoutTransitionModule(transitionModule);
                            }
                            break;
                        }
                    }
                    break;
                case DragTypeEnum.TransitionCreation:
                    iCS_EditorObject destState= GetNodeAtMousePosition();
                    if(destState != null && destState.IsState) {
                        iCS_EditorObject outStatePort= Storage[DragObject.Source];
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
        DragType        = DragTypeEnum.None;
        DragObject      = null;
        DragOriginalPort= null;
        DragFixPort     = null;
        IsDragEnabled   = false;
        Storage.CleanupDeadPorts= true;                    
    }
#endregion User Interaction
    
	// ----------------------------------------------------------------------
    // Manages the object selection.
    iCS_EditorObject DetermineSelectedObject() {
        // Object selection is performed on left mouse button only.
        iCS_EditorObject newSelected= GetObjectAtMousePosition();
        SelectedObject= newSelected;
        return SelectedObject;
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(MousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(MousePosition);
        return Storage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= Storage.GetPortAt(graphPosition);
        if(port != null) {
            if(Storage.IsMinimized(port)) return Storage.GetParent(port);
            return port;
        }
        iCS_EditorObject node= Storage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
        
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject dragPort) {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= Storage.GetOverlappingPort(dragPort);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        
        // Destroy drag port since it is not needed anymore.
        Storage.DestroyInstance(dragPort);
        dragPort= null;
        
        // Connect function & modules ports together.
        iCS_EditorObject inPort = null;
        iCS_EditorObject outPort= null;

        iCS_EditorObject portParent= Storage.EditorObjects[fixPort.ParentId];
        iCS_EditorObject overlappingPortParent= Storage.EditorObjects[overlappingPort.ParentId];
        bool portIsChildOfOverlapping= Storage.IsChildOf(portParent, overlappingPortParent);
        bool overlappingIsChildOfPort= Storage.IsChildOf(overlappingPortParent, portParent);
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
                Debug.LogWarning("Cannot connect nested node ports from input to output !!!");
                return true;
            }
        } else {
            inPort = fixPort.IsInputPort          ? fixPort : overlappingPort;
            outPort= overlappingPort.IsOutputPort ? overlappingPort : fixPort;
        }
        if(inPort != outPort) {
			Type inType= inPort.RuntimeType;
			Type outType= outPort.RuntimeType;
            if(iCS_Types.CanBeConnectedWithoutConversion(outType, inType)) { // No conversion needed.
                SetNewDataConnection(inPort, outPort);
            }
            else {  // A conversion is required.
				if(iCS_Types.CanBeConnectedWithUpConversion(outType, inType)) {
					if(EditorUtility.DisplayDialog("Up Conversion Connection", "Are you sure you want to generate a conversion from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)+"?", "Generate Conversion", "Abort")) {
						SetNewDataConnection(inPort, outPort);							
					}
				} else {
                    iCS_ReflectionDesc conversion= iCS_DataBase.FindConversion(outType, inType);
                    if(conversion == null) {
						ShowNotification(new GUIContent("No direct conversion exists from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)));
                    } else {
                        SetNewDataConnection(inPort, outPort, conversion);
                    }						
				}
            }
        } else {
            Debug.LogWarning("Ports are both either inputs or outputs !!!");
        }
        return true;
    }
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionDesc conversion= null) {
        iCS_EditorObject inNode= Storage.GetParent(inPort);
        iCS_EditorObject outNode= Storage.GetParent(outPort);
        iCS_EditorObject inParent= GetParentModule(inNode);
        iCS_EditorObject outParent= GetParentModule(outNode);
        // No need to create module ports if both connected nodes are under the same parent.
        if(inParent == outParent || inParent == outNode || inNode == outParent) {
            Storage.SetSource(inPort, outPort, conversion);
            return;
        }
        // Create inPort if inParent is not part of the outParent hierarchy.
        bool inParentSeen= false;
        for(iCS_EditorObject op= GetParentModule(outParent); op != null; op= GetParentModule(op)) {
            if(inParent == op) {
                inParentSeen= true;
                break;
            }
        }
        if(!inParentSeen && inParent != null) {
            iCS_EditorObject newPort= Storage.CreatePort(inPort.Name, inParent.InstanceId, inPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            Storage.SetSource(inPort, newPort, conversion);
            SetNewDataConnection(newPort, outPort);
            return;                       
        }
        // Create outPort if outParent is not part of the inParent hierarchy.
        bool outParentSeen= false;
        for(iCS_EditorObject ip= GetParentModule(inParent); ip != null; ip= GetParentModule(ip)) {
            if(outParent == ip) {
                outParentSeen= true;
                break;
            }
        }
        if(!outParentSeen && outParent != null) {
            iCS_EditorObject newPort= Storage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            Storage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        Storage.SetSource(inPort, outPort, conversion);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= Storage.GetNodeAt(point);
        while(node != null && !node.IsState) {
            node= Storage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= Storage.GetNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= Storage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= Storage.GetParent(edObj);
        for(; parentModule != null && !parentModule.IsModule; parentModule= Storage.GetParent(parentModule));
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(Vector2 point, iCS_ObjectTypeEnum objType, string objName) {
        iCS_EditorObject newParent= Storage.GetNodeAt(point);
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(objName, objType, newParent, Storage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(iCS_EditorObject node) {
        if(!node.IsNode) return null;
        Vector2 point= Math3D.Middle(Storage.GetPosition(node));
        iCS_EditorObject newParent= Storage.GetNodeAt(point, node);
        if(newParent == Storage.GetParent(node)) return newParent;
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(node.Name, node.ObjectType, newParent, Storage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= Storage.GetParent(node);
        if(newParent == null || newParent == oldParent) return;
        Storage.SetParent(node, newParent);
		if(node.IsState) CleanupEntryState(node, oldParent);
        CleanupConnections(node);
    }
	// ----------------------------------------------------------------------
	void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= Storage.GetParent(state);
		bool anEntryExists= false;
		Storage.ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		Storage.ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			Storage.ForEachChild(prevParent,
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
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;                
            }
            case iCS_ObjectTypeEnum.State: {
                // Attempt to relocate transition modules.
                Storage.ForEachChildPort(node,
                    p=> {
                        if(p.IsStatePort) {
                            iCS_EditorObject transitionModule= null;
                            if(p.IsInStatePort) {
                                transitionModule= Storage.GetParent(Storage.GetSource(p));
                            } else {
                                iCS_EditorObject[] connectedPorts= Storage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= Storage.GetParent(cp);
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= Storage.GetParent(Storage.GetOutStatePort(transitionModule));
                            iCS_EditorObject inState= Storage.GetParent(Storage.GetInStatePort(transitionModule));
                            iCS_EditorObject newParent= Storage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != Storage.GetParent(transitionModule)) {
                                ChangeParent(transitionModule, newParent);
                                Storage.LayoutTransitionModule(transitionModule);
                                Storage.SetDirty(Storage.GetParent(node));
                            }
                        }
                    }
                );
                // Ask our children to cleanup their connections.
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionModule:
            case iCS_ObjectTypeEnum.TransitionGuard:
            case iCS_ObjectTypeEnum.TransitionAction:
            case iCS_ObjectTypeEnum.Module: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                Storage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod:
            case iCS_ObjectTypeEnum.StaticMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.StaticField:
            case iCS_ObjectTypeEnum.Conversion: {
                Storage.ForEachChildPort(node,
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
        iCS_EditorObject sourcePort= Storage.GetDataConnectionSource(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= Storage.GetSource(inPort);
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= Storage.FindConnectedPorts(tmpPort);
            if(connected.Length == 1) {
                iCS_EditorObject t= Storage.GetSource(tmpPort);
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
    iCS_EditorObject[] FindAllConnectedInDataPorts(iCS_EditorObject outPort) {
        List<iCS_EditorObject> allInDataPorts= new List<iCS_EditorObject>();
        FillConnectedInDataPorts(outPort, allInDataPorts);
        return allInDataPorts.ToArray();
    }
    // ----------------------------------------------------------------------
    void FillConnectedInDataPorts(iCS_EditorObject outPort, List<iCS_EditorObject> result) {
        if(outPort == null) return;
        iCS_EditorObject[] connectedPorts= Storage.FindConnectedPorts(outPort);
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
        iCS_EditorObject pasted= Storage.CopyFrom(sourceRoot, new iCS_IStorage(sourceStorage), parent, point);
        Storage.Fold(pasted);
    }

    // ======================================================================
    // Graph Navigation
	// ----------------------------------------------------------------------
    public void CenterOnRoot() {
        CenterOn(DisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOnSelected() {
        CenterOn(SelectedObject ?? DisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOn(iCS_EditorObject obj) {
        if(obj == null || Storage == null) return;
        CenterAt(Math3D.Middle(Storage.GetPosition(obj)));
    }
	// ----------------------------------------------------------------------
    public void CenterAt(Vector2 point) {
        Vector2 newScrollPosition= point-0.5f/Scale*new Vector2(position.width, position.height);
        float distance= Vector2.Distance(ScrollPosition, newScrollPosition);
        float deltaTime= distance/3500f;
        if(deltaTime > 0.5f) deltaTime= 0.5f+(0.5f*(deltaTime-0.5f));
        AnimatedScrollPosition.Start(ScrollPosition, newScrollPosition, deltaTime, (start,end,ratio)=> Math3D.Lerp(start, end, ratio));
        ScrollPosition= newScrollPosition;
    }
    // ======================================================================
    // NODE GRAPH DISPLAY
	// ----------------------------------------------------------------------
    void DrawGrid() {
        Graphics.DrawGrid(position,
                          Storage.Preferences.Grid.BackgroundColor,
                          Storage.Preferences.Grid.GridColor,
                          Storage.Preferences.Grid.GridSpacing);
    }
    
	// ----------------------------------------------------------------------
	static string[] options= new string[5]{"All", "Class", "Function", "Input", "Output"};
	void Heading() {
		// Determine toolbar height.
		float height= iCS_EditorUtility.GetGUIStyleHeight(EditorStyles.toolbar);

//		Debug.Log("Toolbar: "+height);
//		Debug.Log("TextField: "+GetGUIStyleHeight(EditorStyles.toolbarTextField));
//		Debug.Log("Button: "+GetGUIStyleHeight(EditorStyles.toolbarButton));
//		Debug.Log("Popup: "+GetGUIStyleHeight(EditorStyles.toolbarPopup));
//		Debug.Log("DropDown: "+GetGUIStyleHeight(EditorStyles.toolbarDropDown));

		// Fill toolbar with background image.
		Rect r= new Rect(0,-1,position.width, height);
		GUI.Box(r, "", EditorStyles.toolbar);

		// Insert an initial spacer.
		float spacer= 8f;
		r.x+= spacer;
		r.width-= spacer;
		
        // Adjust toolbar styles
		Vector2 test= EditorStyles.toolbar.contentOffset;
		test.y=3f;
		EditorStyles.toolbar.contentOffset= test;
		test= EditorStyles.toolbarTextField.contentOffset;
		test.y=2f;
		EditorStyles.toolbarTextField.contentOffset= test;
		
        // Show mouse coordinates.
        string mouseValue= ViewportToGraph(MousePosition).ToString();
        iCS_EditorUtility.ToolbarLabel(ref r, new GUIContent(mouseValue), 0, 0, true);
        
		// Show zoom control at the end of the toolbar.
        Scale= iCS_EditorUtility.ToolbarSlider(ref r, 120f, Scale, 1f, 0.15f, spacer, spacer, true);
        iCS_EditorUtility.ToolbarLabel(ref r, new GUIContent("Zoom"), 0, 0, true);
		
		// Show current bookmark.
		string bookmarkString= "Bookmark: ";
		if(Bookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= Bookmark.Name;
		}
		iCS_EditorUtility.ToolbarLabel(ref r, 150f, new GUIContent(bookmarkString),0,0,true);
		
		// Editable field test.		
		iCS_EditorUtility.ToolbarButtons(ref r, 400f, 0, options, 0, 0);
		iCS_EditorUtility.ToolbarText(ref r, 100f, "Search", 0, 0);		
	}
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        Storage.Update();
        
		// Start graphics
        Graphics.Begin(UpdateScrollPosition(), Scale, ClipingArea, SelectedObject, ViewportToGraph(MousePosition), Storage);
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw nodes and their connections.
    	DrawNormalNodes();
        DrawConnections();
        DrawMinimizedNodes();           

        Graphics.End();

		// Show header
		Heading();
	}

	// ----------------------------------------------------------------------
	Vector2 UpdateScrollPosition() {
        Vector2 graphicScrollPosition= ScrollPosition;
        if(AnimatedScrollPosition.IsActive) {
            AnimatedScrollPosition.Update();
            graphicScrollPosition= AnimatedScrollPosition.CurrentValue;
        }
		return graphicScrollPosition;
	}
	// ----------------------------------------------------------------------
    void DrawNormalNodes() {
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating && !node.IsBehaviour) {
                	Graphics.DrawNormalNode(node, Storage);                        
                }
            }
        );
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating && !node.IsBehaviour) {
                	Graphics.DrawNormalNode(node, Storage);                        
                }
            }
        );
    }	
	// ----------------------------------------------------------------------
    void DrawMinimizedNodes() {
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating) {
                	Graphics.DrawMinimizedNode(node, Storage);                        
                }
            }
        );
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating) {
                	Graphics.DrawMinimizedNode(node, Storage);                        
                }
            }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawConnection(port, Storage); });

        // Display ports.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawPort(port, Storage); });
    }

}
