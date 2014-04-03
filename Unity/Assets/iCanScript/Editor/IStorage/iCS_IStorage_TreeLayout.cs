using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // ----------------------------------------------------------------------
	public void ForcedRelayoutOfTree(iCS_EditorObject root) {
        SendStartRelayoutOfTree();
        ForEachRecursiveDepthFirst(root,
            obj=> {
                // Nothing to do if not visible in layout.
            	if(!obj.IsVisibleInLayout) {
            	    return;
        	    }
                // Layout all nodes (ports will also be updated).
				if(obj.IsNode) {
					obj.LayoutNode();
				}
            }
        );
        SendEndRelayoutOfTree();
	}
	// ----------------------------------------------------------------------
    void SendStartRelayoutOfTree() {
        var visualEditor= iCS_EditorMgr.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == this) {
            visualEditor.OnStartRelayoutOfTree();
        }
    }
	// ----------------------------------------------------------------------
    void SendEndRelayoutOfTree() {
        var visualEditor= iCS_EditorMgr.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == this) {
            visualEditor.OnEndRelayoutOfTree();
        }
    }
}
