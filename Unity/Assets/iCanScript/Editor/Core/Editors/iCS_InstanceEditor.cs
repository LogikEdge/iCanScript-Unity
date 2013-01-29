using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_InstanceEditor : iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSCellView              myMainView  = null;
    iCS_InstanceController  myController= null;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public new void OnDisable() {
        base.OnDisable();
        myMainView= null;
        myController= null;
    }
    bool IsInitialized() {
		if(IStorage == null || SelectedObject == null || !SelectedObject.IsClassModule) {
			myMainView= null;
			myController= null;
			return false;
		}
		// Update main view if selection has changed.
        if(myMainView == null || myController == null ||
           (myController != null && (myController.Target != SelectedObject || myController.IStorage != IStorage))) {
               myController= new iCS_InstanceController(SelectedObject, IStorage);            
               myMainView  = new DSCellView(new RectOffset(0,0,kSpacer,0), true, myController.View);
        }		
        return true;
    }
	
    // =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public override void OnGUI() {
		// Update storage manager.
        UpdateMgr();
        // Wait until window is configured.
        if(!IsInitialized()) return;
        EditorGUIUtility.LookLikeInspector();
        myMainView.Display(new Rect(0,0,position.width, position.height));
    }
}
