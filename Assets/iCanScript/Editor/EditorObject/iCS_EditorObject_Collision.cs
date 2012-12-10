using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren() {
        // Reposition all non-sticky node according to ratio
        ForEachChildNode(
            c=> {
                if(!c.IsSticky) {
                    c.GlobalPosition= c.NodeGlobalPositionFromRatio;
                }
            }
        );
        // Resolve collisions.
        ResolveCollisionOnChildrenImp();
    }
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
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject otherNode) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithMargins(otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(otherNode.GlobalRect);
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

        // Seperate by half penetration if none is sticky.
        if(!IsSticky && !otherNode.IsSticky) {
            penetration*= 0.5f;
            otherNode.LocalPosition+= penetration;
            LocalPosition-= penetration;
            return true;            
        }
		// Seperate using the known movement.
    	if(!otherNode.IsSticky) {
            otherNode.LocalPosition+= penetration;
            return true;
    	}
        if(!IsSticky) {            
    		LocalPosition-= penetration;
    		return true;
    	}            
        return false;
    }
    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithMargins(iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(AddMargins(GlobalRect), otherNode.GlobalRect);
    }
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.  The vector
	// returned is the smallest distance to remove the overlap.
	Vector2 GetSeperationVector(Rect _rect) {
        Rect myRect= AddMargins(GlobalRect);
        Rect otherRect= _rect;
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
		// smallest distance to remove the overlap.
        if((totalWidth-xDistance) < (totalHeight-yDistance)) {
            if(myRect.xMin < otherRect.xMin) {
                return new Vector2(totalWidth-xDistance, 0);
            }
            else {
                return new Vector2(xDistance-totalWidth, 0);
            }
        }
        else {
            if(myRect.yMin < otherRect.yMin) {
                return new Vector2(0, totalHeight-yDistance);
            }
            else {
                return new Vector2(0, yDistance-totalHeight);                
            }            
        }
	}

}
