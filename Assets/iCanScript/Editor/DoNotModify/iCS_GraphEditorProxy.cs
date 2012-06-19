using UnityEngine;
using System.Collections;

public class iCS_GraphEditorProxy : iCS_EditorWindow {
    // =================================================================================
    // The editor implementation.
    // ---------------------------------------------------------------------------------
    iCS_GraphEditor   myEditor= null;
    
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    void OnEnable() {
        myEditor= new iCS_GraphEditor();
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
