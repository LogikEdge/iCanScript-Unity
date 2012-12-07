using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE USER DRAG
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void UserDragTo(Vector2 newPosition) {
		if(IsNode) {
            IsSticky= true;
            DeltaPosition= newPosition-GlobalPosition;
            LocalPosition+= DeltaPosition;
            NodeAdjustAfterDrag();
            GlobalPosition= newPosition;
            SaveNodePosition();
            IsSticky= false;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
    // ----------------------------------------------------------------------
    // Adjust position of the siblings and the parent after an object drag.
    void NodeAdjustAfterDrag() {
        // Get a snapshot of the delta position.
        var delta= DeltaPosition;
        DeltaPosition= Vector2.zero;
        // Nothing else to do if this is the root object.
        if(!IsParentValid) return;
        var parent= Parent;
        // Keep copy of child desired global position.
        var childrenRect= parent.NodeGlobalChildRect;
        parent.ForEachChildNode(
            c=> {
                if(c.IsSticky) {
                    c.myGlobalPositionFromRatio= c.GlobalPosition;
                } else {
                    c.myGlobalPositionFromRatio= ComputeNodePositionFromRatio(childrenRect, c.NodePositionRatio);
                }
            }
        );
        // Resolve collision with siblings.
        parent.ResolveCollisionOnChildren(delta);
        // Adjust parent to wrap children.
        var previousGlobalRect= parent.GlobalRect;
        parent.WrapAroundChildrenNodes();
		// Ask parent to do the same if parent Rect has changed.
        var newGlobalRect= parent.GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) return;
        // Parent rect has changed so lets recompute children ratio.
        childrenRect= parent.NodeGlobalChildRect;
        parent.ForEachChildNode(
            c=> {
                c.NodePositionRatio= ComputeNodeRatio(childrenRect, c.myGlobalPositionFromRatio);
            }
        );
        // Move or resize the parent node.
		parent.DeltaPosition= Math3D.Middle(newGlobalRect)-Math3D.Middle(previousGlobalRect);
        parent.IsSticky= true;
        parent.NodeAdjustAfterDrag();
        parent.IsSticky= false;
    }


}
