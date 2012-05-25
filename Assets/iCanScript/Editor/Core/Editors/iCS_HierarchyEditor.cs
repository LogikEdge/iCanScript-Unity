using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_HierarchyEditor : iCS_EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSScrollView                    myMainView;
	iCS_ObjectHierarchyController   myController;
	    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public override void OnStorageChange() {
        if(IStorage == null) return;
        myController= new iCS_ObjectHierarchyController(IStorage[0], IStorage);
        myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, true, myController.View);
		Repaint();
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        iCS_EditorMgr.Update();
		if(IStorage == null) return;
        var frameArea= new Rect(0,0,position.width,position.height);
		myMainView.Display(frameArea);
		ProcessEvents(frameArea);
	}
	// ----------------------------------------------------------------------
    void OnInspectorUpdate() {
        Repaint();
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
				break;
			}
            case EventType.MouseUp: {
				break;
			}
			case EventType.KeyDown: {
				var ev= Event.current;
				if(ev.keyCode == KeyCode.None) break;
                switch(ev.keyCode) {
                    // Delete object under cursor.
                    case KeyCode.Backspace:
                    case KeyCode.Delete: {
                        iCS_EditorUtility.SafeDestroyObject(selected, IStorage);
                        myController.Selected= null;
                        ev.Use();
                        break;
                    }
                    case KeyCode.F: {
                        if(selected != null) {
                            iCS_EditorUtility.SafeSelectAndMakeVisible(selected, IStorage);
                        }
                        Event.current.Use();
                        break;
                    }
                    // Remove name edition.
                    case KeyCode.Escape: {
                        myController.NameEdition= false;
                        ev.Use();
                        break;
                    }
                    // Tree navigation
                    case KeyCode.UpArrow: {
                        break;
                    }
                    case KeyCode.DownArrow: {
                        break;
                    }
                    // Fold/Minimize/Maximize.
                    case KeyCode.Return: {
                        break;
                    }
                    case KeyCode.H: {  // Show Help
                        break;
                    }
                    // myBookmarks
                    case KeyCode.B: {  // myBookmark selected object
                        Event.current.Use();
                        break;
                    }
                    case KeyCode.G: {  // Goto bookmark
                        Event.current.Use();
                        break;
                    }
                }
                break;
			}
        }   
    }
}
