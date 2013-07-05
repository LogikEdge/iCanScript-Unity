using UnityEngine;
using System.Collections;

public class iCS_CodeGenerator {
    // ======================================================================
	// Start Code generator service.
    // ----------------------------------------------------------------------
	static iCS_CodeGenerator() {
        iCS_EditorObject.OnNodeCreated     += OnNodeCreated;
        iCS_EditorObject.OnWillDestroyNode += OnWillDestroyNode;
        iCS_SystemEvents.OnHierarchyChanged+= OnHierarchyChanged;				
	}
	public static void Start() {}

    // ----------------------------------------------------------------------
    // Generate code according to created node.
    static void OnNodeCreated(iCS_EditorObject node) {
        // Install behaviour component
        if(node.IsBehaviour) {
            iCS_MenuUtility.UpdateBehaviourComponent(node.Storage.gameObject);
        }
		// Update Behaviour code if message has been added.
		if(node.IsBehaviourMessage) {
			GenerateBehaviourCode(node.Parent);
		}
    }
    // ----------------------------------------------------------------------
    // Generate code according to node destruction.
    static void OnWillDestroyNode(iCS_EditorObject node) {
        // Remove behaviour code.
        if(node.IsBehaviour) {
            iCS_CEGenerator.RemoveBehaviourCode(node);
        }
        // Regenerate behaviour code if message is removed.
        // TODO: should avoid multiple regeneration on behaviour delete.
		if(node.IsBehaviourMessage) {
			GenerateBehaviourCode(node.Parent);
		}
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
    
    // ----------------------------------------------------------------------
	static void GenerateBehaviourCode(iCS_EditorObject behaviour) {
        iCS_CSBehaviourTemplates.GenerateBehaviourCode(behaviour);
	}
}
