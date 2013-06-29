using UnityEngine;
using System.Collections;

public class iCS_CodeGenerator {
    // ======================================================================
	// Start Code generator service.
    // ----------------------------------------------------------------------
	static iCS_CodeGenerator() {
        iCS_EditorObject.OnNodeCreated    += OnNodeCreated;
        iCS_EditorObject.OnWillDestroyNode+= OnWillDestroyNode;				
	}
	public static void Start() {}

    // ----------------------------------------------------------------------
    // Generate code according to created node.
    static void OnNodeCreated(iCS_EditorObject node) {
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
	static void GenerateBehaviourCode(iCS_EditorObject behaviour) {
        iCS_CEGenerator.GenerateBehaviourCode(behaviour);
	}
}
