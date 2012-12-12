using UnityEngine;
using System;
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
            LocalPosition+= newPosition-GlobalPosition;
            if(IsParentValid) {
                Parent.LayoutParentNodeAfterDrag();
            }
            GlobalPosition= newPosition;
            SaveNodePosition();
            IsSticky= false;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
    // ----------------------------------------------------------------------
    // Adjust position of the siblings and the parent after an object drag.
    void LayoutParentNodeAfterDrag() {
		IsSticky= true;
        // Keep copy of child desired global position.
        var childrenRect= NodeGlobalChildRect;
        var childNodes= BuildListOfChildNodes(c=> !c.IsFloating);
        var nbOfChildren= childNodes.Length;
        var childGlobalPositionsFromRatio= new Vector2[nbOfChildren];
        for(int i= 0; i < nbOfChildren; ++i) {
            var c= childNodes[i];
            if(c.IsSticky) {
                childGlobalPositionsFromRatio[i]= c.GlobalPosition;
            } else {
                childGlobalPositionsFromRatio[i]= ComputeNodePositionFromRatio(childrenRect, c.NodePositionRatio);
            }            
        }
        // Resolve collision with siblings.
        ResolveCollisionOnChildren();
        // Adjust parent to wrap children.
        var previousGlobalRect= GlobalRect;
        WrapAroundChildrenNodes();
		// Ask parent to do the same if parent Rect has changed.
        var newGlobalRect= GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) {
			IsSticky= false;
			return;
		}
        // Parent rect has changed so lets recompute children ratio.
        childrenRect= NodeGlobalChildRect;
        for(int i= 0; i < nbOfChildren; ++i) {
            childNodes[i].NodePositionRatio= ComputeNodeRatio(childrenRect, childGlobalPositionsFromRatio[i]);            
        }
        // Move or resize the parent node.
        if(IsParentValid) {
            Parent.LayoutParentNodeAfterDrag();
  		}
		IsSticky= false;
    }

}
