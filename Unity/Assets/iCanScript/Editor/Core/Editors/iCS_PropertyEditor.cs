using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_PropertyEditor : iCS_EditorBase {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    DSCellView              myMainView         = null;
    iCS_PropertyController  myController       = null;
    bool                    myNotificationShown= false;
	
    // =================================================================================
    // Constants
    // ---------------------------------------------------------------------------------
    const int   kSpacer= 8;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
	public static void Init(iCS_IStorage iStorage) {
		// Get existing open window or if none, make a new one:
		var editor = (iCS_PropertyEditor)EditorWindow.GetWindow (typeof (iCS_PropertyEditor));
		editor.IStorage= iStorage;
		editor.ShowUtility();
	}
	public void Update() {
		if(IStorage != null) {
			var selectedObj= IStorage.SelectedObject;
			if(selectedObj == null || !selectedObj.IsInstanceNode) {
				IStorage= null;
				Close();
			}
		}
	}
	
    // ---------------------------------------------------------------------------------
	public new void OnEnable() {
		base.OnEnable();
		title= "Property";
	}
    public new void OnDisable() {
        base.OnDisable();
        myMainView= null;
        myController= null;
    }
    bool IsInitialized() {
        var targetObject= SelectedObject;
        // Move to the parent node if selected is a port.
        if(targetObject != null && targetObject.IsPort) {
            targetObject= targetObject.ParentNode;
        }
		if(IStorage == null || targetObject == null || !(targetObject.IsInstanceNode || targetObject.IsBehaviour)) {
			myMainView= null;
			myController= null;
			return false;
		}
		// Update main view if selection has changed.
        if(myMainView == null || myController == null ||
           (myController != null && (myController.Target != targetObject || myController.IStorage != IStorage))) {
               myController= new iCS_PropertyController(targetObject, IStorage);            
               myMainView  = new DSCellView(new RectOffset(0,0,kSpacer,0), true, myController.View);
        }		
        return true;
    }
	
    // =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    public new void OnGUI() {
        // Draw common stuff for all editors
        base.OnGUI();
        
		// Update storage manager.
        UpdateMgr();
        // Wait until window is configured.
        if(!IsInitialized() || IStorage == null) {
            // Tell the user that we can display without a behavior or library.
            ShowNotification(new GUIContent("No instance node selected !!!"));
            myNotificationShown= true;
            return;            
        }

        // Remove any previously shown notification.
        if(myNotificationShown) {
            RemoveNotification();
            myNotificationShown= false;
        }
        
        myMainView.Display(new Rect(0,0,position.width, position.height));
    }
}
