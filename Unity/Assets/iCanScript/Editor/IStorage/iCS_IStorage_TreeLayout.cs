using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    
	// ======================================================================
    // ----------------------------------------------------------------------
	public void ForcedRelayoutOfTree() {
        // Get a copy of the sticky position to maintain.
        var stickyObject= SelectedObject ?? DisplayRoot;
        var stickyPosition= stickyObject.GlobalPosition;
        ForcedRelayoutOfTree(stickyObject, stickyPosition);
	}
    // ----------------------------------------------------------------------
	public void ForcedRelayoutOfTree(iCS_EditorObject stickyObject, Vector2 stickyPosition) {
        // Advise of the start of a new layout
        SendStartRelayoutOfTree();

        // Recalculate the layout
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
        
//        // Move the entire graph to maintain the sticky object position
//        var newStickyPosition= stickyObject.GlobalPosition;
//        if(Math3D.IsNotEqual(newStickyPosition, stickyPosition)) {
//            var stickyOffset= stickyPosition-newStickyPosition;
//            DisplayRoot.CollisionOffset+= stickyOffset;
//        }
        
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
    
	// ======================================================================
    // ----------------------------------------------------------------------
    public void ReduceCollisionOffset() {
        ForEachRecursiveDepthFirst(DisplayRoot,
            obj=> {
                // Nothing to do if not visible in layout.
            	if(!obj.IsVisibleInLayout) {
            	    return;
        	    }
                // Nothing to do if not a node.
                if(!obj.IsNode || obj == DisplayRoot) {
                    return;
                }
                var parent= obj.ParentNode;
                var parentWrappingOffset= parent.WrappingOffset;
                obj.LocalAnchorPosition+= obj.CollisionOffset+parentWrappingOffset;
                obj.CollisionOffset= -parentWrappingOffset;
            }
        );
        
    }
}
