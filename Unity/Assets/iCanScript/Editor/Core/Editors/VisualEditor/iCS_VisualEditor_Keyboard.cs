using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
	// ======================================================================
	// Properties
	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus    { get { return EditorWindow.focusedWindow == this; }}
    bool    IsShiftKeyDown      { get { return Event.current.shift; }}
    bool    IsControlKeyDown    { get { return Event.current.control; }}
    bool    IsAltKeyDown        { get { return Event.current.alt; }}
    bool    IsFloatingKeyDown	{ get { return IsControlKeyDown; }}
    bool    IsCopyKeyDown       { get { return IsShiftKeyDown; }}
    bool    IsScaleKeyDown      { get { return IsAltKeyDown; }}
    
	// ----------------------------------------------------------------------
    void KeyDownEvent() {
	    /*
	     TODO: use Event.character for all alphanumeric keyboard commands.
	    */
	    if(!HasKeyboardFocus) return;
		var ev= Event.current;
		if(ev.keyCode == KeyCode.None) return;
        switch(ev.keyCode) {
            // Tree navigation
            case KeyCode.UpArrow: {
                if(SelectedObject != null) {
                    // Move node
                    if(IsShiftKeyDown && SelectedObject.IsNode) {
                        var newPos= SelectedObject.LayoutPosition;
                        newPos.y-= IsAltKeyDown ? 5f: 1f;
                        SelectedObject.IsSticky= true;
                        SelectedObject.NodeDragTo(newPos);
                        SelectedObject.IsSticky= false;
                        Event.current.Use();
                        return;
                    }
                    SelectedObject= SelectedObject.Parent;
                    CenterOnSelected();
                } 
                Event.current.Use();
                break;
            }
            case KeyCode.DownArrow: {
                if(IsShiftKeyDown && SelectedObject.IsNode) {
                    var newPos= SelectedObject.LayoutPosition;
                    newPos.y+= IsAltKeyDown ? 5f: 1f;
                    SelectedObject.IsSticky= true;
                    SelectedObject.NodeDragTo(newPos);
                    SelectedObject.IsSticky= false;
                    Event.current.Use();
                    return;
                }
                if(SelectedObject == null) SelectedObject= DisplayRoot;
				if(SelectedObject.IsUnfoldedInLayout) {
	                SelectedObject= iCS_EditorUtility.GetFirstChild(SelectedObject, IStorage);
	                CenterOnSelected();					
				}
                Event.current.Use();
                break;
            }
            case KeyCode.RightArrow: {
                if(IsShiftKeyDown && SelectedObject.IsNode) {
                    var newPos= SelectedObject.LayoutPosition;
                    newPos.x+= IsAltKeyDown ? 5f: 1f;
                    SelectedObject.IsSticky= true;
                    SelectedObject.NodeDragTo(newPos);
                    SelectedObject.IsSticky= false;
                    Event.current.Use();
                    return;
                }
                SelectedObject= iCS_EditorUtility.GetNextSibling(SelectedObject, IStorage);
                CenterOnSelected();
                Event.current.Use();
                break;
            }
            case KeyCode.LeftArrow: {
                if(IsShiftKeyDown && SelectedObject.IsNode) {
                    var newPos= SelectedObject.LayoutPosition;
                    newPos.x-= IsAltKeyDown ? 5f: 1f;
                    SelectedObject.IsSticky= true;
                    SelectedObject.NodeDragTo(newPos);
                    SelectedObject.IsSticky= false;
                    Event.current.Use();
                    return;
                }
                SelectedObject= iCS_EditorUtility.GetPreviousSibling(SelectedObject, IStorage);
                CenterOnSelected();
                Event.current.Use();
                break;
            }
            case KeyCode.F: {
                var selected= SelectedObject;
                if(selected == null || ev.shift) {
                    selected= DisplayRoot;
                }
                if(selected != null) {
                    iCS_EditorUtility.SafeFocusOn(selected, IStorage);                        
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
                    myBookmark= SelectedObject;                            
                }
                Event.current.Use();
                break;
            }
            case KeyCode.G: {  // Goto bookmark
                if(myBookmark != null) {
                    SelectedObject= myBookmark;
                    CenterOnSelected();
                }
                Event.current.Use();
                break;
            }
            case KeyCode.S: {  // Switch bookmark and selected object
                if(myBookmark != null && SelectedObject != null) {
                    iCS_EditorObject prevmyBookmark= myBookmark;
                    myBookmark= SelectedObject;
                    SelectedObject= prevmyBookmark;
                    CenterOnSelected();
                } else if(myBookmark != null && SelectedObject == null) {
                    SelectedObject= myBookmark;
                    CenterOnSelected();
                } else if(myBookmark == null && SelectedObject != null) {
                    myBookmark= SelectedObject;
                }
                Event.current.Use();
                break;
            }
            case KeyCode.C: {  // Connect bookmark and selected port.
                if(myBookmark != null && myBookmark.IsDataPort && SelectedObject != null && SelectedObject.IsDataPort) {
                    VerifyNewConnection(myBookmark, SelectedObject);
                }
                Event.current.Use();
                break;
            }
            // Object deletion
            case KeyCode.Delete:
            case KeyCode.Backspace: {
                if(SelectedObject != null && SelectedObject != DisplayRoot && SelectedObject != StorageRoot &&
                  !SelectedObject.IsTransitionAction && !SelectedObject.IsTransitionGuard) {
	                iCS_EditorObject parent= SelectedObject.Parent;
					bool changeSelected= true;
					if(IsControlKeyDown) {
						iCS_EditorUtility.ForceDestroyObject(SelectedObject, IStorage);
					} else {
	                    changeSelected= iCS_EditorUtility.SafeDestroyObject(SelectedObject, IStorage);						
					}
                    if(changeSelected) {
	                    SelectedObject= parent;	
					}
                }
                Event.current.Use();
                break;
            }
            // Object creation.
            case KeyCode.KeypadEnter: // fnc+return on Mac
            case KeyCode.Insert: {
                if(SelectedObject == null) SelectedObject= DisplayRoot;
                // Don't use mouse position if it is too far from selected node.
                Vector2 graphPos= GraphMousePosition;
                Rect parentRect= SelectedObject.LayoutRect;
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
                    if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.Update, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                        IStorage.RegisterUndo("Create Update");
                        newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.Update);  
                    } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.LateUpdate, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                        IStorage.RegisterUndo("Create LateUpdate");
                        newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.LateUpdate);                                  
                    } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.FixedUpdate, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                        IStorage.RegisterUndo("Create FixedUpdate");
                        newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.FixedUpdate);                                  
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
                if(SelectedObject.IsKindOfModule) {
                    iCS_EditorObject newObj= null;
                    if(!ev.shift) {
                        IStorage.RegisterUndo("Create Module");
                        newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, null);                                
                    } else {
                        IStorage.RegisterUndo("Create State Chart");
                        newObj= IStorage.CreateStateChart(SelectedObject.InstanceId, graphPos, null);
                    }
                    if(ev.control && newObj != null) {
                        SelectedObject= newObj;
                    }
                    Event.current.Use();
                    break;
                }
                // Auto-insert on state chart.
                if(SelectedObject.IsStateChart) {
                    IStorage.RegisterUndo("Create State");
                    iCS_EditorObject newObj= IStorage.CreateState(SelectedObject.InstanceId, graphPos);
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
                        if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnUpdate, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                            IStorage.RegisterUndo("Create OnUpdate");
                            newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnUpdate);  
                        } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnEntry, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                            IStorage.RegisterUndo("Create OnEntry");
                            newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnEntry);                                  
                        } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnExit, iCS_ObjectTypeEnum.Aggregate, SelectedObject, IStorage)) {
                            IStorage.RegisterUndo("Create OnExit");
                            newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnExit);                                  
                        }                                
                    } else {
                        IStorage.RegisterUndo("Create State");
                        newObj= IStorage.CreateState(SelectedObject.InstanceId, graphPos);
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
    }
}
