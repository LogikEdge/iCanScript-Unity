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
	        NodeUserDragDelta(newPosition-GlobalPosition);
	        IsDirty= true;			
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
    // ----------------------------------------------------------------------
    // Forced adjustment of position of the object dragged by the user. 
    void NodeUserDragDelta(Vector2 delta) {
        LocalPosition+= delta;
        NodeAdjustAfterDrag(delta);
    }
    // ----------------------------------------------------------------------
    // Adjust position of the siblings and the parent after an object drag.
    void NodeAdjustAfterDrag(Vector2 delta) {
        // Nothing else to do if this is the root object.
        if(!IsParentValid) return;
        var parent= Parent;
        // Resolve collision with siblings.
        parent.ResolveCollisionOnChildren(delta);
        // Adjust parent to wrap children.
        var previousGlobalRect= parent.GlobalRect;
        parent.WrapAroundChildrenNodes();
		// Ask parent to do the same if parent Rect has changed.
        var newGlobalRect= parent.GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) return;
		delta= Math3D.Middle(newGlobalRect)-Math3D.Middle(previousGlobalRect);
        parent.NodeAdjustAfterDrag(delta);
    }

    // ======================================================================
    // Collision Functions
    // ----------------------------------------------------------------------
    // Resolves the collision between children.  "true" is returned if a
    // collision has occured.
    public void ResolveCollisionOnChildren(Vector2 delta) {
        bool didCollide= false;
        iCS_EditorObject[] children= BuildListOfChildNodes(c=> !c.IsFloating);
        // Collect collision vectors.
        var len= children.Length;
        for(int i= 0; i < len; ++i) {
            
        }
        // Resolve collisions.
        for(int i= 0; i < children.Length-1; ++i) {
            for(int j= i+1; j < children.Length; ++j) {
                didCollide |= children[i].ResolveCollisionBetweenTwoNodes(children[j], delta);                            
            }
        }
        if(didCollide) ResolveCollisionOnChildren(delta);
    }
    // ----------------------------------------------------------------------
    // Resolves collision between two nodes. "true" is returned if a collision
    // has occured.
    public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject otherNode, Vector2 delta) {
        // Nothing to do if they don't collide.
        if(!DoesCollideWithMargins(otherNode)) return false;

        // Compute penetration.
        Vector2 penetration= GetSeperationVector(otherNode.GlobalRect);
		if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) return false;

		// Seperate using the known movement.
        if( !Math3D.IsZero(delta) ) {
    		if(Vector2.Dot(delta, penetration) > 0) {
                otherNode.LocalPosition+= penetration;
    		}
    		else {
    		    LocalPosition-= penetration;
    		}            
    		return true;
        }

		// Seperate nodes by the penetration that is not a result of movement.
        penetration*= 0.5f;
        otherNode.LocalPosition+= penetration;
        LocalPosition-= penetration;
        return true;
    }
    // ----------------------------------------------------------------------
    // Returns if the given rectangle collides with the node.
    public bool DoesCollideWithMargins(iCS_EditorObject otherNode) {
        return Math3D.DoesCollide(AddMargins(GlobalRect), otherNode.GlobalRect);
    }
    // ----------------------------------------------------------------------
	// Returns the seperation vector of two colliding nodes.
	Vector2 GetSeperationVector(Rect _rect) {
        Rect myRect= AddMargins(GlobalRect);
        Rect otherRect= _rect;
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
