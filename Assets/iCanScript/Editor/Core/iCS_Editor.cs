using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;


// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This non-persistante class is used to edit the iCS_Behaviour.
public class iCS_Editor : EditorWindow {
    // ======================================================================
	// iCanScript Window Menu.
	[MenuItem("Window/iCanScript Editor")]
	public static void ShowiCanScriptEditor() {
        iCS_Editor editor= EditorWindow.GetWindow(typeof(iCS_Editor), false, "iCanScript") as iCS_Editor;
        editor.hideFlags= HideFlags.DontSave;
	}

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
    public  iCS_ScrollView  ScrollView= null;
    private bool			InitScrollPosition= false;
    
    // ----------------------------------------------------------------------
    enum DragTypeEnum { None, PortDrag, NodeDrag, TransitionCreation };
    DragTypeEnum    DragType              = DragTypeEnum.None;
    iCS_EditorObject DragObject            = null;
    Vector2         MouseDragStartPosition= Vector2.zero;
    Vector2         DragStartPosition     = Vector2.zero;
    bool            IsDragEnabled         = false;
    bool            IsDragStarted         { get { return IsDragEnabled && DragObject != null; }}

    // ----------------------------------------------------------------------
    Vector2 ScreenCenter { get { return new Vector2(0.5f*position.width, 0.5f*position.height); } }
    
    // ----------------------------------------------------------------------
    static bool	ourAlreadyParsed  = false;
     
//    // ----------------------------------------------------------------------
//	// Editor information.
//	bool IsNameEditorActive = false;
//	bool IsValueEditorActive= false;
	
    // ======================================================================
    // ACCESSORS
	// ----------------------------------------------------------------------
    iCS_EditorObject SelectedObject {
        get { return mySelectedObject; }
        set {
			if(mySelectedObject != value) {
				mySelectedObject= value;
				if(Inspector != null) Inspector.SelectedObject= value;				
//  			IsNameEditorActive= false;
//  			IsValueEditorActive= false;
			}
		}
    }
    iCS_EditorObject mySelectedObject= null;
    public iCS_IStorage Storage { get { return myStorage; } set { myStorage= value; }}
	// ----------------------------------------------------------------------
    bool    IsCommandKeyDown       { get { return Event.current.command; }}
    bool    IsControlKeyDown       { get { return Event.current.control; }}
    
	// ----------------------------------------------------------------------
	// Mouse services
	void UpdateMouse() {
        myMousePosition= Event.current.mousePosition;
        if(Event.current.type == EventType.MouseDrag) myMousePosition+= Event.current.delta;
	}
    Vector2 MousePosition { get { return myMousePosition; } }
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
        ScrollView      = new iCS_ScrollView();
        DynamicMenu     = new iCS_DynamicMenu();
        
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
        ScrollView  = null;
        DynamicMenu = null;
    }
    
//    WWW GetWebPage() {
//		return new WWW("http://www.icanscript.com/index.html");
//    }

    // ----------------------------------------------------------------------
    // Activates the editor and initializes all Graph shared variables.
	public void Activate(iCS_IStorage storage, iCS_Inspector inspector) {
        Storage= storage;
        Inspector= inspector;
        // Set the graph root.
        DisplayRoot= storage.IsValid(0) ? storage[0] : null;
		InitScrollPosition= true;
		
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
        
        // Update scroll view.
        Rect scrollViewPosition= DisplayRoot != null ? Storage.GetPosition(DisplayRoot) : new Rect(0,0,500,500);
		ScrollView.Update(position, scrollViewPosition);
		if(InitScrollPosition) {
			InitScrollPosition= false;
			ScrollView.CenterAt(Storage.ScrollPosition, 0);
		} else {
	        Storage.ScrollPosition= ScrollView.ScreenToGraph(new Vector2(0.5f*position.width, 0.5f*position.height));			
		}
		
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
                        ScrollView.CenterAt(DragStartPosition-diff, 0f);
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
                        ScrollView.CenterAt(DragStartPosition-diff, 0f);
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
                        IsDragEnabled= true;
                        Event.current.Use();
                        break;
                    }
                    case 1: { // Right mouse button
                        DynamicMenu.Update(SelectedObject, Storage, ScrollView.ScreenToGraph(MousePosition));
                        Event.current.Use();
                        break;
                    }
                    case 2: { // Middle mouse button
                        MouseDragStartPosition= MousePosition;
                        DragStartPosition= ScrollView.ScreenToGraph(ScreenCenter);
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
                            Storage.Fold(SelectedObject);
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
                if(mouseOverWindow == this) {
                    DragAndDropExited();
                    Event.current.Use();
                }
                break;
            }
//			case EventType.KeyDown: {
//				var ev= Event.current;
//				if(ev.keyCode == KeyCode.None) break;
//				bool isReturn= ev.keyCode == KeyCode.Return; 
//				if(isReturn) { IsNameEditorActive= false; Debug.Log("Name editor is deactivated."); break; }
//				Debug.Log("Key code: "+Event.current.keyCode);
//				if(SelectedObject != null) {
//					Debug.Log("Name editor is activated.");
//					IsNameEditorActive= true;
//				} else {
//					Debug.Log("Name editor is deactivated.");
//					IsNameEditorActive= false;
//				}
//				break;
//			}
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
            PasteIntoGraph(ScrollView.ScreenToGraph(MousePosition), storage, storage.EditorObjects[0]);
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
                node.IsFloating= IsCommandKeyDown;
                break;
            case DragTypeEnum.PortDrag:
            case DragTypeEnum.TransitionCreation:
                iCS_EditorObject port= DragObject;
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
        MouseDragStartPosition= MouseDownPosition;
        Vector2 pos= ScrollView.ScreenToGraph(MouseDragStartPosition);

        // Port drag.
        iCS_EditorObject port= Storage.GetPortAt(pos);
        if(port != null && !Storage.IsMinimized(port)) {
            Storage.RegisterUndo("Port Drag");
            DragType= DragTypeEnum.PortDrag;
            DragObject= port;
            DragStartPosition= new Vector2(port.LocalPosition.x, port.LocalPosition.y);
            port.IsFloating= true;
            return true;
        }

        // Node drag.
        iCS_EditorObject node= Storage.GetNodeAt(pos);                
        if(node != null && (node.IsMinimized || !node.IsState || Graphics.IsNodeTitleBarPicked(node, pos, Storage))) {
            if(IsControlKeyDown) {
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
                node.IsFloating= IsCommandKeyDown;
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
                    iCS_EditorObject port= DragObject;
                    port.IsFloating= false;
                    // Verify for a new connection.
                    if(!VerifyNewConnection(port)) {
                        // Verify for disconnection.
                        Storage.SetDirty(port);
                        if(!Storage.IsNearParent(port)) {
                            if(port.IsDataPort) {
                                iCS_EditorObject newPortParent= GetNodeAtMousePosition();
                                if(newPortParent != null && newPortParent.IsModule) {
                                    iCS_EditorObject portParent= Storage.GetParent(port);
                                    Rect portPos= Storage.GetPosition(port);
                                    Rect modulePos= Storage.GetPosition(newPortParent);
                                    float portSize2= 2f*iCS_EditorConfig.PortSize;
                                    if(port.IsOutputPort) {
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(!Storage.IsChildOf(newPortParent, portParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(newPort, port);                               
                                                break;
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.xMax-portSize2, modulePos.xMax+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
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
                                                iCS_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
                                                port.LocalPosition.x= DragStartPosition.x;
                                                port.LocalPosition.y= DragStartPosition.y;
                                                Storage.SetSource(port, newPort);                               
                                                break;                                                                                                    
                                            }
                                        }
                                        if(Math3D.IsWithinOrEqual(portPos.x, modulePos.x-portSize2, modulePos.x+portSize2)) {
                                            if(Storage.IsChildOf(portParent, newPortParent) || Storage.IsChildOf(newPortParent, portParent)) {
                                                iCS_EditorObject newPort= Storage.CreatePort(port.Name, newPortParent.InstanceId, port.RuntimeType, iCS_ObjectTypeEnum.InDynamicModulePort);
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
        DragType= DragTypeEnum.None;
        DragObject= null;
        IsDragEnabled= false;                    
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
        Vector2 graphPosition= ScrollView.ScreenToGraph(MousePosition);
        return Storage.GetNodeAt(graphPosition);
    }

	// ----------------------------------------------------------------------
    // Returns the object at the given mouse position.
    public iCS_EditorObject GetObjectAtScreenPosition(Vector2 _screenPos) {
        Vector2 graphPosition= ScrollView.ScreenToGraph(_screenPos);
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
    bool VerifyNewConnection(iCS_EditorObject port) {
        // No new connection if no overlapping port found.
        iCS_EditorObject overlappingPort= Storage.GetOverlappingPort(port);
        if(overlappingPort == null) return false;
        
        // Reestablish port position.
        port.LocalPosition.x= DragStartPosition.x;
        port.LocalPosition.y= DragStartPosition.y;
        
        // Connect function & modules ports together.
        if(port.IsDataPort && overlappingPort.IsDataPort) {            
            iCS_EditorObject inPort = null;
            iCS_EditorObject outPort= null;

            iCS_EditorObject portParent= Storage.EditorObjects[port.ParentId];
            iCS_EditorObject overlappingPortParent= Storage.EditorObjects[overlappingPort.ParentId];
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
				Type inType= inPort.RuntimeType;
				Type outType= outPort.RuntimeType;
                if(iCS_Types.CanBeConnectedWithoutConversion(outType, inType)) { // No conversion needed.
                    SetNewDataConnection(inPort, outPort);                       
                }
                else {  // A conversion is required.
					if(iCS_Types.CanBeConnectedWithUpConversion(outType, inType)) {
						if(EditorUtility.DisplayDialog("Up Conversion Connection", "Are you sure you want to generate a conversion from "+iCS_Types.GetTypeName(outType)+" to "+iCS_Types.GetTypeName(inType)+"?", "Generate Conversion", "Abort")) {
							SetNewDataConnection(inPort, outPort);							
						}
					} else {
	                    iCS_ReflectionDesc conversion= iCS_DataBase.FindConversion(outType, inType);
	                    if(conversion == null) {
							ShowNotification(new GUIContent("No direct conversion exists from "+iCS_Types.GetTypeName(outType)+" to "+iCS_Types.GetTypeName(inType)));
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

        // Connect transition port together.
        if(port.IsStatePort && overlappingPort.IsStatePort) {
            return true;
        }
        
        Debug.LogWarning("Trying to connect incompatible port types: "+port.TypeName+"<=>"+overlappingPort.TypeName);
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
        if(!inParentSeen) {
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
        if(!outParentSeen) {
            iCS_EditorObject newPort= Storage.CreatePort(outPort.Name, outParent.InstanceId, outPort.RuntimeType, iCS_ObjectTypeEnum.OutDynamicModulePort);
            Storage.SetSource(newPort, outPort, conversion);
            SetNewDataConnection(inPort, newPort);
            return;                       
        }
        // Should never happen ... just connect the ports.
        Storage.SetSource(inPort, outPort, conversion);
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
        if(obj == null) return;
        CenterAt(Math3D.Middle(Storage.GetPosition(obj)));
    }
	// ----------------------------------------------------------------------
    public void CenterAt(Vector2 point) {
        if(ScrollView == null) return;
        ScrollView.CenterAt(point);
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

//		if(IsNameEditorActive) {
//			Rect selectedRect= Storage.GetPosition(SelectedObject);
//			Rect editorRect= new Rect(selectedRect.x, selectedRect.y, selectedRect.width, iCS_EditorConfig.NodeTitleHeight);
//			GUI.SetNextControlName("NameEntry");
//			SelectedObject.Name= GUI.TextField(editorRect, SelectedObject.Name, GUI.skin.box);
//			GUI.FocusControl("NameEntry");
//		}
		
        ScrollView.End();
	}

	// ----------------------------------------------------------------------
    void DrawNormalNodes() {
        // Display node starting from the root node.
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && !node.IsFloating) {
                	Graphics.DrawNormalNode(node, SelectedObject, Storage);                        
                }
            }
        );
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating) {
                	Graphics.DrawNormalNode(node, SelectedObject, Storage);                        
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
                	Graphics.DrawMinimizedNode(node, SelectedObject, Storage);                        
                }
            }
        );
        Storage.ForEachRecursiveDepthLast(DisplayRoot,
            node=> {
                if(node.IsNode && node.IsFloating) {
                	Graphics.DrawMinimizedNode(node, SelectedObject, Storage);                        
                }
            }
        );
    }	
	
	// ----------------------------------------------------------------------
    private void DrawConnections() {
        // Display all connections.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawConnection(port, Storage); });

        // Display ports.
        Storage.ForEachChildRecursive(DisplayRoot, port=> { if(port.IsPort) Graphics.DrawPort(port, SelectedObject, Storage); });
    }

}
