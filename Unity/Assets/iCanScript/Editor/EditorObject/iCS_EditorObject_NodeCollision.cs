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
		// Prepare to animate nodes affected by collisions.
		List<Vector2> childStartPos= new List<Vector2>();
		ForEachChildNode(
			c=> {
				childStartPos.Add(c.GlobalLayoutPosition);
			}
		);
        // Reposition all node at their anchor position.
        ForEachChildNode(c=> c.GlobalLayoutPosition= c.GlobalAnchorPosition);
        // Resolve collisions.
        ResolveCollisionOnChildrenImp();
		// Animate all nodes affected by collisions.
		int i= 0;
		ForEachChildNode(
			c=> {
				var targetPos= c.GlobalLayoutPosition;
				var startPos= childStartPos[i++];
                if(!IsSticky && Math3D.IsNotEqual(startPos, targetPos)) {
					c.SetPositionAnimationStartValue(startPos);
                    c.StartPositionAnimation(targetPos);
                }
			}
		);
    }
    // ----------------------------------------------------------------------
    public void ResolveCollisionOnChildrenImp() {
        iCS_EditorObject[] children= BuildListOfChildNodes(c=> !c.IsFloating);
        // Resolve collisions.
        bool didCollide= true;
		while(didCollide) {
			didCollide= false;
	        for(int i= 0; i < children.Length-1; ++i) {
				var c1= children[i];
				if(c1.IsFloating) continue;
	            for(int j= i+1; j < children.Length; ++j) {
					var c2= children[j];
					if(c2.IsFloating) continue;
					var r1= c1.GlobalLayoutRect;
					var r2= c2.GlobalLayoutRect;
	                didCollide |= c1.ResolveCollisionBetweenTwoNodes(c2, ref r1, ref r2);                            
	            }
	        }			
		}
    }
    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject theOtherNode,
												ref Rect myRect, ref Rect theOtherRect) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithMargins(myRect, theOtherRect)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(theOtherNode,
												 myRect,
												 theOtherRect);
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
    public static bool DoesCollideWithMargins(Rect r1, Rect r2) {
        return Math3D.DoesCollide(AddMargins(r1), r2);
    }
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.  The vector
	// returned is the smallest distance to remove the overlap.
	Vector2 GetSeperationVector(iCS_EditorObject theOther, Rect myRect, Rect otherRect) {
        myRect= AddMargins(myRect);
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
