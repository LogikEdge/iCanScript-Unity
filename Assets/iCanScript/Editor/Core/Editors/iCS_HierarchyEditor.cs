using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_HierarchyEditor : iCS_EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	struct NodeInfo {
		public NodeInfo(bool isFolded) {
			IsFolded= isFolded;
		}
		public bool IsFolded;	
	}
	
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject			    myRoot;
	iCS_IStorage				    myStorage;
    DSScrollView                    myMainView;
	iCS_ObjectHierarchyController   myController;
	
    // =================================================================================
    // Initialization.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        iCS_IStorageMgr.Register(SetStorage);
    }
    void OnDisable() {
        iCS_IStorageMgr.Unregister(SetStorage);
    }
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public override void OnActivate(iCS_EditorObject rootObject, iCS_IStorage storage) {
		myRoot= rootObject;
        SetStorage(storage);
	}
	public override void OnDeactivate() {
		myRoot= null;
		myStorage= null;
	}
    void SetStorage(iCS_IStorage storage) {
		myStorage= storage;
		myRoot= myStorage.EditorObjects[0];
        myController= new iCS_ObjectHierarchyController(myRoot, myStorage);
        myMainView= new DSScrollView(new RectOffset(0,0,0,0), false, myController.View);
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        iCS_IStorageMgr.Update();
		if(myStorage == null) return;
		myMainView.Display(new Rect(0,0,position.width,position.height));
	}
}
