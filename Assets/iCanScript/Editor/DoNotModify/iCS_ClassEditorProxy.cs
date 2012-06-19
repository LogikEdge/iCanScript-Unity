using UnityEngine;
using System.Collections;

public class iCS_ClassEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_ClassWizard   myEditor  = null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_ClassWizard();
        Register(myEditor.GetType().Name, myEditor);
        myEditor.OnEnable();
    }
    void OnDisable() {
        myEditor.OnDisable();
        Unregister(myEditor.GetType().Name);
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position);
    }
}
