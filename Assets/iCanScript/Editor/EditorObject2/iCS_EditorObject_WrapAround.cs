using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
	/*
		FIXME : Need to manage animated children...
	*/
    // ----------------------------------------------------------------------
    // Updates the global Rect arround the children nodes.  It is assume that
    // the children have previously been layed out.  The parent node will not
    // be affect or marked for relayout if the node Rect has been modified.
    public void WrapAroundChildrenNodes() { 
		// Nothing to do if node is not visible.
		if(!IsVisibleInLayout) return;
		// Take a snapshot of the children global position.
		var childPositions= new List<Vector2>();
		ForEachChildNode(c=> childPositions.Add(c.AnimatedGlobalDisplayPosition));
		// Get padding for all sides.
		float topPadding= NodeTopPadding;
		float bottomPadding= NodeBottomPadding;
		float leftPadding= NodeLeftPadding;
		float rightPadding= NodeRightPadding;
		// Determine rect to wrap children.
		var childRect= AnimatedGlobalChildRectWithMargins;
		var r= new Rect(childRect.x-leftPadding,
						childRect.y-topPadding,
						childRect.width+leftPadding+rightPadding,
						childRect.height+topPadding+bottomPadding);
		var center= Math3D.Middle(r);
		// Assure minimum size for title and ports.
		var titleHeight= NodeTitleHeight;
		var titleWidth= NodeTitleWidth;
		var neededPortHeight= MinimumHeightForPorts;
		var neededPortWidth = MinimumWidthForPorts;
		var minHeight= titleHeight+neededPortHeight;
		var minWidth= Mathf.Max(titleWidth, neededPortWidth);
        // Readjust parent size & position.
		// Reposition child to maintain their global positions.
    }

}
