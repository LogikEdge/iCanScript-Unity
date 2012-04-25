using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class iCS_TreeEditor : EditorWindow {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	struct NodeInfo {
		public NodeInfo(bool isFolded) {
			IsFolded= isFolded;
		}
		public bool IsFolded;	
	}
	
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	iCS_EditorObject			myRoot;
	iCS_IStorage				myStorage;
	Dictionary<object,NodeInfo>	myNodes;
	
    // =================================================================================
    // Activation/Deactivation.
    // ---------------------------------------------------------------------------------
    public void OnActivate(iCS_EditorObject rootObject, iCS_IStorage storage) {
		myRoot= rootObject;
		myStorage= storage;
		myNodes= new Dictionary<object, NodeInfo>();
	}
	public void OnDeactivate() {
		myRoot= null;
		myStorage= null;
		myNodes= null;
	}

	// =================================================================================
    // Display.
    // ---------------------------------------------------------------------------------
    void OnGUI() {
		if(myStorage == null) return;
	}
}
