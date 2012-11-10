using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // Node Layout Utilities ================================================
	public Vector2 NodeNeededSize {
		get {
			if(!IsVisible) return Vector2.zero;
			if(IsIconized) return iCS_Graphics.GetMaximizeIconSize(this);
			int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
			float height= NodeTopPadding+NodeBottomPadding+nbOfPorts*iCS_Config.MinimumPortSeparation;
			float width= Mathf.Max(NodeTitleWidth, NodeLeftPadding+NodeRightPadding);
			if(IsFolded) {
				return new Vector2(width, height);
			}
			var center= 0.5f*DisplaySize;
			Rect childRect= new Rect(center.x, center.y, 0, 0);
			ForEachChildNode(o=> { if(!o.IsFloating) Math3D.Merge(childRect, o.LocalRect); });
			return new Vector2(width+childRect.width, height+childRect.height);
		}
	}
    // ----------------------------------------------------------------------
	public Rect NodeNeededAbsoluteChildRect {
		get {
			if(!IsUnfolded) return new Rect(0,0,0,0);
			var center= Math3D.Middle(AbsoluteRect);
			Rect childRect= new Rect(center.x, center.y, 0, 0);
			ForEachChildNode(o=> { if(!o.IsFloating) Math3D.Merge(childRect, o.AbsoluteRect); });
			int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
			float portHeight= nbOfPorts*iCS_Config.MinimumPortSeparation;
			if(portHeight > childRect.height) {
				var deltaY= portHeight-childRect.height;
				childRect.y-= 0.5f*deltaY;
				childRect.height= portHeight;
			}
			var left= NodeLeftPadding;
			childRect.x-= left;
			childRect.width+= left+NodeRightPadding;
			var top= NodeTopPadding;
			childRect.y-= top;
			childRect.height+= top+NodeBottomPadding;
			var titleWidth= NodeTitleWidth;
			if(childRect.width < titleWidth) {
				var deltaX= titleWidth-childRect.width;
				childRect.x-= 0.5f*deltaX;
				childRect.width= titleWidth;
			}
			return childRect;
		}
	}
    // ----------------------------------------------------------------------
	public Rect NodeAbsoluteChildRect {
		get {
			var r= AbsoluteRect;
			var top= NodeTopPadding;
			r.y+= top;
			r.height-= top+NodeBottomPadding;
			var left= NodeLeftPadding;
			r.x+= left;
			r.width-= left+NodeRightPadding;
			return r;
		}
		set {
			var top= NodeTopPadding;
			value.y-= top;
			value.height+= top+NodeBottomPadding;
			var left= NodeLeftPadding;
			value.x-= left;
			value.width+= left+NodeRightPadding;
			AbsoluteRect= value;
		}
	}
    // ----------------------------------------------------------------------
	public float NodeTitleWidth {
		get {
			if(IsIconized) return 0;
			return iCS_Config.GetNodeWidth(Name)+iCS_Config.ExtraIconWidth+2f*iCS_Config.PaddingSize;
		}
	}
    // ----------------------------------------------------------------------
    public float NodeTopPadding {
        get {
            if(IsIconized) return 0;
            return iCS_Config.NodeTitleHeight+iCS_Config.PaddingSize;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeBottomPadding {
        get {
            if(IsIconized) return 0;
            return iCS_Config.PaddingSize;            
        }
    }
    // ----------------------------------------------------------------------
    public float NodeLeftPadding {
        get {
            if(IsIconized) return 0;
            float leftPadding= iCS_Config.PaddingSize;
            ForEachLeftChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                        float nameSize= labelSize.x+iCS_Config.PortSize;
                        if(leftPadding < nameSize) leftPadding= nameSize;
                    }
                }
            );
            return leftPadding;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeRightPadding {
        get {
            if(IsIconized) return 0;
            float rightPadding= iCS_Config.PaddingSize;
            ForEachRightChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                        float nameSize= labelSize.x+iCS_Config.PortSize;
                        if(rightPadding < nameSize) rightPadding= nameSize;
                    }
                }
            );
            return rightPadding;
        }
    }
    // ----------------------------------------------------------------------
    public void AdjustChildPosition(Vector2 _delta) {
        ForEachChildNode(child=> child.LocalPosition= child.LocalPosition+_delta);
    }

}

