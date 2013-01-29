using UnityEngine;
using System.Collections;

public class iCS_PreferencesProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_PreferencesEditor   myEditor= null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_PreferencesEditor();
        Register(myEditor);
        myEditor.OnEnable();
    }
    void OnDisable() {
        if(myEditor != null) myEditor.OnDisable();
        Unregister(typeof(iCS_PreferencesEditor));
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position);
    }
}
