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
//		Debug.Log("Focus: "+((bool)(EditorWindow.focusedWindow == this)));
	}
	// ----------------------------------------------------------------------
    void ProcessEvents(Rect frameArea) {
     	Vector2 mousePosition= Event.current.mousePosition;
		switch(Event.current.type) {
            case EventType.ScrollWheel: {
                break;
            }
            case EventType.MouseDown: {
                /*
                    FIXME: Should remove selection when clicking outside scroll view.
                */
//                myController.MouseDownOn(null, frameArea);
//                Event.current.Use();
				break;
			}
            case EventType.MouseUp: {
				break;
			}
        }   
    }
}
