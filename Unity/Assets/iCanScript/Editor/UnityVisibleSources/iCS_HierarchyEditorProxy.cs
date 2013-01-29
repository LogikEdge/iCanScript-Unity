using UnityEngine;
using System.Collections;

public class iCS_HierarchyEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_HierarchyEditor   myEditor= null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_HierarchyEditor();
        Register(myEditor);
        myEditor.OnEnable();
    }
    void OnDisable() {
        if(myEditor != null) myEditor.OnDisable();
        Unregister(typeof(iCS_HierarchyEditor));
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position);
    }
}
