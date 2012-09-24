using UnityEngine;
using System.Collections;

public class iCS_VisualEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_VisualEditor   myEditor= null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_VisualEditor();
        Register(myEditor);
        myEditor.OnEnable();
    }
    void OnDisable() {
        if(myEditor != null) myEditor.OnDisable();
        Unregister(typeof(iCS_VisualEditor));
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position);
    }
    void Update() {
        myEditor.Update();
    }
}
