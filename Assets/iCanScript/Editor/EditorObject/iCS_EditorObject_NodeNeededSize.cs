using UnityEngine;
using System;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE NEEDED SIZE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the size of the node.  We assume that all children have been
    // previously layed out prior to updating the node size.
    public void UpdateNodeSizeFromGlobalRect() {
        DisplaySize= ComputeNodeSizeFromGlobalRect();
    }
    // ----------------------------------------------------------------------
    // Returns the size of the node using the current children global position.
	Vector2 ComputeNodeSizeFromGlobalRect() {
	    return ComputeNodeSize(o=> o.ChildrenSizeFromGlobalRect);
	}
    // ----------------------------------------------------------------------
    // Returns the size of the node using the current children position ratio.
	Vector2 ComputeNodeSizeFromRatio() {
	    return ComputeNodeSize(o=> o.ChildrenSizeFromRatio);
	}
    // ----------------------------------------------------------------------
    // Returns the size of the node using the current children layout.
	Vector2 ComputeNodeSize(Func<iCS_EditorObject, Vector2> childrenSizeFunc) {
		if(!IsVisible) return Vector2.zero;
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
        var childrenSize= childrenSizeFunc(this);
        width = Mathf.Max(minWidth+childrenSize.x, portsTitleWidth);
        height= Mathf.Max(minHeight+childrenSize.y, neededPortsHeight);

var childrenSizeFromRatio= ChildrenSizeFromRatio;
if(Math3D.IsNotEqual(childrenSize.x, childrenSizeFromRatio.x) || Math3D.IsNotEqual(childrenSize.y, childrenSizeFromRatio.y)) {
    Debug.Log("ChildrenSizeRect= "+childrenSize+" ChildrenSizeFromRatio= "+childrenSizeFromRatio);
    Debug.Log("Not same children size");
}

		return new Vector2(width, height);
	}
    // ----------------------------------------------------------------------
    // Returns the minimium height needed for the left / right ports.
    public float MinimumHeightForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            return nbOfPorts*iCS_Config.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimum width needed for the top / bottom ports.
    public float MinimumWidthForPorts {
        get {
            int nbOfPorts= Mathf.Max(NbOfTopPorts, NbOfBottomPorts);
            return nbOfPorts*iCS_Config.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the global rectangle currently used by the children.
    public Rect ChildrenGlobalRectFromGlobalRect {
        get {
            var center= GlobalPosition;
            Rect childRect= new Rect(center.x,center.y,0,0);
            if(!IsUnfolded) return childRect;
            // The size is initialized with the largest & tallest child.
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
                        childRect= Math3D.Merge(childRect, c.GlobalRect);                        
                    }
                }
            );
            return childRect;
        }
    }
    // ----------------------------------------------------------------------
    // Computes the children area size using the children global rect.  Note
    // that child collision resolution is not performed by this function.
    public Vector2 ChildrenSizeFromGlobalRect {
        get {
            Rect childRect= ChildrenGlobalRectFromGlobalRect;
            return new Vector2(childRect.width, childRect.height);
        }
    }
    // ----------------------------------------------------------------------
    // Computes the children area size using the children size and position
    // ratio.  Note that child collision resolution is not performed by this
    // function.
    public Vector2 ChildrenSizeFromRatio {
        get {
            Vector2 childrenSize= Vector2.zero;
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
                        var ratio= c.NodePositionRatio;
                        var rx= 0.5f-Mathf.Abs(0.5f-ratio.x);
                        var ry= 0.5f-Mathf.Abs(0.5f-ratio.y);
                        if(Math3D.IsZero(rx) || Math3D.IsZero(ry)) {
                            Debug.LogWarning("iCanScript: Invalid node position ratio !!!");
                        } else {
                            var childHalfSize= 0.5f*c.DisplaySize; 
                            var sx= childHalfSize.x/rx;
                            var sy= childHalfSize.y/ry;
                            if(sx > childrenSize.x) childrenSize.x= sx;
                            if(sy > childrenSize.y) childrenSize.y= sy;
                        }
                    }
                }
            );
            return childrenSize;
        }
    }
}
