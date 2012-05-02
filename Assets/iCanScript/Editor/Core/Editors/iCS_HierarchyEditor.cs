using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_HierarchyEditor : iCS_EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_IStorage				    myStorage;
    DSScrollView                    myMainView;
	iCS_ObjectHierarchyController   myController;
	    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public override void OnStorageChange() {
		myStorage= iCS_StorageMgr.IStorage;
        myController= new iCS_ObjectHierarchyController(myStorage[0], myStorage);
        myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, myController.View);
		Repaint();
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        iCS_StorageMgr.Update();
		if(myStorage == null) return;
		myMainView.Display(new Rect(0,0,position.width,position.height));
	}
}
