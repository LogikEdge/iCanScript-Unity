using UnityEngine;
using System.Collections;

public class iCS_EditorCodeGenerator {
    // ======================================================================

    // ----------------------------------------------------------------------
    // Register event callbacks.
    static iCS_EditorCodeGenerator() {
        iCS_EditorObject.OnNodeCreated    += OnNodeCreated;
        iCS_EditorObject.OnWillDestroyNode+= OnWillDestroyNode;
    }

    // ----------------------------------------------------------------------
    // Generate code according to created node.
    static void OnNodeCreated(iCS_EditorObject node) {
        Debug.Log("Node created: "+node.Name);
    }
    // ----------------------------------------------------------------------
    // Generate code according to node destruction.
    static void OnWillDestroyNode(iCS_EditorObject node) {
        Debug.Log("Will destroy node: "+node.Name);
    }
}
