using UnityEngine;
using System.Collections;

public static class iCS_CodeGenerator {
    // ======================================================================
	// Start Code generator service.
    // ----------------------------------------------------------------------
	static iCS_CodeGenerator() {
        iCS_EditorObject.OnNodeCreated     += OnNodeCreated;
        iCS_EditorObject.OnWillDestroyNode += OnWillDestroyNode;
        iCS_SystemEvents.OnHierarchyChanged+= OnHierarchyChanged;
        iCS_SystemEvents.OnEditorStarted   += OnEditorStarted;				
	}
	public static void Start() {}

    // ----------------------------------------------------------------------
    // Generate code according to created node.
    static void OnNodeCreated(iCS_EditorObject node) {
    }
    // ----------------------------------------------------------------------
    // Generate code according to node destruction.
    static void OnWillDestroyNode(iCS_EditorObject node) {
    }

    // ----------------------------------------------------------------------
    static void OnEditorStarted() {
        iCS_CSGenerateBehaviour.UpdateBehaviourCode();        
    }
    
    // ----------------------------------------------------------------------
    // Update Visual Script to Behaviour relationship.
    static void OnHierarchyChanged() {
        var allGameObjects= GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
        if(allGameObjects != null) {
            foreach(var go in allGameObjects) {
                iCS_MenuUtility.UpdateBehaviourComponent(go);
            }
        }
    }
}
