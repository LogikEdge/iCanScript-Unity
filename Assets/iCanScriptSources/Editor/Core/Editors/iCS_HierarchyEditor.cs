using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_HierarchyEditor : iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSScrollView            myMainView;
	iCS_HierarchyController myController;
	Rect                    mySelectedAreaCache= new Rect(0,0,0,0);
	int                     myLastFocusId= -1;
	int                     myModificationId= -1;
	    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public override void OnStorageChange() {
        if(IStorage == null) return;
        myModificationId= IStorage.ModificationId;
        if(myController == null) {
            myController= new iCS_HierarchyController(IStorage[0], IStorage);            
        } else {
            myController.Init(IStorage[0], IStorage);
        }
        myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, true, myController.View);
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public override void OnGUI() {
        UpdateMgr();
		if(IStorage == null) return;
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
        myController.ShowElement(eObj);
    }
	// ----------------------------------------------------------------------
	Rect ShowToolbar() {
		var toolbarRect= iCS_ToolbarUtility.BuildToolbar(position.width);
		string searchString= myController.SearchString ?? "";
		myController.SearchString= iCS_ToolbarUtility.Search(ref toolbarRect, 120.0f, searchString, 0, 0, true);
		return toolbarRect;
	}
	// ----------------------------------------------------------------------
    void OnInspectorUpdate() {
        if(IStorage == null) return;
        // Verify for change within storage.
        if(IStorage.ModificationId != myModificationId) {
            OnStorageChange();
        }
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
                MyWindow.Focus();
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
                        iCS_EditorUtility.SafeDestroyObject(selected, IStorage);
                        myController.Selected= null;
                        ev.Use();
                        break;
                    }
                    // Remove name edition.
                    case KeyCode.Escape: {
                        myController.NameEdition= false;
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
                        if(iCS_EditorUtility.IsCurrentModificationId(myLastFocusId, IStorage)) {
                            /*
                                FIXME: Undo creates a null exception error.
                            */
                            iCS_EditorUtility.UndoIfModificationId(myLastFocusId, IStorage);
                            myLastFocusId= -1;
                        } else {
                            if(selected != null) {
                                myLastFocusId= iCS_EditorUtility.SafeSelectAndMakeVisible(selected, IStorage);
                                MyWindow.Focus();
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
