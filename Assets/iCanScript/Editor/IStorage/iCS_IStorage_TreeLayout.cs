#define NEW_LAYOUT

using UnityEngine;
using System.Collections;

public partial class iCS_IStorage {
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
        //Debug.Log(obj.Name+" is dirty.  Display option: "+obj.DisplayOption);
		Debug.Log("Name: "+obj.Name+" GPos= "+obj.GlobalPosition);
    }

    // ----------------------------------------------------------------------
    // Recompute the layout of a parent node.
    // Returns "true" if the new layout is within the window area.
    public void NodeLayout(iCS_EditorObject node, bool needsToBeCentered= false) {
        // Don't layout node if it is not visible.
        if(!node.IsVisible) return;

        // Update transition module name
        if(node.IsTransitionModule) {
            GetTransitionName(node);
        }

        // Minimized nodes are fully collapsed.
        if(node.IsIconized) {
            node.DisplaySize= iCS_Graphics.GetMaximizeIconSize(node);
            return;
        }

        // Update node size according to the current children layout.
        node.LayoutNode();
    }

}
