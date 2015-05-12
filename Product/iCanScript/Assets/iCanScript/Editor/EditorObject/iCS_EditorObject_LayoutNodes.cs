using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //  NODE LAYOUT
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    public partial class iCS_EditorObject {
        // ----------------------------------------------------------------------
        // Layout the nodes from the parent of the object moving up the hierarchy
        // until we reach the top.  The sticky bit is carried over from the object
        // to the parent.
        /*
            TODO : Verify how the sticky bit is promoted.
        */
        public void LayoutParentNodesUntilTop() {
            var parent= ParentNode;
            if(parent == null) return;
            var parentGlobalRect= parent.GlobalRect;
            parent.LayoutNode();
            if(Math3D.IsNotEqual(parentGlobalRect, parent.GlobalRect)) {
                parent.LayoutParentNodesUntilTop();
            }
        }
        public void LayoutParentNodesUntilDisplayRoot() {
            var parent= ParentNode;
            if(parent == null) return;
            if(parent == IStorage.DisplayRoot) {
                parent.LayoutNode();
                return;
            }
            var parentGlobalRect= parent.GlobalRect;
            parent.LayoutNode();
            if(Math3D.IsNotEqual(parentGlobalRect, parent.GlobalRect)) {
                parent.LayoutParentNodesUntilTop();
            }
        }

        // ----------------------------------------------------------------------
        // Revised: feb 10, 2014
    	public void LayoutNode() {
            // Clear information created in previous layout
            CollisionOffset= Vector2.zero;
            // Nothing to do for invisible ports.
            if(!IsVisibleInLayout) return;
            // Just update the size of the node if it is iconized.
            if(IsIconizedInLayout) {
                LocalSize= iCS_Graphics.GetMaximizeIconSize(this);
                return;
            }
            LayoutPorts();
            // Resolve any existing collisions on children for unfolded modules.
            if(IsUnfoldedInLayout && NbOfChildNodes != 0) {
                ResolveCollisionOnChildrenNodes();
                WrapAroundChildrenNodes();                                
        		return;            
            }
            // Update the size and ports for folded & Function nodes.
            CollisionOffsetFromGlobalRect= FoldedNodeRect();
    	}

        // ----------------------------------------------------------------------
        // Updates the global Rect arround the children nodes.  It is assume that
        // the children have previously been layed out.  The anchor position and
        // layout size will be updated accordingly.
        // NOTE: This function must not be called for iconized nodes.
        // ----------------------------------------------------------------------
        // Revised: feb 10, 2014
        public void WrapAroundChildrenNodes() { 
    		// Nothing to do if node is not visible.
    		if(!IsVisibleInLayout || IsIconizedInLayout) {
    		    return;
    	    }
            if(IsFoldedInLayout) {
                var r= NodeRectFromChildrenRectWithMargins(new Rect(0,0,0,0));
        		LocalSize= new Vector2(r.width, r.height);
                return;
            }
            var childNodes= BuildListOfVisibleChildNodes(n => !n.IsFloating);
            var childRects= P.map(n => n.LocalRect, childNodes);
            // WrapAroundChildRects(childRects);
            var totalChildRect= GetRectWithMargins(childRects);
            var parentRect= NodeRectFromChildrenRectWithMargins(totalChildRect);
            var center= PositionFrom(parentRect);
            WrappingOffset= center;
    		LocalSize = SizeFrom (parentRect);
            // Restore child global position.
            for(int i= 0; i < childNodes.Length; ++i) {
                childNodes[i].CollisionOffset-= center;
            }
        }

        // ----------------------------------------------------------------------
        // Revised: feb 10, 2014
        Vector2 IconizedSize() {
            return iCS_Graphics.GetMaximizeIconSize(this);
        }
        // ----------------------------------------------------------------------
        // Revised: feb 10, 2014
        Rect FoldedNodeRect() {
            return NodeRectFromChildrenRectWithMargins(new Rect(0,0,0,0));
        }

        // ----------------------------------------------------------------------
        // Revised: feb 10, 2014
        Rect NodeRectFromChildrenRectWithMargins(Rect childRect) {
            // Get padding for all sides.
    		float leftPadding= NodeLeftPadding;
    		float rightPadding= NodeRightPadding;
            // Determine size of node.
            // NOTE: Node drawing engine forces to have at least 2 Node Title Height.
            float width = childRect.width+leftPadding+rightPadding;
            float height= childRect.height+2*NodeTitleHeight;
    		// Assure minimum size for title and ports.
    		var titleHeight= NodeTitleHeight;
    		var titleWidth= NodeTitleWidth;
    		var neededPortHeight= MinimumHeightForPorts;
    		var neededPortWidth = MinimumWidthForPorts;
    		var minHeight= titleHeight+neededPortHeight;
    		var minWidth= Mathf.Max(titleWidth, neededPortWidth);
            // Readjust parent size & position.
            float xOffset= 0f;
            float yOffset= 0f;
            if(width < minWidth) {
                xOffset= 0.5f*(minWidth-width);
                width= minWidth;
            }
            if(height < minHeight) {
                yOffset= 0.5f*(minHeight-height);
                height= minHeight;
            }
    		// Determine rect to wrap children.
            float x, y;
    		if(Math3D.IsZero(childRect.width) || Math3D.IsZero(childRect.height)) {
                var pos= GlobalPosition;
    		    x= pos.x-0.5f*width;
    		    y= pos.y-0.5f*height;		    
    		} else {
    		    x= childRect.x-leftPadding-xOffset;
    		    y= childRect.y-NodeTitleHeight-yOffset;
    		}
    		var r= new Rect(x, y, width, height);
            return r;
        }
    
        // ----------------------------------------------------------------------
        public void WrapAroundChildRect(Rect childRect) {
            childRect= NodeRectFromChildrenRectWithMargins(childRect);
            SetNodeLayoutRect(childRect); 
        }
        // ----------------------------------------------------------------------
        public void WrapAroundChildRects(Rect[] childRects) {
            Rect r= GetRectWithMargins(childRects);
            WrapAroundChildRect(r);
        }
        // ----------------------------------------------------------------------
        public void SetNodeLayoutRect(Rect r) {
    		// Update layout size.
    		LocalSize= new Vector2(r.width, r.height);        
        }
        // ----------------------------------------------------------------------
        public void ReduceChildrenAnchorPosition() {
            var childNodes= BuildListOfChildNodes(_ => true);
            if(childNodes.Length == 0) return;
            // Reduce Local anchor position
            var max= P.fold(
                (acc,n)=> {
                    var lap= n.LocalAnchorPosition;
                    return new Vector2(Mathf.Max(acc.x, lap.x), Mathf.Max(acc.y, lap.y));
                },
                childNodes[0].LocalAnchorPosition, childNodes
            );
            var min= P.fold(
                (acc,n)=> {
                    var lap= n.LocalAnchorPosition;
                    return new Vector2(Mathf.Min(acc.x, lap.x), Mathf.Min(acc.y, lap.y));
                },
                childNodes[0].LocalAnchorPosition, childNodes
            );
            var delta= (max-min)/2;
            var offset= max-delta;
            P.forEach(n=> n.LocalAnchorPosition= n.LocalAnchorPosition-offset, childNodes);        
        }
    
        // ======================================================================
        // Utilities
        // ----------------------------------------------------------------------
        static Rect GetRectWithMargins(Rect[] rs) {
            // Determine child area
            Rect r= Math3D.Union(rs);
            // Add margins
            if(Math3D.IsNotZero(Math3D.Area(r))) {
                r= AddMargins(r);
            }
            return r;
        }
    
    }

    
}

