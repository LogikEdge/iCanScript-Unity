using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the global Rect arround the children nodes.  It is assume that
    // the children have previously been layed out.  The parent node will not
    // be affect or marked for relayout if the node Rect has been modified.
    public void WrapAroundChildrenNodes() { 
		// Nothing to do if node is not visible.
		if(!IsVisibleOnDisplay) return;
		// Keep a copy of original rect to determine if we should forward
		// this call to our parent.
		var originalGlobalRect= GlobalLayoutRect;
		// Take a snapshot of the children global position.
		var childGlobalAnchorPositions= new List<Vector2>();
		var childGlobalLayoutPositions= new List<Vector2>();
		ForEachChildNode(
		    c=> {
		        childGlobalAnchorPositions.Add(c.GlobalAnchorPosition);
                childGlobalLayoutPositions.Add(c.GlobalLayoutPosition);
	        }
		);
		// Get padding for all sides.
		float topPadding= NodeTopPadding;
		float bottomPadding= NodeBottomPadding;
		float leftPadding= NodeLeftPadding;
		float rightPadding= NodeRightPadding;
		// Determine rect to wrap children.
		var childRect= GlobalDisplayChildRectWithMargins;
		var r= new Rect(childRect.x-leftPadding,
						childRect.y-topPadding,
						childRect.width+leftPadding+rightPadding,
						childRect.height+topPadding+bottomPadding);
		// Assure minimum size for title and ports.
		var titleHeight= NodeTitleHeight;
		var titleWidth= NodeTitleWidth;
		var neededPortHeight= MinimumHeightForPorts;
		var neededPortWidth = MinimumWidthForPorts;
		var minHeight= titleHeight+neededPortHeight;
		var minWidth= Mathf.Max(titleWidth, neededPortWidth);
        // Readjust parent size & position.
        if(r.width < minWidth) {
            r.x-= 0.5f*minWidth;
            r.width= minWidth;
        }
        if(r.height < minHeight) {
            r.y-= 0.5f*minHeight;
            r.height= minHeight;
        }
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
		if(Parent != null && Math3D.IsNotEqual(originalGlobalRect, GlobalLayoutRect)) {
			Parent.WrapAroundChildrenNodes();
		}
    }

}
