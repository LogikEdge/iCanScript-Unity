using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    
	// ======================================================================
    // ----------------------------------------------------------------------
	public void ForcedRelayoutOfTree() {
        // Advise of the start of a new layout
        SendStartRelayoutOfTree();

        // Get a copy of the sticky position to maintain.
        var stickyObject= SelectedObject ?? DisplayRoot;
        var stickyPosition= stickyObject.GlobalPosition;
        
        ForEachRecursiveDepthFirst(DisplayRoot,
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
        
        // Move the entire graph to maintain the sticky object position
        var newStickyPosition= stickyObject.GlobalPosition;
        if(Math3D.IsNotEqual(newStickyPosition, stickyPosition)) {
            var stickyOffset= stickyPosition-newStickyPosition;
            DisplayRoot.CollisionOffset+= stickyOffset;
        }
        
        // Advise that the layout has completed.
        SendEndRelayoutOfTree();
	}
	// ----------------------------------------------------------------------
    void SendStartRelayoutOfTree() {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == this) {
            visualEditor.OnStartRelayoutOfTree();
        }
    }
	// ----------------------------------------------------------------------
    void SendEndRelayoutOfTree() {
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null && visualEditor.IStorage == this) {
            visualEditor.OnEndRelayoutOfTree();
        }
    }
}
