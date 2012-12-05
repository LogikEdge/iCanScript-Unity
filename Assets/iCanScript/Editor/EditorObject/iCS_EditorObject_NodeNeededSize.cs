using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE NEEDED SIZE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the size of the node.  We assume that the size of all children
    // have previously been updated.
    public void UpdateNodeSize() {
        DisplaySize= NodeNeededSize;
    }
    // ----------------------------------------------------------------------
    // Returns the size needed by the node.  We assume that the display size
    // of all children have previously been updated.
	public Vector2 NodeNeededSize {
		get {
			if(!IsVisible) return Vector2.zero;
			if(IsIconized) return iCS_Graphics.GetMaximizeIconSize(this);
            float titleHeight= NodeTitleHeight;
            float titleWidth = NodeTitleWidth;
			float minHeight= NodeTopPadding+NodeBottomPadding;
			float minWidth = NodeLeftPadding+NodeRightPadding;
			float neededPortsHeight= titleHeight+NeededPortsHeight;
			float neededPortsWidth = NeededPortsWidth;
            // The simple case is without any visible child.
            float portsTitleWidth= Mathf.Max(neededPortsWidth, titleWidth);
            float width = Mathf.Max(minWidth, portsTitleWidth);
            float height= Mathf.Max(minHeight, neededPortsHeight);
			if(IsFolded || IsFunction) {
				return new Vector2(width, height);
			}
            // We need to add the children area if any are visible.
            var childrenSize= ChildrenSizeFromGlobalRect;
            width = Mathf.Max(minWidth+childrenSize.x, portsTitleWidth);
            height= Mathf.Max(minHeight+childrenSize.y, neededPortsHeight);
			return new Vector2(width, height);
		}
	}
    // ----------------------------------------------------------------------
    // Returns the minimium height needed for the left / right ports.
    public float NeededPortsHeight {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            return nbOfPorts*iCS_Config.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the minimum width needed for the top / bottom ports.
    public float NeededPortsWidth {
        get {
            int nbOfPorts= Mathf.Max(NbOfTopPorts, NbOfBottomPorts);
            return nbOfPorts*iCS_Config.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    public Rect ChildrenLocalRectFromLocalRect {
        get {
            if(!IsUnfolded) return new Rect(0,0,0,0);
            // The size is initialized with the largest & tallest child.
            Rect childRect= new Rect(0,0,0,0);
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
                        childRect= Math3D.Merge(childRect, c.LocalRect);                        
                    }
                }
            );
            return childRect;
        }
    }
    // ----------------------------------------------------------------------
    public Vector2 ChildrenSizeFromGlobalRect {
        get {
            Rect childRect= ChildrenLocalRectFromLocalRect;
            return new Vector2(childRect.width, childRect.height);
        }
    }
    // ----------------------------------------------------------------------
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
    // Computes the children area using the existing children display size 
    // and their position ratios
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
