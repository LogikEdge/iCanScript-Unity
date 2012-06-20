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
    void OnSelectionChange() {
        myEditor.OnSelectionChange();
    }

}
