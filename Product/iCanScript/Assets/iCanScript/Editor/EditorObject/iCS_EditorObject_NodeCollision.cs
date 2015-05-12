// FIXME: Need to understand why new collision priority is not working.
//#define NEW_COLLISION
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;

    public partial class iCS_EditorObject {    
        // ======================================================================
        // Collision Functions
        // ----------------------------------------------------------------------
        // Resolves the collision between children.  "true" is returned if a
        // collision has occured.
        // ----------------------------------------------------------------------
        public void ResolveCollisionOnChildrenNodes() {
    		// Get a snapshot of the children state.
    		var children= BuildListOfVisibleChildNodes(c=> !c.IsFloating);
            var childPos= P.map(n => n.LocalAnchorPosition+n.WrappingOffset, children);
    		var childRect= P.map(n => BuildRect(n.LocalAnchorPosition+n.WrappingOffset, n.LocalSize), children);
            // Resolve collisions.
            ResolveCollisionOnChildrenImp(children, ref childRect);
            // Update child position.
    		for(int i= 0; i < P.length(children); ++i) {
                children[i].CollisionOffset= PositionFrom(childRect[i])-childPos[i];
    		}
        }
        // ----------------------------------------------------------------------
        private void ResolveCollisionOnChildrenImp(iCS_EditorObject[] children, ref Rect[] childRect) {
            // Resolve collisions.
    		int r= 0;
            bool didCollide= true;
    		while(didCollide) {
    			didCollide= false;
    			iCS_EditorObject lowest= null;
    	        for(int i= 0; i < children.Length-1; ++i) {
    				var c1= children[i];
    	            for(int j= i+1; j < P.length(children); ++j) {
    					var c2= children[j];
    	                if(c1.ResolveCollisionBetweenTwoNodes(c2, ref childRect[i],
    															  ref childRect[j])) {
    					    didCollide= true;
    #if NEW_COLLISION
                            if(c1.LayoutPriority < c2.LayoutPriority) {
                                c2.LayoutPriority= c1.LayoutPriority+1;
                            }
                            if(c2.LayoutPriority < c1.LayoutPriority) {
                                c1.LayoutPriority= c2.LayoutPriority+1;
                            }
    #else
                            --c1.LayoutPriority;
                            --c2.LayoutPriority;	
    #endif
                        }
    					if(c1.LayoutPriority > c2.LayoutPriority) {
    						lowest= c1;
    					} else if(c2.LayoutPriority > c1.LayoutPriority) {
    						lowest= c2;
    					}
    	            }
    	        }
    			if(++r > 10) {
    				if(lowest == null || lowest.LayoutPriority <= 1) {
    					break;
    				}
    				lowest.LayoutPriority= lowest.LayoutPriority-1;
    				r= 0;
    			}
    		}
        }
        // ----------------------------------------------------------------------
        // Resolves collision between two nodes. "true" is returned if a collision
        // has occured.
        public bool ResolveCollisionBetweenTwoNodes(iCS_EditorObject theOtherNode,
    												ref Rect myRect, ref Rect theOtherRect) {
            // Nothing to do if they don't collide.
            if(!DoesCollideWithMargins(myRect, theOtherRect)) {
    			return false;
    		}
        
            // Compute penetration.
            Vector2 penetration;
            penetration= GetSeperationVector(theOtherNode, myRect, theOtherRect);
            if(Mathf.Abs(penetration.x) < 1.0f && Mathf.Abs(penetration.y) < 1.0f) {
    			return false;
    		}
    		// Use Layout priority to determine which node to move.
    		if(LayoutPriority == theOtherNode.LayoutPriority) {
                penetration*= 0.5f;
    			theOtherRect.x+= penetration.x;
    			theOtherRect.y+= penetration.y;
    			myRect.x-= penetration.x;
    			myRect.y-= penetration.y;
                return true;            			
    		}
    		if(LayoutPriority < theOtherNode.LayoutPriority) {
    			theOtherRect.x+= penetration.x;
    			theOtherRect.y+= penetration.y;
                return true;			
    		}
    		if(LayoutPriority > theOtherNode.LayoutPriority) {
    			myRect.x-= penetration.x;
    			myRect.y-= penetration.y;
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
            var intersection= Math3D.Intersection(myRect, otherRect);
            if(Math3D.IsSmallerOrEqual(intersection.width,0f) || Math3D.IsSmallerOrEqual(intersection.height, 0f)) {
                return Vector2.zero;
            }
    		// A collision is detected.  The seperation vector is the
    		// smallest distance to remove the overlap.  The separtion
    		// must also respect the anchor position relationship
    		// between the two overalpping nodes.
            var theOtherPivot= theOther.LocalAnchorPosition+theOther.WrappingOffset;
            var myPivot= LocalAnchorPosition+WrappingOffset;
    		var pivotSepDir= theOtherPivot-myPivot;
            if(Mathf.Abs(pivotSepDir.x) > Mathf.Abs(pivotSepDir.y)) {
                return new Vector2(intersection.width*Mathf.Sign(pivotSepDir.x), 0f);            
            }
            else {
                return new Vector2(0f, intersection.height*Mathf.Sign(pivotSepDir.y));            
            }
    	}

    	// ======================================================================
    	// Layout priority functionality.
        // ----------------------------------------------------------------------
        // Clear the layout priority on all children
        public void ClearLayoutPriority() {
    #if NEW_COLLISION
            // TODO: Remove all references to ClearLayoutPriority()
    #else
    		var parent= ParentNode;
    		if(parent != null) {
                parent.ForEachChildNode(n=> { n.LayoutPriority= n.IsSticky ? 0 : 100; });
                parent.ClearLayoutPriority();
            }
            LayoutPriority= IsSticky ? 0 : 100;
    #endif
        }
        // ----------------------------------------------------------------------
    	// Sets the current object as the highest layout priority.
    	public void SetAsHighestLayoutPriority() {
            // Set all sibling node priority to 10
    		var parent= ParentNode;
    		if(parent != null) {
                parent.ForEachChildNode(n=> { n.LayoutPriority= n.IsSticky ? 0 : n.LayoutPriority+100; });
                parent.SetAsHighestLayoutPriority();
            }        
    		LayoutPriority= 0;
    	}
    }
    
}

