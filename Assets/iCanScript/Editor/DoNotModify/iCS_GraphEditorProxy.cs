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
    void Update() {
        myEditor.Update();
    }

}
