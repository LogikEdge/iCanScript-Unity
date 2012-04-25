using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_TreeEditor : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject	myRoot;
	iCS_IStorage		myStorage;
	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnActivate(iCS_EditorObject rootObject, iCS_IStorage storage) {
		myRoot= rootObject;
		myStorage= storage;
	}
	public void OnDeactivate() {
	}

	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
	}
}
