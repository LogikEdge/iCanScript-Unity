using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_VisualEditor : iCS_EditorBase {
	// ======================================================================
	// Properties
	// ----------------------------------------------------------------------
	bool    HasKeyboardFocus     { get { return EditorWindow.focusedWindow == this; }}
    bool    IsShiftKeyDown       { get { return Event.current.shift; }}
    bool    IsControlKeyDown     { get { return Event.current.control; }}
    bool    IsAltKeyDown         { get { return Event.current.alt; }}
    bool    IsFloatingKeyDown	 { get { return IsControlKeyDown; }}
    bool    IsCopyKeyDown        { get { return IsShiftKeyDown; }}
    bool    IsScaleKeyDown       { get { return IsAltKeyDown; }}
	bool	IsMultiSelectKeyDown { get { return Event.current.command; }}
    bool    IsDoubleClick        { get { return Event.current.clickCount >= 2; }}
	bool    IsIsolateKeyDown     { get { return IsAltKeyDown; }}
	
    
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
                    iCS_EditorUtility.SafeCenterOn(selected, IStorage);                        
                }
                Event.current.Use();
                break;
            }
			case KeyCode.P: {
				iCS_UserCommands.ToggleShowRootNode(IStorage);
                Event.current.Use();
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
