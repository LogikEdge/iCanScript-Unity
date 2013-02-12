using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void UserDragTo(Vector2 newPosition) {
		if(IsNode) {
            IsSticky= true;
            SetGlobalAnchorAndLayoutPosition(newPosition);
            LayoutParentNodesUntilTop();
            IsSticky= false;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
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
