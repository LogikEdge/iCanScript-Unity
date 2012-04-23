using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_ClassWizard : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    iCS_ClassWizardController       myController= null;

    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnActivate(iCS_EditorObject target, iCS_IStorage storage) {
        // Transform invalid activation to a deactivation.
        if(target == null || storage == null) {
            myController= null;
            return;
        }
        if(myController == null ||
           (myController != null && (myController.Target != target || myController.IStorage != storage))) {
            myController= new iCS_ClassWizardController(target, storage);            
        }
        Repaint();
    }
    // ---------------------------------------------------------------------------------
    public void OnDeactivate() {
        myController= null;
        Repaint();
    }

    // =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        // Wait until window is configured.
        if(myController == null) return;
        EditorGUIUtility.LookLikeInspector();
        myController.View.Display(new Rect(0,0,position.width, position.height));
    }
}
