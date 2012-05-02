using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : iCS_EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSAccordionView             	myMainView     = null;
    iCS_ClassWizardController   	myController   = null;
    iCS_ObjectHierarchyController	myTreeView     = null;
    DSCellView                  	myInspectorView= null;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public override void OnSelectedObjectChange() {
		// Deactivate if we don't have an active object.
		iCS_EditorObject target= iCS_StorageMgr.SelectedObject;
		iCS_IStorage storage= iCS_StorageMgr.IStorage;
		if(storage == null || target == null || !target.IsClassModule) {
			myMainView= null;
			return;
		}
		// Update main view if selection has changed.
        if(myMainView == null ||
           (myController != null && (myController.Target != target || myController.IStorage != storage))) {
               myController   = new iCS_ClassWizardController(target, storage);            
               myTreeView     = new iCS_ObjectHierarchyController(target, storage);
               myInspectorView= new DSCellView(new RectOffset(kSpacer,kSpacer,kSpacer,kSpacer), true);
               myMainView     = new DSAccordionView(new RectOffset(kSpacer, kSpacer, kSpacer, kSpacer), true, 3);
               myMainView.AddSubview(new GUIContent("Wizard"), myController.View);
               myMainView.AddSubview(new GUIContent("Inspector"), myInspectorView);
               myMainView.AddSubview(new GUIContent("Tree View"), myTreeView.View);
        }		
	}
	
    // =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
		// Update storage manager.
		iCS_StorageMgr.Update();
		OnSelectedObjectChange();
        // Wait until window is configured.
        if(myMainView == null) return;
        EditorGUIUtility.LookLikeInspector();
        myMainView.Display(new Rect(0,0,position.width, position.height));
    }
}
