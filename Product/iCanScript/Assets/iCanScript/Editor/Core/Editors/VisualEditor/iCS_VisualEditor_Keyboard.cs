using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Internal.Editor;

namespace iCanScript.Internal.Editor {
public partial class iCS_VisualEditor : iCS_EditorBase {
	// ======================================================================
	// Properties
	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus     { get { return EditorWindow.focusedWindow == this; }}
    bool    IsShiftKeyDown       { get { return Event.current.shift; }}
    bool    IsControlKeyDown     { get { return Event.current.control; }}
    bool    IsAltKeyDown         { get { return Event.current.alt; }}
    bool    IsCommandKeyDown     { get { return Event.current.command; }}
    bool    IsDoubleClick        { get { return Event.current.clickCount >= 2; }}

    bool    IsFloatingKeyDown	 { get { return IsControlKeyDown; }}
    bool    IsCopyKeyDown        { get { return IsShiftKeyDown; }}
    bool    IsScaleKeyDown       { get { return IsAltKeyDown; }}
    bool    IsMaximizeKeyDown    { get { return IsAltKeyDown; }}
    bool    IsIconizeKeyDown     { get { return IsAltKeyDown && IsShiftKeyDown; }}
	bool    IsDisplayRootKeyDown { get { return IsControlKeyDown; }}
    bool    IsHistoryKeyDown     { get { return IsCommandKeyDown; }}
    bool    IsMuxPortKeyDown     { get { return IsControlKeyDown; }}
	bool	IsMultiSelectKeyDown {
        get {
            if(Application.platform == RuntimePlatform.OSXEditor) {
                return IsCommandKeyDown;                
            }
            else {
                return IsControlKeyDown;
            }
        }
    }
	
    
	// ----------------------------------------------------------------------
    void KeyDownEvent() {
	    /*
	     TODO: use Event.character for all alphanumeric keyboard commands.
	    */
	    if(!HasKeyboardFocus) return;
		var ev= Event.current;
		var keyCode= ev.keyCode;
		if(keyCode == KeyCode.None) return;
        switch(ev.keyCode) {
            // Reset to default
            case KeyCode.Escape: {
                IStorage.ClearMultiSelection();
                break;
            }
            // Tree navigation
            case KeyCode.UpArrow: {
                if(SelectedObject != null) {
                    // Move node
                    if(IsShiftKeyDown && SelectedObject.IsNode) {
                        var newPos= SelectedObject.GlobalPosition;
                        newPos.y-= IsAltKeyDown ? 5f: 1f;
                        SelectedObject.IsSticky= true;
                        SelectedObject.NodeDragTo(newPos);
                        SelectedObject.IsSticky= false;
                        Event.current.Use();
                        return;
                    }
                    if(SelectedObject == DisplayRoot) {
                        
                    }                    
                    SelectedObject= SelectedObject.Parent;
                    if(SelectedObject.IsParentOf(DisplayRoot)) {
                        DisplayRoot= SelectedObject;
                        CenterAndScaleOn(SelectedObject);
                    }
                    CenterOn(SelectedObject);
                } 
                Event.current.Use();
                break;
            }
            case KeyCode.DownArrow: {
                if(IsShiftKeyDown && SelectedObject.IsNode) {
                    var newPos= SelectedObject.GlobalPosition;
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
                    var newPos= SelectedObject.GlobalPosition;
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
                    var newPos= SelectedObject.GlobalPosition;
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
            // Navigation
            case KeyCode.LeftBracket: {
//                if(IsNavigationKeyDown) {
                    iCS_UserCommands.ReloadFromBackwardNavigationHistory(IStorage);
                    Event.current.Use();                    
//                }
                break;
            }
            case KeyCode.RightBracket: {
//                if(IsNavigationKeyDown) {
                    iCS_UserCommands.ReloadFromForwardNavigationHistory(IStorage);
                    Event.current.Use();                    
//                }
                break;
            }
            case KeyCode.F: {
                if(IsControlKeyDown) {
                    iCS_UserCommands.FocusOn(DisplayRoot);                        
                }
                else {
                    var focusNode= SelectedObject;
                    if(focusNode != null) {
                        if(IsShiftKeyDown) {
                            var parent= focusNode.ParentNode;
                            if(parent != null) {
                                focusNode= parent;
                            }
                        }
                        iCS_UserCommands.SmartFocusOn(focusNode);
                    }
                }
                Event.current.Use();
                break;
            }
            // Toggle show root node
			case KeyCode.R: {
				if(IsCommandKeyDown) {
					SendEvent(EditorGUIUtility.CommandEvent("ReloadStorage"));
				}
				else {
					iCS_UserCommands.ToggleShowDisplayRootNode(IStorage);
				}
                Event.current.Use();
				break;
			}
            // Layout
            case KeyCode.L: {
                if(SelectedObject != null) {
	                if(SelectedObject.IsDataOrControlPort) {
	                    iCS_UserCommands.AutoLayoutPort(SelectedObject);
	                    Event.current.Use();                    
						break;
	                }
					if(SelectedObject.IsNode) {
						iCS_UserCommands.AutoLayoutPortsOnNode(SelectedObject);
	                    Event.current.Use();                    
						break;
					}
				}
                break;
            }
            // Wrap in package
            case KeyCode.W: {
                if(IsMultiSelectionActive) {
                    iCS_UserCommands.WrapMultiSelectionInPackage(IStorage);
                }
                else {
                    if(SelectedObject != null) {
                        iCS_UserCommands.WrapInPackage(SelectedObject);
                    }                    
                }
                Event.current.Use();
                break;
            }
            // Fold/Minimize/Maximize.
            case KeyCode.Return: {
				if(IsControlKeyDown) {
					if(SelectedObject == DisplayRoot) {
   						if(IStorage.HasBackwardNavigationHistory) {
      						iCS_UserCommands.ReloadFromBackwardNavigationHistory(IStorage);
      					}
		                Event.current.Use();
		                break;						
					}
					if(SelectedObject != null && SelectedObject.IsNode) {
						iCS_UserCommands.SetAsDisplayRoot(SelectedObject);						
					}
	                Event.current.Use();
	                break;						
				}
	            ProcessNodeDisplayOptionEvent();					
				Event.current.Use();
                break;
            }
            case KeyCode.H: {  // Show Help
                if(SelectedObject != null) {
					HelpController.openDetailedHelp(SelectedObject);
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
                if(myBookmark != null && myBookmark.IsDataOrControlPort && SelectedObject != null && SelectedObject.IsDataOrControlPort) {
                    VerifyNewConnection(myBookmark, SelectedObject);
                }
                Event.current.Use();
                break;
            }
            // Object deletion
            case KeyCode.Delete:
            case KeyCode.Backspace: {
				// First attempt to delete multi-selected objects.
				if(iCS_UserCommands.DeleteMultiSelectedObjects(IStorage)) {
	                Event.current.Use();
					break;
				}
                if(SelectedObject != null && SelectedObject != DisplayRoot && SelectedObject != StorageRoot &&
                   SelectedObject != DisplayRoot && !SelectedObject.IsFixDataPort) {
	                iCS_EditorObject parent= SelectedObject.Parent;
					bool changeSelected= true;
					iCS_UserCommands.DeleteObject(SelectedObject);
                    if(changeSelected) {
                        SelectedObject= parent;	
					}
                }
                Event.current.Use();
                break;
            }
        }        
    }
}
}