using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildrenNodes() {
		// Prepare to animate Drag node sibling.
		List<Vector2> initialChildNodePos= new List<Vector2>();
		if(this == ourDragObjectParent) {
			ForEachChildNode(
				c=> {
					var animStartValue= c.GlobalLayoutPosition;
					initialChildNodePos.Add(animStartValue);
					c.SetPositionAnimationStartValue(animStartValue);
				}
			);
		}
        // Reposition all node at their anchor position.
        ForEachChildNode(c=> c.GlobalLayoutPosition= c.GlobalAnchorPosition);
        // Resolve collisions.
        ResolveCollisionOnChildrenImp();
		// Only animate drag node sibling if they move in opposite direction.
		if(this == ourDragObjectParent) {
			float dragMagnitude= ourDragObjectDelta.magnitude;
			int i= 0;
			ForEachChildNode(
				c=> {
					var newGlobalPos= c.GlobalLayoutPosition;
					if(c.IsPositionAnimated && Math3D.IsNotEqual(AnimatedPosition.TargetValue, newGlobalPos)) {
						c.StartDisplayPositionAnimation();						
					}
					var move= newGlobalPos-initialChildNodePos[i++];
					if(Math3D.IsGreater(move.magnitude, dragMagnitude)) {
						if(Vector2.Dot(ourDragObjectDelta, move) < 0) {
							c.StartDisplayPositionAnimation();
						}
					}
				}
			);
		}
    }
    // ----------------------------------------------------------------------
    public void ResolveCollisionOnChildrenImp() {
        bool didCollide= false;
        iCS_EditorObject[] children= BuildListOfChildNodes(c=> !c.IsFloating);
        // Resolve collisions.
        for(int i= 0; i < children.Length-1; ++i) {
            for(int j= i+1; j < children.Length; ++j) {
                didCollide |= children[i].ResolveCollisionBetweenTwoNodes(children[j]);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildrenImp();
    }
    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject theOtherNode) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithMargins(theOtherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(theOtherNode);
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

        // Seperate by half penetration if none is sticky.
        if(!IsSticky && !theOtherNode.IsSticky) {
            penetration*= 0.5f;
            theOtherNode.LocalLayoutPosition+= penetration;
            LocalLayoutPosition-= penetration;
            return true;            
        }
		// Seperate using the known movement.
    	if(!theOtherNode.IsSticky) {
            theOtherNode.LocalLayoutPosition+= penetration;
            return true;
    	}
        if(!IsSticky) {            
    		LocalLayoutPosition-= penetration;
    		return true;
    	}            
        return false;
    }
    // ----------------------------------------------------------------------
    // Returns true if the given rectangle collides with the node.
    public bool DoesCollideWithMargins(iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(AddMargins(GlobalLayoutRect), otherNode.GlobalLayoutRect);
    }
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.  The vector
	// returned is the smallest distance to remove the overlap.
	Vector2 GetSeperationVector(iCS_EditorObject theOther) {
        Rect myRect   = AddMargins(GlobalLayoutRect);
        Rect otherRect= theOther.GlobalLayoutRect;
		// No collision if X & Y distance of the enclosing rect is either
		// larger or higher then the total width/height.
        float xMin= Mathf.Min(myRect.xMin, otherRect.xMin);
        float yMin= Mathf.Min(myRect.yMin, otherRect.yMin);
        float xMax= Mathf.Max(myRect.xMax, otherRect.xMax);
        float yMax= Mathf.Max(myRect.yMax, otherRect.yMax);
        float xDistance= xMax-xMin;
        float yDistance= yMax-yMin;
        float totalWidth= myRect.width+otherRect.width;
        float totalHeight= myRect.height+otherRect.height;
        if(xDistance >= totalWidth) return Vector2.zero;
        if(yDistance >= totalHeight) return Vector2.zero;
		// A collision is detected.  The seperation vector is the
		// smallest distance to remove the overlap.  The separtion
		// must also respect the anchor position relationship
		// between the two overalpping nodes.
		var anchorSepDir= theOther.LocalAnchorPosition-LocalAnchorPosition;
		float sepX= anchorSepDir.x > 0 ? totalWidth-xDistance : xDistance-totalWidth;
		float sepY= anchorSepDir.y > 0 ? totalHeight-yDistance : yDistance-totalHeight;
		if(Mathf.Abs(sepX) < Mathf.Abs(sepY)) {
			return new Vector2(sepX, 0);
		}
		return new Vector2(0, sepY);
	}
}
