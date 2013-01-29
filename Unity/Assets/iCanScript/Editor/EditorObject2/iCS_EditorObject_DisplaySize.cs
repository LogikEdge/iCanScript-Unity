using UnityEngine;
using System;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the node display size.  It is assumed that the child animated
    // display rect has been previously updated.
    public void UpdateNodeLayoutSize() {
        LayoutSize= ComputeNodeLayoutSize(GlobalDisplayChildRectWithMargins);
    }
    // ----------------------------------------------------------------------
    // Returns the size of the node using the current children layout.
	public Vector2 ComputeNodeLayoutSize(Rect childrenGlobalRect) {
		if(!IsVisibleInLayout) return Vector2.zero;
		if(IsIconized) return iCS_Graphics.GetMaximizeIconSize(this);
        float titleHeight= NodeTitleHeight;
        float titleWidth = NodeTitleWidth;
		float minHeight= NodeTopPadding+NodeBottomPadding;
		float minWidth = NodeLeftPadding+NodeRightPadding;
		float neededPortsHeight= titleHeight+MinimumHeightForPorts;
		float neededPortsWidth = MinimumWidthForPorts;
        // The simple case is without any visible child.
        float portsTitleWidth= Mathf.Max(neededPortsWidth, titleWidth);
        float width = Mathf.Max(minWidth, portsTitleWidth);
        float height= Mathf.Max(minHeight, neededPortsHeight);
		if(IsFolded || IsFunction) {
			return new Vector2(width, height);
		}
        // We need to add the children area if any are visible.
        width = Mathf.Max(minWidth+childrenGlobalRect.width, portsTitleWidth);
        height= Mathf.Max(minHeight+childrenGlobalRect.height, neededPortsHeight);
		return new Vector2(width, height);
	}
    
    // ----------------------------------------------------------------------
    // Returns the minimium height needed for the left / right ports.
    public float MinimumHeightForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            return nbOfPorts*iCS_EditorConfig.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimum width needed for the top / bottom ports.
    public float MinimumWidthForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfTopPorts, NbOfBottomPorts);
            return nbOfPorts*iCS_EditorConfig.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the global rectangle currently used by the children.
    public Rect GlobalDisplayChildRectWithMargins {
        get {
            var childRect= GlobalDisplayChildRect;
            if(Math3D.IsNotZero(Math3D.Area(childRect))) {
                childRect= AddMargins(childRect);
            }
            return childRect;
        }
    }
    // ----------------------------------------------------------------------
    public void WrapAroundChildren() {
        var childrenGlobalRect= GlobalDisplayChildRectWithMargins;
        
    }
}
