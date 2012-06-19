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
        myEditor.OnEnable();
        Register(myEditor.GetType().Name, myEditor);
    }
    void OnDisable() {
        Unregister(myEditor.GetType().Name);
        myEditor.OnDisable();
        myEditor= null;
    }
    
	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
        myEditor.OnGUI(position, myIStorage);
    }
}
