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
        myEditor.OnEnable();
        Register(myEditor.GetType().Name, myEditor);
    }
    void OnDisable() {
        Unregister(myEditor.GetType().Name);
        myEditor.OnDisable();
        myEditor= null;
    }
    
    // =================================================================================
    // Internal storage has change.
    // ---------------------------------------------------------------------------------
	protected override void OnStorageChange() {
        myEditor.OnStorageChange(myIStorage);
        Repaint();
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position, myIStorage);
    }
}
