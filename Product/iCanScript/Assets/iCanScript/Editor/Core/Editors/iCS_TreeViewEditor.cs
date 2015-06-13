using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/*
    FIXME: Fix bug which does not properly initialize hiearchy view on code reload
*/
namespace iCanScript.Internal.Editor {
public class iCS_TreeViewEditor : iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSScrollView            myMainView;
	iCS_TreeViewController  myController;
	Rect                    mySelectedAreaCache= new Rect(0,0,0,0);
	int                     myLastFocusId      = -1;
	int                     myUndoRedoId       = -1;
    bool                    myNotificationShown= false;
	    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void Init() {
        myMainView= null;
        myController= null;
        mySelectedAreaCache= new Rect(0,0,0,0);
        myLastFocusId= -1;
        myUndoRedoId= -1;
    }
    public new void OnEnable() {
        base.OnEnable();
        
        // -- Set window title --
        Texture2D iCanScriptLogo= null;
        TextureCache.GetTexture(iCS_EditorStrings.TitleLogoIcon, out iCanScriptLogo);
        titleContent= new GUIContent("Tree View", iCanScriptLogo);
    }
    public new void OnDisable() {
        base.OnDisable();
        Init();
    }
    bool IsInitialized() {
        if(IStorage == null) {
            Init();
            return false;
        }
        if(myController == null || myController.IStorage != IStorage) {
            myController= new iCS_TreeViewController(IStorage[0], IStorage);                        
            myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, true, myController.View);            
            return true;
        }
        if(myMainView == null) {
            myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, true, myController.View);            
        }
        if(myUndoRedoId != IStorage.UndoRedoId) {
            myController.Init(IStorage[0], IStorage);
        }
        return true;        
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public new void OnGUI() {
        // Draw common stuff for all editors
        base.OnGUI();
        
        UpdateMgr();
		// Nothing to be drawn until we are fully initialized.
        if(!IsInitialized() || IStorage == null) {
            // Tell the user that we can display without a behavior or library.
            ShowNotification(new GUIContent("No iCanScript component selected !!!"));
            myNotificationShown= true;
            return;            
        }

        // Remove any previously shown notification.
        if(myNotificationShown) {
            RemoveNotification();
            myNotificationShown= false;
        }
            
		var toolbarRect= ShowToolbar();
        var frameArea= new Rect(0,toolbarRect.height,position.width,position.height-toolbarRect.height);
		myMainView.Display(frameArea);
		ProcessEvents(frameArea);
		// Make new selection visible
		if(mySelectedAreaCache != myController.SelectedArea) {
		    mySelectedAreaCache= myController.SelectedArea;
		    myMainView.MakeVisible(mySelectedAreaCache, frameArea);
		}
	}
	// ----------------------------------------------------------------------
    public void ShowElement(iCS_EditorObject eObj) {
        if(!IsInitialized()) return;
        myController.ShowElement(eObj);
        Repaint();            
    }
	// ----------------------------------------------------------------------
	Rect ShowToolbar() {
		var toolbarRect= iCS_ToolbarUtility.BuildToolbar(position.width);
		string searchString= myController.SearchString ?? "";
		myController.SearchString= iCS_ToolbarUtility.Search(ref toolbarRect, 120.0f, searchString, 0, 0, true);
		return toolbarRect;
	}
	// ----------------------------------------------------------------------
    void ProcessEvents(Rect frameArea) {
     	Vector2 mousePosition= Event.current.mousePosition;
        var selected= myController.Selected;
		switch(Event.current.type) {
            case EventType.ScrollWheel: {
                break;
            }
            case EventType.MouseDown: {
                var mouseInScreenPoint= GUIUtility.GUIToScreenPoint(mousePosition);
                var areaInScreenPoint= GUIUtility.GUIToScreenPoint(new Vector2(frameArea.x, frameArea.y));
                var areaInScreenPosition= new Rect(areaInScreenPoint.x, areaInScreenPoint.y, frameArea.width, frameArea.height);
                myController.MouseDownOn(null, mouseInScreenPoint, areaInScreenPosition);
                Event.current.Use();
                // Move keyboard focus to this window.
                Focus();
				break;
			}
            case EventType.MouseUp: {
				break;
			}
			case EventType.KeyDown: {
				var ev= Event.current;
				if(!ev.isKey) break;
                switch(ev.keyCode) {
                    // Tree navigation
                    case KeyCode.UpArrow: {
                        myController.SelectPrevious();
                        ev.Use();
                        break;
                    }
                    case KeyCode.DownArrow: {
                        myController.SelectNext();
                        ev.Use();
                        break;
                    }
                    // Delete object under cursor.
                    case KeyCode.Backspace:
                    case KeyCode.Delete: {
                        iCS_UserCommands.DeleteObject(selected);
                        myController.Selected= null;
                        ev.Use();
                        break;
                    }
                    // Fold/Unfold toggle
                    case KeyCode.Return: {
                        myController.ToggleFoldUnfoldSelected();
                        ev.Use();
                        break;
                    }
                }
                switch(ev.character) {
                    // Fold/Unfold.
                    case '+': {
                        myController.UnfoldSelected();
                        ev.Use();
                        break;
                    }
                    case '-': {
                        myController.FoldSelected();
                        ev.Use();
                        break;
                    }
                    case 'f': {
                        if(iCS_EditorUtility.IsCurrentUndoRedoId(myLastFocusId, IStorage)) {
                            /*
                                FIXME: Undo creates a null exception error.
                            */
                            iCS_EditorUtility.UndoIfUndoRedoId(myLastFocusId, IStorage);
                            myLastFocusId= -1;
                        } else {
                            if(selected != null) {
                                myLastFocusId= iCS_EditorUtility.SafeSelectAndMakeVisible(selected, IStorage);
                                Focus();
                                Event.current.Use();
                            }                            
                        }
                        break;
                    }
                }
                break;
			}
        }   
    }
}
}
