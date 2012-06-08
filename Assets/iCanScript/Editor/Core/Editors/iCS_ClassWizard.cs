using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : iCS_EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	bool						myNeedsViewUpdate= true;
    DSAccordionView             myMainView       = null;
    iCS_ClassWizardController   myController     = null;
    iCS_HierarchyController	    myTreeView       = null;
    DSCellView                  myInspectorView  = null;
    
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public override void OnSelectedObjectChange() {
		myNeedsViewUpdate= true;
	}
	void UpdateView() {
		myNeedsViewUpdate= false;
		if(IStorage == null || SelectedObject == null || !SelectedObject.IsClassModule) {
			myMainView= null;
			return;
		}
		// Update main view if selection has changed.
        if(myMainView == null ||
           (myController != null && (myController.Target != SelectedObject || myController.IStorage != IStorage))) {
               myController   = new iCS_ClassWizardController(SelectedObject, IStorage);            
               myTreeView     = new iCS_HierarchyController(SelectedObject, IStorage);
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
		iCS_EditorMgr.Update();
		if(myNeedsViewUpdate) UpdateView();
        // Wait until window is configured.
        if(myMainView == null) return;
        EditorGUIUtility.LookLikeInspector();
        myMainView.Display(new Rect(0,0,position.width, position.height));
    }
}
