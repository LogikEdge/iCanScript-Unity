using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Resolve collision between children then wrap node around the children.
    public void LayoutNodes() {
        UpdateNodeLayoutSize();
    }
    // ----------------------------------------------------------------------
    // Layout the nodes from the parent of the object moving up the hierarchy
    // until we reach the top.  The sticky bit is carried over from the object
    // to the parent.
    void LayoutParentNodesUntilTop() {
        var parent= ParentNode;
        if(parent == null) return;
        var parentGlobalRect= parent.GlobalLayoutRect;
        parent.ResolveCollisionOnChildrenNodes();
        parent.WrapAroundChildrenNodes();
        if(Math3D.IsNotEqual(parentGlobalRect, parent.GlobalLayoutRect)) {
            parent.IsSticky= IsSticky;
            parent.LayoutParentNodesUntilTop();
            parent.IsSticky= false;
        }
    }
}

