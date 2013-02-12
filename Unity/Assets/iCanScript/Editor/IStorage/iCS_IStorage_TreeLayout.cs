using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
	// ======================================================================
    // ----------------------------------------------------------------------
	void ForcedRelayoutOfTree(iCS_EditorObject root) {
        ForEachRecursiveDepthFirst(root,
            obj=> {
            	if(!obj.IsVisibleInLayout) return;
				UpdateTransitionModuleName(obj);
				if(obj.IsNode) {
					obj.LayoutNode();
				}
            }
        );    		
	}
    // ----------------------------------------------------------------------
	void ForcedRedisplayOfTree(iCS_EditorObject root) {
        ForEachRecursiveDepthFirst(root,
            obj=> {
            	if(!obj.IsVisibleOnDisplay) return;
				UpdateTransitionModuleName(obj);
				if(obj.IsNode) {
					obj.LayoutNode();
				}
            }
        );    		
	}
    // ----------------------------------------------------------------------
	void UpdateTransitionModuleName(iCS_EditorObject obj) {
		if(!obj.IsTransitionModule) return;
        GetTransitionName(obj);		
	}
	
	// ======================================================================
    // ----------------------------------------------------------------------
    void PerformTreeLayoutFor(iCS_EditorObject branch) {
        ForEachRecursiveDepthFirst(branch,
            obj=> {
                if(obj.IsDirty) {
                    Layout(obj);
                }
            }
        );    
    }
    // ----------------------------------------------------------------------
    public void Layout(iCS_EditorObject obj) {
        obj.IsDirty= false;
        ExecuteIf(obj, o=> o.IsNode, o=> NodeLayout(o));
    }

    // ----------------------------------------------------------------------
    // Recompute the layout of a parent node.
    // Returns "true" if the new layout is within the window area.
    public void NodeLayout(iCS_EditorObject node, bool needsToBeCentered= false) {
        // Don't layout node if it is not visible.
        if(!node.IsVisibleInLayout) return;

        // Update transition module name
        if(node.IsTransitionModule) {
            GetTransitionName(node);
        }

        // Minimized nodes are fully collapsed.
        if(node.IsIconizedInLayout) {
            node.LayoutSize= iCS_Graphics.GetMaximizeIconSize(node);
            return;
        }

        // Update node size according to the current children layout.
        node.LayoutNodes();
    }

}
