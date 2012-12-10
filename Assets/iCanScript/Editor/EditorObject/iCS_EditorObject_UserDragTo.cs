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
            LocalPosition+= newPosition-GlobalPosition;
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
        // Nothing else to do if this is the root object.
        if(!IsParentValid) return;
        // Now we need to reformat the parent...
        var parent= Parent;
        // Keep copy of child desired global position.
        var childrenRect= parent.NodeGlobalChildRect;
        var childNodes= parent.BuildListOfChildNodes(c=> !c.IsFloating);
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
        parent.ResolveCollisionOnChildren();
        // Adjust parent to wrap children.
        var previousGlobalRect= parent.GlobalRect;
        parent.WrapAroundChildrenNodes();
		// Ask parent to do the same if parent Rect has changed.
        var newGlobalRect= parent.GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) return;
        // Parent rect has changed so lets recompute children ratio.
        childrenRect= parent.NodeGlobalChildRect;
        for(int i= 0; i < nbOfChildren; ++i) {
            childNodes[i].NodePositionRatio= ComputeNodeRatio(childrenRect, childGlobalPositionsFromRatio[i]);            
        }
        // Move or resize the parent node.
        parent.IsSticky= true;
        parent.NodeAdjustAfterDrag();
        parent.IsSticky= false;
    }


}
