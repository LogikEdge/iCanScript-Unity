using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Layout the nodes from the parent of the object moving up the hierarchy
    // until we reach the top.  The sticky bit is carried over from the object
    // to the parent.
    void LayoutParentNodesUntilTop() {
        var parent= ParentNode;
        if(parent == null) return;
        var parentGlobalRect= parent.GlobalLayoutRect;
        parent.LayoutNode();
        if(Math3D.IsNotEqual(parentGlobalRect, parent.GlobalLayoutRect)) {
            parent.IsSticky= IsSticky;
            parent.LayoutParentNodesUntilTop();
            parent.IsSticky= false;
        }
    }
    // ----------------------------------------------------------------------
	public void LayoutNode() {
        ResolveCollisionOnChildrenNodes();
        WrapAroundChildrenNodes();
		LayoutPorts();
	}
}

