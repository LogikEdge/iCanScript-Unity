using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the size of the node.  We assume that the size of all children
    // have previously been updated.
    public void UpdateNodeSize() {
        DisplaySize= NodeNeededSize;
        LayoutPorts();
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
            var childrenSize= NeededChildrenSize;
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
    public Vector2 NeededChildrenSize {
        get {
            // The size is initialized with the largest & tallest child.
            Rect childRect= new Rect(0,0,0,0);
            ForEachChildNode(
                c=> {
                    childRect= Math3D.Merge(childRect, c.LocalRect);
                }
            );
            return new Vector2(childRect.width, childRect.height);
        }
    }

}
