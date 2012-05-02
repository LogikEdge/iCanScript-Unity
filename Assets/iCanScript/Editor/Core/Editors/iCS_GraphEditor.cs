using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the iCS_Behaviour.
public class iCS_GraphEditor : iCS_EditorWindow {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    iCS_IStorage         myStorage      = null;
    iCS_EditorObject     myDisplayRoot  = null;
    iCS_DynamicMenu      DynamicMenu    = null;
    
    // ----------------------------------------------------------------------
    int   RefreshCounter= 0;
    float CurrentTime   = 0;
    float DeltaTime     = 0;
    
    // ----------------------------------------------------------------------
    private iCS_Graphics    Graphics  = null;
    
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortConnection, PortRelocation, NodeDrag, TransitionCreation };
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
    iCS_EditorObject Bookmark= null;
	bool			 ShouldRotateMuxPort= false;
    
    // ----------------------------------------------------------------------
    Rect    ClipingArea { get { return new Rect(ScrollPosition.x, ScrollPosition.y, Viewport.width, Viewport.height); }}
    Vector2 ViewportCenter { get { return new Vector2(0.5f/Scale*position.width, 0.5f/Scale*position.height); } }
    Rect    Viewport { get { return new Rect(0,0,position.width/Scale, position.height/Scale); }}
    Vector2 ViewportToGraph(Vector2 v) { return v+ScrollPosition; }
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;
    // ----------------------------------------------------------------------
	static string[] menuOptions= new string[2]{"Normal", "Expert"};
    int MenuOption= 0;
     
    // ======================================================================
    // Accessors
	// ----------------------------------------------------------------------
    iCS_EditorObject StorageRoot {
        get {
            if(myStorage == null || Prelude.length(myStorage.EditorObjects) == 0) return null;
            return myStorage.EditorObjects[0];
        }
    }
	// ----------------------------------------------------------------------
    new iCS_EditorObject SelectedObject {
        get { return myStorage.SelectedObject; }
        set { myStorage.SelectedObject= value; }
    }
	// ----------------------------------------------------------------------
    Vector2     ScrollPosition { get { return myStorage.ScrollPosition; } set { myStorage.ScrollPosition= value; }}
    float       Scale {
        get { return myStorage.GuiScale; }
        set {
            if(value > 1f) value= 1f;
            if(value < 0.15f) value= 0.15f;
            myStorage.GuiScale= value;
        }
    }
    Prelude.Animate<Vector2>    AnimatedScrollPosition= new Prelude.Animate<Vector2>();

	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus    { get { return focusedWindow == this; }}
    bool    IsFloatingKeyDown	{ get { return Event.current.control; }}
    bool    IsCopyKeyDown       { get { return Event.current.shift; }}
    bool    IsScaleKeyDown      { get { return Event.current.alt; }}
    bool    IsShiftKeyDown      { get { return Event.current.shift; }}
    
	// ----------------------------------------------------------------------
	void UpdateMouse() {
        myMousePosition= Event.current.mousePosition;
        if(Event.current.type == EventType.MouseDrag) myMousePosition+= Event.current.delta;
	}
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
	public void OnEnable() {        
		// Tell Unity we want to be informed of move drag events
		wantsMouseMove= true;

        // Create worker objects.
        Graphics        = new iCS_Graphics();
        DynamicMenu     = new iCS_DynamicMenu();

        // Inspect the assemblies for components.
        if(!ourAlreadyParsed) {
            ourAlreadyParsed= true;
            iCS_Reflection.ParseAppDomain();
        }
        
//        // Register to receive storage selection changes.
//        iCS_StorageMgr.Register(SetStorage);
//
        // Get snapshot for realtime clock.
        CurrentTime= Time.realtimeSinceStartup;	    
	}

	// ----------------------------------------------------------------------
    // Releases all resources used by the iCS_Behaviour editor.
    public void OnDisable() {
        // Release all worker objects.
        Graphics    = null;
        DynamicMenu = null;
//        // Unregister storage selection notification.
//        iCS_StorageMgr.Unregister(SetStorage);
    }
    
//    // ----------------------------------------------------------------------
//    // Activates the editor and initializes all Graph shared variables.
//	public void ActivateLicense() {
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
//
//    }
//    

	// ----------------------------------------------------------------------
    protected virtual iCS_ClassWizard GetClassWizard()  { return GetWindow(typeof(iCS_ClassWizard)) as iCS_ClassWizard; }
    protected virtual void            InvokeInstaller() {}
	
	// ----------------------------------------------------------------------
    // Assures proper initialization and returns true if editor is ready
    // to execute.
	public bool IsInitialized() {
        // Nothing to do if we don't have a Graph to edit...
		if(myStorage == null) { return false; }
        
		// Don't run if graphic sub-system did not initialise.
		if(iCS_Graphics.IsInitialized == false) {
            iCS_Graphics.Init(myStorage);
			return false;
		}
        return true;
	}


    // ======================================================================
    // UPDATE FUNCTIONALITY
	// ----------------------------------------------------------------------
	public void Update() {
        // Update storage selection.
        iCS_EditorMgr.Update();
		myStorage= iCS_StorageMgr.IStorage;
		myDisplayRoot= myStorage != null ? myStorage[0] : null;
        // Determine repaint rate.
        if(myStorage != null) {
            // Repaint window
            if(myStorage.IsDirty || myStorage.IsAnimationPlaying || AnimatedScrollPosition.IsActive) {
                myStorage.IsAnimationPlaying= false;
                Repaint();
            }
            float refreshFactor= (Application.isPlaying || mouseOverWindow == this ? 8f : 1f);
            int newRefreshCounter= (int)(Time.realtimeSinceStartup*refreshFactor);
            if(newRefreshCounter != RefreshCounter) {
                RefreshCounter= newRefreshCounter;
                Repaint();
            }
            // Update DisplayRoot
            if(myDisplayRoot == null && myStorage.IsValid(0)) {
                myDisplayRoot= myStorage[0];
            }
        }
        // Make certain the installer is ran.
        if(iCS_Reflection.NeedToRunInstaller) {
            InvokeInstaller();
//            StreamWriter stream= new StreamWriter("database.txt");
//            List<iCS_ReflectionDesc> menu= iCS_DataBase.BuildExpertMenu();
//            foreach(var desc in menu) {
//                stream.Write(desc.FunctionPath+"/"+desc.FunctionSignature+"\n");
//            }
//            stream.Close();
        }
        // Cleanup objects.
        iCS_AutoReleasePool.Update();
	}
	
	// ----------------------------------------------------------------------
    public void OnSelectionChanged() {
        Update();
    }
    
	// ----------------------------------------------------------------------
	// User GUI function.
//    static int frameCount= 0;
//    static int seconds= 0;
	public void OnGUI() {
		// Don't do start editor if not properly initialized.
		if( !IsInitialized() ) return;
       	
        // Update GUI time.
        DeltaTime= Time.realtimeSinceStartup-CurrentTime;
        CurrentTime= Time.realtimeSinceStartup;
        
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
                SelectedObjectBeforeMouseDown= SelectedObject;
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
                float mouseUpDeltaTime= CurrentTime-MouseUpTime;
                MouseUpTime= CurrentTime;
                if(IsDragStarted) {
                    EndDrag();
                } else {
					if(ShouldRotateMuxPort) RotateSelectedMuxPort();
                    if(SelectedObject != null) {
                        // Process fold/unfold/minimize/maximize click.
                        Vector2 mouseGraphPos= MouseGraphPosition;
                        if(Graphics.IsFoldIconPicked(SelectedObject, mouseGraphPos, myStorage)) {
                            if(myStorage.IsFolded(SelectedObject)) {
                                myStorage.RegisterUndo("Unfold");
                                myStorage.Maximize(SelectedObject);
                            } else {
                                myStorage.RegisterUndo("Fold");
                                myStorage.Fold(SelectedObject);
                            }
                        } else if(Graphics.IsMinimizeIconPicked(SelectedObject, mouseGraphPos, myStorage)) {
                            myStorage.RegisterUndo("Minimize");
                            myStorage.Minimize(SelectedObject);
                        } else {
                            if(SelectedObject == SelectedObjectBeforeMouseDown && mouseUpDeltaTime < 0.25f) {
                                ProcessNodeDisplayOptionEvent();                                
                            }
                        }
                    }                                                
                }
                Event.current.Use();
                break;
            }
            case EventType.ScrollWheel: {
                Vector2 delta= Event.current.delta;
                if(IsScaleKeyDown) {
                    Vector2 pivot= MouseGraphPosition;
					float zoomDirection= myStorage.Preferences.ControlOptions.InverseZoom ? -1f : 1f;
                    Scale= Scale+(delta.y > 0 ? -0.05f : 0.05f)*zoomDirection;
                    Vector2 offset= pivot-ViewportToGraph(MousePosition);
                    ScrollPosition+= offset;
                } else {
                    delta*= myStorage.Preferences.ControlOptions.ScrollSpeed*(1f/Scale); 
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
			    if(!HasKeyboardFocus) break;
				var ev= Event.current;
				if(ev.keyCode == KeyCode.None) break;
                switch(ev.keyCode) {
                    // Tree navigation
                    case KeyCode.UpArrow: {
                        if(SelectedObject != null) {
                            SelectedObject= myStorage.GetParent(SelectedObject);
                            CenterOnSelected();
                        } 
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.DownArrow: {
                        if(SelectedObject == null) SelectedObject= myDisplayRoot;
                        SelectedObject= iCS_EditorUtility.GetFirstChild(SelectedObject, myStorage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.RightArrow: {
                        SelectedObject= iCS_EditorUtility.GetNextSibling(SelectedObject, myStorage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.LeftArrow: {
                        SelectedObject= iCS_EditorUtility.GetPreviousSibling(SelectedObject, myStorage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.F: {
                        if(ev.shift) {
                            CenterOn(myDisplayRoot);
                        } else {
                            CenterOn(SelectedObject);
                        }
                        Event.current.Use();
                        break;
                    }
                    // Fold/Minimize/Maximize.
                    case KeyCode.Return: {
                        ProcessNodeDisplayOptionEvent();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.H: {  // Show Help
                        if(SelectedObject != null) {
                            Help.ShowHelpPage("file:///unity/ScriptReference/index.html");
                        }
                        Event.current.Use();
                        break;
                    }
                    // Bookmarks
                    case KeyCode.B: {  // Bookmark selected object
                        if(SelectedObject != null) {
                            Bookmark= SelectedObject;                            
                        }
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.G: {  // Goto bookmark
                        if(Bookmark != null) {
                            SelectedObject= Bookmark;
                            CenterOnSelected();
                        }
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.S: {  // Switch bookmark and selected object
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
                    case KeyCode.C: {  // Connect bookmark and selected port.
                        if(Bookmark != null && Bookmark.IsDataPort && SelectedObject != null && SelectedObject.IsDataPort) {
                            VerifyNewConnection(Bookmark, SelectedObject);
                        }
                        Event.current.Use();
                        break;
                    }
                    // Object deletion
                    case KeyCode.Delete:
                    case KeyCode.Backspace: {
                        if(SelectedObject != null && SelectedObject != myDisplayRoot && SelectedObject != StorageRoot &&
                          !SelectedObject.IsTransitionAction && !SelectedObject.IsTransitionGuard) {
                            iCS_EditorObject parent= myStorage.GetParent(SelectedObject);
                            if(ev.shift) {
                                myStorage.RegisterUndo("Delete");
                                myStorage.DestroyInstance(SelectedObject.InstanceId);                                                        
                            } else {
                                iCS_EditorUtility.DestroyObject(SelectedObject, myStorage);
                            }
                            SelectedObject= parent;
                        }
                        Event.current.Use();
                        break;
                    }
                    // Object creation.
                    case KeyCode.KeypadEnter: // fnc+return on Mac
                    case KeyCode.Insert: {
                        if(SelectedObject == null) SelectedObject= myDisplayRoot;
                        // Don't use mouse position if it is too far from selected node.
                        Vector2 graphPos= ViewportToGraph(MousePosition);
                        Rect parentRect= myStorage.GetPosition(SelectedObject);
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
                            if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.Update, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                myStorage.RegisterUndo("Create Update");
                                newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.Update);  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.LateUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                myStorage.RegisterUndo("Create LateUpdate");
                                newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.LateUpdate);                                  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.FixedUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                myStorage.RegisterUndo("Create FixedUpdate");
                                newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.FixedUpdate);                                  
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
                                myStorage.RegisterUndo("Create Module");
                                newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, null);                                
                            } else {
                                myStorage.RegisterUndo("Create State Chart");
                                newObj= myStorage.CreateStateChart(SelectedObject.InstanceId, graphPos, null);
                            }
                            if(ev.control && newObj != null) {
                                SelectedObject= newObj;
                            }
                            Event.current.Use();
                            break;
                        }
                        // Auto-insert on state chart.
                        if(SelectedObject.IsStateChart) {
                            myStorage.RegisterUndo("Create State");
                            iCS_EditorObject newObj= myStorage.CreateState(SelectedObject.InstanceId, graphPos);
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
                                if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                    myStorage.RegisterUndo("Create OnUpdate");
                                    newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnUpdate);  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnEntry, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                    myStorage.RegisterUndo("Create OnEntry");
                                    newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnEntry);                                  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnExit, iCS_ObjectTypeEnum.Module, SelectedObject, myStorage)) {
                                    myStorage.RegisterUndo("Create OnExit");
                                    newObj= myStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnExit);                                  
                                }                                
                            } else {
                                myStorage.RegisterUndo("Create State");
                                newObj= myStorage.CreateState(SelectedObject.InstanceId, graphPos);
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
                    // Class module shortcuts.
                    case KeyCode.D: {
                        if(SelectedObject.IsClassModule) {
                            myStorage.ClassModuleCreateInputInstanceFields(SelectedObject);
                            myStorage.ClassModuleDestroyOutputInstanceFields(SelectedObject);
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
    void ProcessNodeDisplayOptionEvent() {
        if(SelectedObject != null && SelectedObject.IsNode) {
            if(SelectedObject.IsMinimized) {
                myStorage.RegisterUndo("Unfold");
                myStorage.Fold(SelectedObject);
            } else if(SelectedObject.IsMaximized) {
                myStorage.RegisterUndo("Fold");
                myStorage.Fold(SelectedObject);
            } else {
                if(IsShiftKeyDown) {
                    myStorage.RegisterUndo("Minimize");
                    myStorage.Minimize(SelectedObject);
                } else {
                    myStorage.RegisterUndo("Maximize");
                    myStorage.Maximize(SelectedObject);                                                        
                }
            }
        }        
    }

	// ----------------------------------------------------------------------
    void ShowDynamicMenu() {
        if(SelectedObject == null && myDisplayRoot.IsBehaviour) {
            SelectedObject= myDisplayRoot;
        }
        ShowClassWizard();
        DynamicMenu.Update(SelectedObject, myStorage, ViewportToGraph(MousePosition), MenuOption == 0);
        myStorage.SetDirty(SelectedObject);                    
    }
	// ----------------------------------------------------------------------
    void ShowClassWizard() {
        if(SelectedObject != null && SelectedObject.IsClassModule) {
            bool hadKeyboardFocus= HasKeyboardFocus;
            iCS_EditorMgr.GetClassWizardEditor();
            // Keep keyboard focus.
            if(hadKeyboardFocus) Focus();
        }        
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
    void MakeDataConnectionDrag() {
        if(DragFixPort != DragOriginalPort) myStorage.SetSource(DragOriginalPort, DragFixPort);
        myStorage.SetSource(DragObject, DragOriginalPort);
        DragFixPort= DragOriginalPort;
    }
	// ----------------------------------------------------------------------
    void BreakDataConnectionDrag() {
        var originalSource= myStorage.GetSource(DragOriginalPort);
        if(originalSource != null && originalSource != DragObject) {
            DragFixPort= originalSource;
            myStorage.SetSource(DragObject, DragFixPort);
            myStorage.SetSource(DragOriginalPort, null);
        } else {
            if(DragFixPort == DragOriginalPort) {
                myStorage.SetSource(DragFixPort, DragObject);
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
        if(port != null && port.IsPort && !myStorage.IsMinimized(port) && !port.IsTransitionPort) {
            myStorage.RegisterUndo("Port Drag");
            myStorage.CleanupDeadPorts= false;
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
        if(node != null && node.IsNode && (node.IsMinimized || !node.IsState || Graphics.IsNodeTitleBarPicked(node, pos, myStorage))) {
            if(IsCopyKeyDown) {
                GameObject go= new GameObject(node.Name);
                go.hideFlags = HideFlags.HideAndDontSave;
                go.AddComponent("iCS_Library");
                iCS_Library library= go.GetComponent<iCS_Library>();
                iCS_IStorage iStorage= new iCS_IStorage(library);
                iStorage.CopyFrom(node, myStorage, null, Vector2.zero);
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences= new UnityEngine.Object[1]{go};
                DragAndDrop.StartDrag(node.Name);
                iCS_AutoReleasePool.AutoRelease(go, 60f);
                // Disable dragging.
                IsDragEnabled= false;
                DragType= DragTypeEnum.None;
            } else {
                myStorage.RegisterUndo("Node Drag");
                node.IsFloating= IsFloatingKeyDown;
                DragType= DragTypeEnum.NodeDrag;
                DragObject= node;
                Rect nodePos= myStorage.GetPosition(node);
                DragStartPosition= new Vector2(nodePos.x, nodePos.y);                                                                    
            }
            return true;
        }
        
        // New state transition drag.
        if(node != null && node.IsState) {
            myStorage.RegisterUndo("Transition Creation");
            DragType= DragTypeEnum.TransitionCreation;
            iCS_EditorObject outTransition= myStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.OutStatePort);
            iCS_EditorObject inTransition= myStorage.CreatePort("[false]", node.InstanceId, typeof(void), iCS_ObjectTypeEnum.InStatePort);
            myStorage.SetInitialPosition(outTransition, pos);
            myStorage.SetInitialPosition(inTransition, pos);
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
                myStorage.MoveTo(node, DragStartPosition+delta);
                myStorage.SetDirty(node);                        
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
                iCS_EditorObject parent= myStorage.GetParentNode(DragOriginalPort);
                if(!myStorage.IsNearParentEdge(DragObject)) {
					CreateDragPort();
                } else {
                    myStorage.PositionOnEdge(DragObject);
                    myStorage.LayoutPorts(parent); 
                }
                break;
            }
            case DragTypeEnum.PortConnection: {
                // Update port position.
                Vector2 newLocalPos= DragStartPosition+delta;
                DragObject.LocalPosition.x= newLocalPos.x;
                DragObject.LocalPosition.y= newLocalPos.y;
                // Determine if we should go back to port relocation.
                if(!DragOriginalPort.IsInMuxPort && myStorage.IsNearParentEdge(DragObject, DragOriginalPort.Edge)) {
                    iCS_EditorObject dragObjectSource= myStorage.GetSource(DragObject);
                    if(dragObjectSource != DragOriginalPort) {
                        myStorage.SetSource(DragOriginalPort, dragObjectSource);
                    }
                    myStorage.DestroyInstance(DragObject);
                    DragObject= DragOriginalPort;
                    DragFixPort= DragOriginalPort;
                    DragObject.IsFloating= true;
                    DragType= DragTypeEnum.PortRelocation;
                    break;
                }
                // Snap to nearby ports
                Vector2 mousePosInGraph= ViewportToGraph(MousePosition);
                iCS_EditorObject closestPort= myStorage.GetClosestPortAt(mousePosInGraph, p=> p.IsDataPort);
                if(closestPort != null && (closestPort.ParentId != DragOriginalPort.ParentId || closestPort.Edge != DragOriginalPort.Edge)) {
                    Rect closestPortRect= myStorage.GetPosition(closestPort);
                    Vector2 closestPortPos= new Vector2(closestPortRect.x, closestPortRect.y);
                    if(Vector2.Distance(closestPortPos, mousePosInGraph) < 4f*iCS_Config.PortRadius) {
                        Rect parentPos= myStorage.GetPosition(myStorage.GetParent(DragObject));
                        DragObject.LocalPosition.x= closestPortRect.x-parentPos.x;
                        DragObject.LocalPosition.y= closestPortRect.y-parentPos.y;
                    }                    
                }
                // Special case for module ports.
                if(DragOriginalPort.IsModulePort) {
                    if(myStorage.IsInside(myStorage.GetParent(DragOriginalPort), mousePosInGraph)) {
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
                    iCS_EditorObject oldParent= myStorage.GetParent(node);
                    if(oldParent != null) {
                        iCS_EditorObject newParent= GetValidParentNodeUnder(node);
                        if(newParent != null) {
                            if(newParent != oldParent) {
                                ChangeParent(node, newParent);
                            }
                        } else {
                            myStorage.MoveTo(node, DragStartPosition);
                        }
                        myStorage.SetDirty(oldParent);                        
                    }
                    node.IsFloating= false;
                    break;
                }
                case DragTypeEnum.PortRelocation:
                    DragObject.IsFloating= false;
                    if(DragObject.IsDataPort) {
                        myStorage.LayoutPorts(myStorage.GetParent(DragObject));
                        break;
                    }
                    if(DragObject.IsStatePort) {
                        // Get original port state & state chart.
                        iCS_EditorObject origState= myStorage.GetParent(DragObject);
                        iCS_EditorObject origStateChart= myStorage.GetParent(origState);
                        while(origStateChart != null && !origStateChart.IsStateChart) {
                            origStateChart= myStorage.GetParent(origStateChart);
                        }
                        // Get new drag port state & state chart.
                        Rect dragObjRect= myStorage.GetPosition(DragObject);
                        Vector2 dragObjPos= new Vector2(dragObjRect.x, dragObjRect.y);
                        iCS_EditorObject newState= GetStateAt(dragObjPos);
                        iCS_EditorObject newStateChart= null;
                        if(newState != null) {
                            newStateChart= myStorage.GetParent(newState);
                            while(newStateChart != null && !newStateChart.IsStateChart) {
                                newStateChart= myStorage.GetParent(newStateChart);
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
                                myStorage.DestroyInstance(DragObject);
                            } else {
                                DragObject.LocalPosition.x= DragStartPosition.x;
                                DragObject.LocalPosition.y= DragStartPosition.y;                                    
                            }
                            break;
                        }
                        // Relocate transition to the new state.
                        myStorage.SetParent(DragObject, newState);
                        iCS_EditorObject transitionModule= myStorage.GetTransitionModule(DragObject);
                        iCS_EditorObject otherStatePort= DragObject.IsInputPort ? myStorage.GetOutStatePort(transitionModule) : myStorage.GetInStatePort(transitionModule);
                        iCS_EditorObject otherState= myStorage.GetParent(otherStatePort);
                        iCS_EditorObject moduleParent= myStorage.GetParent(transitionModule);
                        iCS_EditorObject newModuleParent= myStorage.GetTransitionParent(newState, otherState);
                        if(moduleParent != newModuleParent) {
                            myStorage.SetParent(transitionModule, newModuleParent);
                            myStorage.LayoutTransitionModule(transitionModule);
                        }
                        break;
                    }
                    break;
                case DragTypeEnum.PortConnection:
                    // Verify for a new connection.
                    if(!VerifyNewDragConnection(DragFixPort, DragObject)) {
                        bool isNearParent= myStorage.IsNearParent(DragObject);
                        if(DragFixPort.IsDataPort) {
                            // We don't need the drag port anymore.
                            Rect dragPortPos= myStorage.GetPosition(DragObject);
                            myStorage.DestroyInstance(DragObject);
                            // Verify for disconnection.
                            if(!isNearParent) {
                                // Let's determine if we want to create a module port.
                                iCS_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent == null) break;
                                if(newPortParent.IsModule) {
                                    iCS_EditorObject portParent= myStorage.GetParent(DragFixPort);
                                    Rect modulePos= myStorage.GetPosition(newPortParent);
                                    float portSize2= 2f*iCS_Config.PortSize;
                                    if(DragFixPort.IsInputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(myStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= myStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(!myStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= myStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(DragFixPort, newPort);
                                                break;                                                
                                            }
                                        }                                    
                                    }
                                    if(DragFixPort.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(myStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= myStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(dragPortPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!myStorage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= myStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                SetNewDataConnection(newPort, DragFixPort);
                                                break;
                                            }
                                        }
                                    }                                    
                                }
                                if(DragFixPort.IsOutputPort && (newPortParent.IsState || newPortParent.IsStateChart)) {
									if(myStorage.IsNearNodeEdge(newPortParent, Math3D.ToVector2(dragPortPos), iCS_EditorObject.EdgeEnum.Right)) {
	                                    iCS_EditorObject newPort= myStorage.CreatePort(DragFixPort.Name, newPortParent.InstanceId, DragFixPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
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
                        iCS_EditorObject outStatePort= myStorage[DragObject.Source];
                        outStatePort.IsFloating= false;
                        myStorage.CreateTransition(outStatePort, destState);
                        DragObject.Source= -1;
                        myStorage.DestroyInstance(DragObject);
                    } else {
                        myStorage.DestroyInstance(DragObject.Source);
                        myStorage.DestroyInstance(DragObject);
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
        myStorage.CleanupDeadPorts= true;                    
    }
#endregion User Interaction
    
	// ----------------------------------------------------------------------
	void CreateDragPort() {
        // Data port. Create a drag port as appropriate.
        iCS_EditorObject parent= myStorage.GetParentNode(DragOriginalPort);
        DragObject.IsFloating= false;
        if(DragOriginalPort.IsInputPort) {
            DragObject= myStorage.CreatePort(DragOriginalPort.Name, parent.InstanceId, DragOriginalPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            iCS_EditorObject prevSource= myStorage.GetSource(DragOriginalPort);
            if(prevSource != null) {
                DragFixPort= prevSource;
                myStorage.SetSource(DragObject, DragFixPort);
                myStorage.SetSource(DragOriginalPort, null);
                DragObject.Name= DragFixPort.Name;
            } else {
                DragFixPort= DragOriginalPort;
                myStorage.SetSource(DragFixPort, DragObject);
            }                    
        } else {
            DragObject= myStorage.CreatePort(DragOriginalPort.Name, parent.InstanceId, DragOriginalPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            DragFixPort= DragOriginalPort;
            myStorage.SetSource(DragObject, DragOriginalPort);
        }
        DragType= DragTypeEnum.PortConnection;
        Rect portPos= myStorage.GetPosition(DragOriginalPort);
        myStorage.SetInitialPosition(DragObject, new Vector2(portPos.x, portPos.y));
        myStorage.SetDisplayPosition(DragObject, portPos);
		Rect parentPos= myStorage.GetPosition(parent);
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
		if(SelectedObject != null && newSelected != null && newSelected.IsOutMuxPort && myStorage.GetOutMuxPort(SelectedObject) == newSelected) {
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
    public iCS_EditorObject GetObjectAtMousePosition() {
        return GetObjectAtScreenPosition(MousePosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetNodeAtMousePosition() {
        Vector2 graphPosition= ViewportToGraph(MousePosition);
        return myStorage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ViewportToGraph(_screenPos);
        iCS_EditorObject port= myStorage.GetPortAt(graphPosition);
        if(port != null) {
            if(myStorage.IsMinimized(port)) return myStorage.GetParent(port);
            return port;
        }
        iCS_EditorObject node= myStorage.GetNodeAt(graphPosition);                
        if(node != null) return node;
        return null;
    }
	// ----------------------------------------------------------------------
    void RotateSelectedMuxPort() {
		if(SelectedObject == null || !SelectedObject.IsDataPort) return;
		if(SelectedObject.IsOutMuxPort) {
			myStorage.ForEachChild(SelectedObject, 
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
		iCS_EditorObject muxPort= myStorage.GetParent(SelectedObject);
		if(!muxPort.IsDataPort) return;
		bool takeNext= false;
		bool found= myStorage.ForEachChild(muxPort,
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
        iCS_EditorObject overlappingPort= myStorage.GetOverlappingPort(dragPort);
        if(overlappingPort == null) return false;

        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        // Destroy drag port since it is not needed anymore.
        myStorage.DestroyInstance(dragPort);
        dragPort= null;
        return VerifyNewConnection(fixPort, overlappingPort);
    }
	// ----------------------------------------------------------------------
    bool VerifyNewConnection(iCS_EditorObject fixPort, iCS_EditorObject overlappingPort) {
        // Only data ports can be connected together.
        if(!fixPort.IsDataPort || !overlappingPort.IsDataPort) return false;
        iCS_EditorObject portParent= myStorage.GetParent(fixPort);
        iCS_EditorObject overlappingPortParent= myStorage.GetParent(overlappingPort);
        if(overlappingPort.IsOutputPort && (overlappingPortParent.IsState || overlappingPortParent.IsStateChart)) {
			CreateStateMux(fixPort, overlappingPort);
			return true;
		}
        
        // Connect function & modules ports together.
        iCS_EditorObject inPort = null;
        iCS_EditorObject outPort= null;

        bool portIsChildOfOverlapping= myStorage.IsChildOf(portParent, overlappingPortParent);
        bool overlappingIsChildOfPort= myStorage.IsChildOf(overlappingPortParent, portParent);
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
                ShowNotification(new GUIContent("Cannot connect nested node ports from input to output !!!"));
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
            ShowNotification(new GUIContent("Cannot connect an "+direction+" port to an "+direction+" port !!!"));
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
			ShowNotification(new GUIContent("No automatic type conversion exists from "+iCS_Types.TypeName(outType)+" to "+iCS_Types.TypeName(inType)));
            return false;
        }
        return true;
    }
	// ----------------------------------------------------------------------
	void CreateStateMux(iCS_EditorObject fixPort, iCS_EditorObject stateMuxPort) {
        iCS_ReflectionDesc conversion= null;
        if(!VerifyConnectionTypes(stateMuxPort, fixPort, out conversion)) return;
		var source= myStorage.GetSource(stateMuxPort);
		// Simply connect a disconnected mux state port.
		if(source == null && myStorage.NbOfChildren(stateMuxPort, c=> c.IsDataPort) == 0) {
			SetNewDataConnection(stateMuxPort, fixPort, conversion);
			return;
		}
		// Convert source port to child port.
		if(source != null) {
			stateMuxPort.ObjectType= iCS_ObjectTypeEnum.OutMuxPort;
			var firstMuxInput= myStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
			myStorage.SetSource(firstMuxInput, source);
			myStorage.SetSource(stateMuxPort, null);
		}
		// Create new mux input port.
		var inMuxPort= myStorage.CreatePort(fixPort.Name, stateMuxPort.InstanceId, stateMuxPort.RuntimeType, iCS_ObjectTypeEnum.InMuxPort);
		SetNewDataConnection(inMuxPort, fixPort, conversion);
	}
	// ----------------------------------------------------------------------
    void SetNewDataConnection(iCS_EditorObject inPort, iCS_EditorObject outPort, iCS_ReflectionDesc conversion= null) {
        iCS_EditorObject inNode= myStorage.GetParent(inPort);
        iCS_EditorObject outNode= myStorage.GetParent(outPort);
        iCS_EditorObject inParent= GetParentNode(inNode);
        iCS_EditorObject outParent= GetParentNode(outNode);
        // No need to create module ports if both connected nodes are under the same parent.
        if(inParent == outParent || inParent == outNode || inNode == outParent) {
            myStorage.SetSource(inPort, outPort, conversion);
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
            iCS_EditorObject newPort= myStorage.CreatePort(outPort.Name, inParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
            myStorage.SetSource(inPort, newPort, conversion);
            SetNewDataConnection(newPort, outPort);
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
            iCS_EditorObject newPort= myStorage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            myStorage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        myStorage.SetSource(inPort, outPort, conversion);
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateAt(Vector2 point) {
        iCS_EditorObject node= myStorage.GetNodeAt(point);
        while(node != null && !node.IsState) {
            node= myStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetStateChartAt(Vector2 point) {
        iCS_EditorObject node= myStorage.GetNodeAt(point);
        while(node != null && !node.IsStateChart) {
            node= myStorage.GetNodeAt(point, node);
        }
        return node;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentModule(iCS_EditorObject edObj) {
        iCS_EditorObject parentModule= myStorage.GetParent(edObj);
        for(; parentModule != null && !parentModule.IsModule; parentModule= myStorage.GetParent(parentModule));
        return parentModule;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetParentNode(iCS_EditorObject edObj) {
        iCS_EditorObject parentNode= myStorage.GetParent(edObj);
        for(; parentNode != null && !parentNode.IsNode; parentNode= myStorage.GetParent(parentNode));
        return parentNode;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(Vector2 point, iCS_ObjectTypeEnum objType, string objName) {
        iCS_EditorObject newParent= myStorage.GetNodeAt(point);
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(objName, objType, newParent, myStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    iCS_EditorObject GetValidParentNodeUnder(iCS_EditorObject node) {
        if(!node.IsNode) return null;
        Vector2 point= Math3D.Middle(myStorage.GetPosition(node));
        iCS_EditorObject newParent= myStorage.GetNodeAt(point, node);
        if(newParent == myStorage.GetParent(node)) return newParent;
        if(newParent != null && !iCS_AllowedChildren.CanAddChildNode(node.Name, node.ObjectType, newParent, myStorage)) {
            newParent= null;
        }
        return newParent;
    }
	// ----------------------------------------------------------------------
    void ChangeParent(iCS_EditorObject node, iCS_EditorObject newParent) {
        iCS_EditorObject oldParent= myStorage.GetParent(node);
        if(newParent == null || newParent == oldParent) return;
        myStorage.SetParent(node, newParent);
		if(node.IsState) CleanupEntryState(node, oldParent);
        CleanupConnections(node);
    }
	// ----------------------------------------------------------------------
	void CleanupEntryState(iCS_EditorObject state, iCS_EditorObject prevParent) {
		state.IsEntryState= false;
		iCS_EditorObject newParent= myStorage.GetParent(state);
		bool anEntryExists= false;
		myStorage.ForEachChild(newParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) state.IsEntryState= true;
		anEntryExists= false;
		myStorage.ForEachChild(prevParent, child=> { if(child.IsEntryState) anEntryExists= true; });
		if(!anEntryExists) {
			myStorage.ForEachChild(prevParent,
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
                myStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;                
            }
            case iCS_ObjectTypeEnum.State: {
                // Attempt to relocate transition modules.
                myStorage.ForEachChildPort(node,
                    p=> {
                        if(p.IsStatePort) {
                            iCS_EditorObject transitionModule= null;
                            if(p.IsInStatePort) {
                                transitionModule= myStorage.GetParent(myStorage.GetSource(p));
                            } else {
                                iCS_EditorObject[] connectedPorts= myStorage.FindConnectedPorts(p);
                                foreach(var cp in connectedPorts) {
                                    if(cp.IsInTransitionPort) {
                                        transitionModule= myStorage.GetParent(cp);
                                        break;
                                    }
                                }
                            }
                            iCS_EditorObject outState= myStorage.GetParent(myStorage.GetOutStatePort(transitionModule));
                            iCS_EditorObject inState= myStorage.GetParent(myStorage.GetInStatePort(transitionModule));
                            iCS_EditorObject newParent= myStorage.GetTransitionParent(inState, outState);
                            if(newParent != null && newParent != myStorage.GetParent(transitionModule)) {
                                ChangeParent(transitionModule, newParent);
                                myStorage.LayoutTransitionModule(transitionModule);
                                myStorage.SetDirty(myStorage.GetParent(node));
                            }
                        }
                    }
                );
                // Ask our children to cleanup their connections.
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                myStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.TransitionModule:
            case iCS_ObjectTypeEnum.TransitionGuard:
            case iCS_ObjectTypeEnum.TransitionAction:
            case iCS_ObjectTypeEnum.Module: {
                List<iCS_EditorObject> childNodes= new List<iCS_EditorObject>();
                myStorage.ForEachChild(node, c=> { if(c.IsNode) childNodes.Add(c);});
                foreach(var childNode in childNodes) { CleanupConnections(childNode); }
                break;
            }
            case iCS_ObjectTypeEnum.InstanceMethod:
            case iCS_ObjectTypeEnum.StaticMethod:
            case iCS_ObjectTypeEnum.InstanceField:
            case iCS_ObjectTypeEnum.StaticField:
            case iCS_ObjectTypeEnum.TypeCast: {
                myStorage.ForEachChildPort(node,
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
        iCS_EditorObject sourcePort= myStorage.GetDataConnectionSource(inPort);
        // Tear down previous connection.
        iCS_EditorObject tmpPort= myStorage.GetSource(inPort);
        List<iCS_EditorObject> toDestroy= new List<iCS_EditorObject>();
        while(tmpPort != null && tmpPort != sourcePort) {
            iCS_EditorObject[] connected= myStorage.FindConnectedPorts(tmpPort);
            if(connected.Length == 1) {
                iCS_EditorObject t= myStorage.GetSource(tmpPort);
                toDestroy.Add(tmpPort);
                tmpPort= t;
            } else {
                break;
            }
        }
        foreach(var byebye in toDestroy) {
            myStorage.DestroyInstance(byebye.InstanceId);
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
        iCS_EditorObject[] connectedPorts= myStorage.FindConnectedPorts(outPort);
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
        iCS_EditorObject pasted= myStorage.CopyFrom(sourceRoot, new iCS_IStorage(sourceStorage), parent, point);
        myStorage.Fold(pasted);
    }

    // ======================================================================
    // Graph Navigation
	// ----------------------------------------------------------------------
    public void CenterOnRoot() {
        CenterOn(myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOnSelected() {
        CenterOn(SelectedObject ?? myDisplayRoot);
    }
	// ----------------------------------------------------------------------
    public void CenterOn(iCS_EditorObject obj) {
        if(obj == null || myStorage == null) return;
        CenterAt(Math3D.Middle(myStorage.GetPosition(obj)));
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
                          myStorage.Preferences.Grid.BackgroundColor,
                          myStorage.Preferences.Grid.GridColor,
                          myStorage.Preferences.Grid.GridSpacing);
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
        Scale= iCS_ToolbarUtility.Slider(ref r, 120f, Scale, 1f, 0.15f, spacer, spacer, true);
        iCS_ToolbarUtility.Label(ref r, new GUIContent("Zoom"), 0, 0, true);
		
		// Show current bookmark.
		string bookmarkString= "Bookmark: ";
		if(Bookmark == null) {
		    bookmarkString+= "(empty)";
		} else {
		    bookmarkString+= Bookmark.Name;
		}
		iCS_ToolbarUtility.Label(ref r, 150f, new GUIContent(bookmarkString),0,0,true);
		
		// Editable field test.		
		iCS_ToolbarUtility.Label(ref r, new GUIContent("Mode:"), 0, 0, false);		
		MenuOption= iCS_ToolbarUtility.Buttons(ref r, 90f, MenuOption, menuOptions, 0, 0);
		iCS_ToolbarUtility.Buttons(ref r, 8f, -1, new string[1]{""}, 0, 0);
	}
	// ----------------------------------------------------------------------
	void DrawGraph () {
        // Ask the storage to update itself.
        myStorage.Update();

		// Start graphics
        Graphics.Begin(UpdateScrollPosition(), Scale, ClipingArea, SelectedObject, ViewportToGraph(MousePosition), myStorage);
        
        // Draw editor grid.
        DrawGrid();
        
        // Draw nodes and their connections.
    	DrawNormalNodes();
        DrawConnections();
        DrawMinimizedNodes();           

        Graphics.End();

        // Show scroll zone (is applicable).
        if(IsDragStarted) DrawScrollZone();

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
        myStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating && !node.IsBehaviour) {
                	Graphics.DrawNormalNode(node, myStorage);                        
                }
            }
        );
        myStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating && !node.IsBehaviour) {
                	Graphics.DrawNormalNode(node, myStorage);                        
                }
            }
        );
    }	
	// ----------------------------------------------------------------------
    void DrawMinimizedNodes() {
        // Display node starting from the root node.
        myStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating) {
                	Graphics.DrawMinimizedNode(node, myStorage);                        
                }
            }
        );
        myStorage.ForEachRecursiveDepthLast(myDisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating) {
                	Graphics.DrawMinimizedNode(node, myStorage);                        
                }
            }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        myStorage.ForEachChildRecursive(myDisplayRoot, port=> { if(port.IsPort) Graphics.DrawConnection(port, myStorage); });

        // Display ports.
        myStorage.ForEachChildRecursive(myDisplayRoot, port=> { if(port.IsPort) Graphics.DrawPort(port, myStorage); });
    }

    // ======================================================================
    // SCROLL ZONE
	// ----------------------------------------------------------------------
    void ProcessScrollZone() {
        // Compute the amount of scroll needed.
        var dir= CanScrollInDirection(DetectScrollZone());
        if(Math3D.IsZero(dir)) return;
        dir*= myStorage.Preferences.ControlOptions.EdgeScrollSpeed*DeltaTime;

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
        Rect rootRect= myStorage.GetPosition(myDisplayRoot);
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
