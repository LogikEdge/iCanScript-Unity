using UnityEngine;
using System.Collections;

public class iCS_InstanceEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_InstanceEditor   myEditor  = null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_InstanceEditor();
        Register(myEditor);
        myEditor.OnEnable();
    }
    void OnDisable() {
        if(myEditor != null) myEditor.OnDisable();
        Unregister(typeof(iCS_InstanceEditor));
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position);
    }
}
