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
    // Initialization.
    // ---------------------------------------------------------------------------------
    protected new void OnEnable() {
		base.OnEnable();
        iCS_StorageMgr.RegisterStorageChangeNotification(SetStorage);
    }
    protected new void OnDisable() {
		base.OnDisable();
        iCS_StorageMgr.UnregisterStorageChangeNotification(SetStorage);
    }
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public override void OnActivate(iCS_EditorObject rootObject, iCS_IStorage storage) {
        SetStorage(storage);
	}
	public override void OnDeactivate() {
		myStorage= null;
	}
    void SetStorage(iCS_IStorage iStorage) {
		myStorage= iStorage;
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
