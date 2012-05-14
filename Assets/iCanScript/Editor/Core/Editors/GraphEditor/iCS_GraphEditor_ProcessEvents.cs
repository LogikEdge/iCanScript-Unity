using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// ===========================================================================
// Graph editor event processing.
// ===========================================================================
public partial class iCS_GraphEditor : iCS_EditorWindow {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
	iCS_SubEditor		mySubEditor       = null;
	iCS_EditorObject	myObjectUnderMouse= null;
    
    // ======================================================================
    // USER INTERACTIONS
	// ----------------------------------------------------------------------
    void ProcessEvents() {
        // Process window events.
        switch(Event.current.type) {
            case EventType.MouseMove: {
                switch(Event.current.button) {
                    case 2: { // Middle mouse button
                        Vector2 diff= MousePosition-MouseDragStartPosition;
                        ScrollPosition= DragStartPosition-diff;
                        break;
                    }
                    default: {
                        ResetDrag();
//						iCS_EditorObject objectUnderMouse= GetObjectAtMousePosition();
//						if(mySubEditor != null && objectUnderMouse == myObjectUnderMouse && mySubEditor.Target == objectUnderMouse) break;
//						if(mySubEditor != null) {
//							mySubEditor.Close();
//							break;
//						}
//						myObjectUnderMouse= objectUnderMouse;
//						if(myObjectUnderMouse == null) break;
//						Debug.Log("Object under cursor= "+myObjectUnderMouse.Name);
//						if(myObjectUnderMouse.IsNode) {
//                            var screenPoint= GUIUtility.GUIToScreenPoint(RealMousePosition);
//                            mySubEditor= GetWindowWithRect<iCS_NodeTitlePopup>(new Rect(screenPoint.x, screenPoint.y, 200, 75));
//                            mySubEditor.Init(myObjectUnderMouse, IStorage);
////                            mySubEditor.ShowPopup();										
//						} else {
//							mySubEditor= ScriptableObject.CreateInstance<iCS_PortEditor>();
//                            mySubEditor.Init(myObjectUnderMouse, IStorage);
//                            var screenPoint= GUIUtility.GUIToScreenPoint(RealMousePosition);
//                            mySubEditor.position= new Rect(screenPoint.x, screenPoint.y, mySubEditor.position.width, mySubEditor.position.height);
//                            mySubEditor.ShowPopup();																				
//						}						
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
                float mouseUpDeltaTime= myCurrentTime-MouseUpTime;
                MouseUpTime= myCurrentTime;
                if(IsDragStarted) {
                    EndDrag();
                } else {
					if(ShouldRotateMuxPort) RotateSelectedMuxPort();
                    if(SelectedObject != null) {
                        // Process fold/unfold/minimize/maximize click.
                        Vector2 mouseGraphPos= MouseGraphPosition;
                        if(myGraphics.IsFoldIconPicked(SelectedObject, mouseGraphPos, IStorage)) {
                            if(IStorage.IsFolded(SelectedObject)) {
                                IStorage.RegisterUndo("Unfold");
                                IStorage.Maximize(SelectedObject);
                            } else {
                                IStorage.RegisterUndo("Fold");
                                IStorage.Fold(SelectedObject);
                            }
                        } else if(myGraphics.IsMinimizeIconPicked(SelectedObject, mouseGraphPos, IStorage)) {
                            IStorage.RegisterUndo("Minimize");
                            IStorage.Minimize(SelectedObject);
                        } else {
                            // Fold/Unfold on double click.
                            if(SelectedObject == SelectedObjectBeforeMouseDown) {
                                if(mouseUpDeltaTime < 0.25f) {
                                    ProcessNodeDisplayOptionEvent();
                                }
                                else {
                                    Event.current.Use();
                                    myGraphics.GetPickInfo(ViewportToGraph(MousePosition), IStorage);
//									if(SelectedObject.IsNode) {
//										if(mySubEditor != null) mySubEditor.Close();
//	                                    mySubEditor= ScriptableObject.CreateInstance<iCS_NodeTitlePopup>();
//	                                    var screenPoint= GUIUtility.GUIToScreenPoint(RealMousePosition);
//	                                    mySubEditor.position= new Rect(screenPoint.x, screenPoint.y, mySubEditor.position.width, mySubEditor.position.height);
//	                                    mySubEditor.Init(SelectedObject, IStorage);
//	                                    mySubEditor.ShowPopup();										
//									} else {
//										if(mySubEditor != null) mySubEditor.Close();
//										mySubEditor= ScriptableObject.CreateInstance<iCS_PortEditor>();
//	                                    var screenPoint= GUIUtility.GUIToScreenPoint(RealMousePosition);
//	                                    mySubEditor.position= new Rect(screenPoint.x, screenPoint.y, mySubEditor.position.width, mySubEditor.position.height);
//	                                    mySubEditor.Init(SelectedObject, IStorage);
//	                                    mySubEditor.ShowPopup();																				
//									}
                                    break;
                                }
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
					float zoomDirection= IStorage.Preferences.ControlOptions.InverseZoom ? -1f : 1f;
                    Scale= Scale+(delta.y > 0 ? -0.05f : 0.05f)*zoomDirection;
                    Vector2 offset= pivot-ViewportToGraph(MousePosition);
                    ScrollPosition+= offset;
                } else {
                    delta*= IStorage.Preferences.ControlOptions.ScrollSpeed*(1f/Scale); 
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
                            SelectedObject= IStorage.GetParent(SelectedObject);
                            CenterOnSelected();
                        } 
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.DownArrow: {
                        if(SelectedObject == null) SelectedObject= myDisplayRoot;
                        SelectedObject= iCS_EditorUtility.GetFirstChild(SelectedObject, IStorage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.RightArrow: {
                        SelectedObject= iCS_EditorUtility.GetNextSibling(SelectedObject, IStorage);
                        CenterOnSelected();
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.LeftArrow: {
                        SelectedObject= iCS_EditorUtility.GetPreviousSibling(SelectedObject, IStorage);
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
                    // myBookmarks
                    case KeyCode.B: {  // myBookmark selected object
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
                        if(SelectedObject != null && SelectedObject != myDisplayRoot && SelectedObject != StorageRoot &&
                          !SelectedObject.IsTransitionAction && !SelectedObject.IsTransitionGuard) {
                            iCS_EditorObject parent= IStorage.GetParent(SelectedObject);
                            if(ev.shift) {
                                IStorage.RegisterUndo("Delete");
                                IStorage.DestroyInstance(SelectedObject.InstanceId);                                                        
                            } else {
                                iCS_EditorUtility.DestroyObject(SelectedObject, IStorage);
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
                        Rect parentRect= IStorage.GetPosition(SelectedObject);
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
                            if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.Update, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
                                IStorage.RegisterUndo("Create Update");
                                newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.Update);  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.LateUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
                                IStorage.RegisterUndo("Create LateUpdate");
                                newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.LateUpdate);                                  
                            } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.FixedUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
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
                        if(SelectedObject.IsModule) {
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
                                if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnUpdate, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
                                    IStorage.RegisterUndo("Create OnUpdate");
                                    newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnUpdate);  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnEntry, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
                                    IStorage.RegisterUndo("Create OnEntry");
                                    newObj= IStorage.CreateModule(SelectedObject.InstanceId, graphPos, iCS_Strings.OnEntry);                                  
                                } else if(iCS_AllowedChildren.CanAddChildNode(iCS_Strings.OnExit, iCS_ObjectTypeEnum.Module, SelectedObject, IStorage)) {
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
                    // Class module shortcuts.
                    case KeyCode.D: {
                        if(SelectedObject.IsClassModule) {
                            IStorage.ClassModuleCreateInputInstanceFields(SelectedObject);
                            IStorage.ClassModuleDestroyOutputInstanceFields(SelectedObject);
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
                IStorage.RegisterUndo("Unfold");
                IStorage.Fold(SelectedObject);
            } else if(SelectedObject.IsMaximized) {
                IStorage.RegisterUndo("Fold");
                IStorage.Fold(SelectedObject);
            } else {
                if(IsShiftKeyDown) {
                    IStorage.RegisterUndo("Minimize");
                    IStorage.Minimize(SelectedObject);
                } else {
                    IStorage.RegisterUndo("Maximize");
                    IStorage.Maximize(SelectedObject);                                                        
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
        myDynamicMenu.Update(SelectedObject, IStorage, ViewportToGraph(MousePosition), MenuOption == 0);
        IStorage.SetDirty(SelectedObject);                    
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
}
