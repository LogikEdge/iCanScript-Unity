using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSCellView                  myMainView  = null;
    iCS_ClassWizardController   myController= null;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	void UpdateView() {
		if(IStorage == null || SelectedObject == null || !SelectedObject.IsClassModule) {
			myMainView= null;
			return;
		}
		// Update main view if selection has changed.
        if(myMainView == null ||
           (myController != null && (myController.Target != SelectedObject || myController.IStorage != IStorage))) {
               myController= new iCS_ClassWizardController(SelectedObject, IStorage);            
               myMainView  = new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), true, myController.View);
        }		
	}
	
    // =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public override void OnGUI() {
		// Update storage manager.
        UpdateMgr();
		if(myController == null || myController.Target != IStorage.SelectedObject) UpdateView();
        // Wait until window is configured.
        if(myMainView == null) return;
        EditorGUIUtility.LookLikeInspector();
        myMainView.Display(new Rect(0,0,position.width, position.height));
    }
}
