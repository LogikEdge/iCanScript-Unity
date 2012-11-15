using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // Node Layout Utilities ================================================
	public Vector2 NodeNeededSize {
		get {
			if(!IsVisible) return Vector2.zero;
			if(IsIconized) return iCS_Graphics.GetMaximizeIconSize(this);
			float portsHeight= NeededPortsHeight;
			float height= NodeTopPadding+NodeBottomPadding+portsHeight;
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
	public Rect NodeNeededGlobalChildRect {
		get {
			if(!IsUnfolded) return new Rect(0,0,0,0);
			var center= Math3D.Middle(GlobalRect);
			Rect childRect= new Rect(center.x, center.y, 0, 0);
			ForEachChildNode(o=> { if(!o.IsFloating) Math3D.Merge(childRect, o.GlobalRect); });
			float portsHeight= NeededPortsHeight;
			if(portsHeight > childRect.height) {
				var deltaY= portsHeight-childRect.height;
				childRect.y-= 0.5f*deltaY;
				childRect.height= portsHeight;
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
	public Rect NodeGlobalChildRect {
		get {
			var r= GlobalRect;
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
			GlobalRect= value;
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
    public float NodeTitleHeight {
        get {
            if(IsIconized) return 0;
            return 0.75f*iCS_Config.NodeTitleHeight;
        }
    }
    // ----------------------------------------------------------------------
    public float NodeTopPadding {
        get {
            if(IsIconized) return 0;
            return NodeTitleHeight+iCS_Config.PaddingSize;
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
            float paddingBy2= 0.5f*iCS_Config.PaddingSize;
            float leftPadding= iCS_Config.PaddingSize;
            ForEachLeftChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                        float nameSize= paddingBy2+labelSize.x+iCS_Config.PortSize;
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
            float paddingBy2= 0.5f*iCS_Config.PaddingSize;
            float rightPadding= iCS_Config.PaddingSize;
            ForEachRightChildPort(
                port=> {
                    if(!port.IsStatePort && port.IsPortOnParentEdge) {
                        Vector2 labelSize= iCS_Config.GetPortLabelSize(port.Name);
                        float nameSize= paddingBy2+labelSize.x+iCS_Config.PortSize;
                        if(rightPadding < nameSize) rightPadding= nameSize;
                    }
                }
            );
            return rightPadding;
        }
    }
    // ----------------------------------------------------------------------
    public void AdjustChildLocalPosition(Vector2 _delta) {
        ForEachChildNode(child=> child.LocalPosition= child.LocalPosition+_delta);
    }
    // ----------------------------------------------------------------------
    public float NeededPortsHeight {
        get {
            int nbOfPorts= Mathf.Max(NbOfLeftPorts, NbOfRightPorts);
            if(nbOfPorts == 0) return 0;
            return iCS_Config.PortSize + (nbOfPorts-1)*iCS_Config.MinimumPortSeparation;                                            
        }
    }
    // ----------------------------------------------------------------------
    public int NbOfVerticalPortSlots {
        get {
            float height= DisplaySize.y-NodeTitleHeight-NodeBottomPadding;
            int nbSlots= 0;
            if(height > iCS_Config.PortSize) {
                nbSlots= 1;
                height-= iCS_Config.PortSize;
            }
            nbSlots+= (int)(height/iCS_Config.MinimumPortSeparation);
            return nbSlots;
        }
    }
    // ----------------------------------------------------------------------
    public int NbOfHorizontalPortSlots {
        get {
            float width= DisplaySize.x-2f*iCS_Config.PaddingSize;
            int nbSlots= 0;
            if(width > iCS_Config.PortSize) {
                nbSlots= 1;
                width-= iCS_Config.PortSize;
            }
            nbSlots+= (int)(width/iCS_Config.MinimumPortSeparation);
            return nbSlots;
        }
    }
    // ----------------------------------------------------------------------
    // This is a port property
    public int PortSlotFromPosition {
        get {
            var parentRect= Parent.GlobalRect;
            var pos= GlobalPosition;
            float deltaY= pos.y-parentRect.y-NodeTitleHeight-iCS_Config.PortRadius;
//            if(deltaY <= iCS_Config.PortSize+0.5f*iCS_Config.MinimumPortSeparation) return 0;
//            deltaY-= iCS_Config.PortSize+0.5f*iCS_Config.MinimumPortSeparation;
            return (int)(deltaY/iCS_Config.MinimumPortSeparation);
        }
    }
}

