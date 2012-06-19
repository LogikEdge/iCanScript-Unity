using UnityEngine;
using System.Collections;

public class iCS_LibraryEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_LibraryEditor   myEditor= null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_LibraryEditor();
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
        myEditor.OnGUI();
    }
}
