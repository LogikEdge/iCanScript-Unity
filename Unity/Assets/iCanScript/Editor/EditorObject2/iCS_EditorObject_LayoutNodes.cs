using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Layout the nodes from the parent of the object moving up the hierarchy
    // until we reach the top.  The sticky bit is carried over from the object
    // to the parent.
    void LayoutParentNodesUntilTop() {
        var parent= ParentNode;
        if(parent == null) return;
        var parentGlobalRect= parent.GlobalLayoutRect;
        parent.LayoutNode();
        if(Math3D.IsNotEqual(parentGlobalRect, parent.GlobalLayoutRect)) {
            parent.IsSticky= IsSticky;
            parent.LayoutParentNodesUntilTop();
            parent.IsSticky= false;
        }
    }
    // ----------------------------------------------------------------------
	public void LayoutNode() {
        // Nothing to do for invisible ports.
        if(!IsVisibleInLayout) return;
        // Resolve any existing collisions on children for unfolded modules.
        if(IsUnfoldedInLayout && !IsFunction) {
            ResolveCollisionOnChildrenNodes();
            WrapAroundChildrenNodes();
    		LayoutPorts();
    		return;            
        }
        // Just update the size of the node if it is iconized.
        if(IsIconizedInLayout) {
            LayoutSize= iCS_Graphics.GetMaximizeIconSize(this);
            return;
        }
        // Update the size and ports for folded & Function nodes.
        GlobalLayoutRect= NodeGlobalLayoutRectFromChildrenGlobalLayoutRectWithMargins(new Rect(0,0,0,0));
        LayoutPorts();
	}

    // ----------------------------------------------------------------------
    // Updates the global Rect arround the children nodes.  It is assume that
    // the children have previously been layed out.  The anchor position and
    // layout size will be updated accordingly.
    // NOTE: This function must not be called for iconized nodes.
    public void WrapAroundChildrenNodes() { 
		// Nothing to do if node is not visible.
		if(!IsVisibleOnDisplay || IsIconizedOnDisplay) {
            Debug.LogWarning("iCanScript: Internal Error: WrapAroundChildrenNodes should not be invoked for invisible or iconized nodes.");
		    return;
	    }
		// Keep a copy of original rect to determine if we should forward
		// this call to our parent.
		var originalGlobalRect= GlobalLayoutRect;
		// Take a snapshot of the children global position.
		var childGlobalAnchorPositions= new List<Vector2>();
		ForEachChildNode(
		    c=> {
		        childGlobalAnchorPositions.Add(c.GlobalAnchorPosition);
	        }
		);
        // Determine node global layout.
        var r= NodeGlobalLayoutRectFromChildrenGlobalLayoutRectWithMargins(GlobalDisplayChildRectWithMargins);
		// Update parent node anchor positions.
		var center= Math3D.Middle(r);
		GlobalAnchorPosition= center-LocalLayoutOffset;
		// Update layout size.
		LayoutSize= new Vector2(r.width, r.height);
		// Reposition child to maintain their global positions.
		int i= 0;
		ForEachChildNode(
		    c=> {
		        c.GlobalAnchorPosition= childGlobalAnchorPositions[i];
				++i;
	        }
		);
    }

    // ----------------------------------------------------------------------
    Rect NodeGlobalLayoutRectFromChildrenGlobalLayoutRectWithMargins(Rect childRect) {
        // Get padding for all sides.
		float topPadding= NodeTopPadding;
		float bottomPadding= NodeBottomPadding;
		float leftPadding= NodeLeftPadding;
		float rightPadding= NodeRightPadding;
        // Determine size of node.
        float width = childRect.width+leftPadding+rightPadding;
        float height= childRect.height+topPadding+bottomPadding;
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
            var pos= GlobalLayoutPosition;
		    x= pos.x-0.5f*width;
		    y= pos.y-0.5f*height;		    
		} else {
		    x= childRect.x-leftPadding-xOffset;
		    y= childRect.y-topPadding-yOffset;
		}
		var r= new Rect(x, y, width,height);
        return r;
    }
    
    
}

