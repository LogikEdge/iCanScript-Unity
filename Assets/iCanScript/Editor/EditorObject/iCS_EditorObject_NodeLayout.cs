using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  NODE LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Storage accessors ====================================================
    public Vector2 NodePositionRatio {
        get { return EngineObject.LocalPositionRatio; }
		set { EngineObject.LocalPositionRatio= value; }
    }

    // Node Layout Utilities ================================================
    // ----------------------------------------------------------------------
	public Rect NodeGlobalChildRect {
		get {
            if(IsIconized || IsFolded) {
                Debug.LogWarning("iCanScript: NodeGlobalChildRect should not be called when the node is iconized or folded");
                var gp= GlobalPosition;
                return new Rect(gp.x, gp.y, 0, 0);
            }
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
    Rect ChildrenLocalRect {
        get {
            var globalPosition= GlobalPosition;
            var childGlobalRect= NodeGlobalChildRect;
            return new Rect(childGlobalRect.x-globalPosition.x, childGlobalRect.y-globalPosition.y, childGlobalRect.width, childGlobalRect.height);
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
	// Initializes port position ratio of all edges.
	public void InitialPortLayout() {
		InitialPortLayout(LeftPorts);
		InitialPortLayout(RightPorts);
		InitialPortLayout(TopPorts);
		InitialPortLayout(BottomPorts);
	}
    // ----------------------------------------------------------------------
	// Initializes port position ratio for ports on the same edge.
	void InitialPortLayout(iCS_EditorObject[] ports) {
		// Sort according to port index.
		int len= ports.Length;
		if(len == 0) return;
		// Sort port according to port index.
		for(int i= 0; i < len-1; ++i) {
			for(int j= i+1; j < len; ++j) {
				if(ports[i].PortIndex > ports[j].PortIndex) {
					var tmp= ports[i];
					ports[i]= ports[j];
					ports[j]= tmp;
				}
			}
		}
		// Update port position ratio.
		float step= 1f/(float)(len);
		for(int i= 0; i < len; ++i) {
			float ratio= ((float)(i)+0.5f)*step;
			ports[i].PortPositionRatio= ratio;
		}
	}
    // ----------------------------------------------------------------------
	// Updates the port position ratio of all edges.
    public void UpdatePortRatios() {
        foreach(var port in LeftPorts) {
            port.SaveVerticalPortRatioFromLocalPosition(port.LocalPosition);
        }
        foreach(var port in RightPorts) {
            port.SaveVerticalPortRatioFromLocalPosition(port.LocalPosition);
        }
        foreach(var port in TopPorts) {
            port.SaveHorizontalPortRatioFromLocalPosition(port.LocalPosition);
        }
        foreach(var port in BottomPorts) {
            port.SaveHorizontalPortRatioFromLocalPosition(port.LocalPosition);
        }
    }

    // ======================================================================
    // Ports layout helpers.
    // ----------------------------------------------------------------------
    // Returns the available height to layout ports on the vertical edge.
    public float AvailableHeightForPorts {
        get {
            return VerticalPortsBottom-VerticalPortsTop;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the available width to layout ports on the horizontal edge.
    public float AvailableWidthForPorts {
        get {
            return HorizontalPortsRight-HorizontalPortsLeft;
        }
    }

    // ----------------------------------------------------------------------
    // Returns the top most coordinate for a port on the vertical edge.
    public float VerticalPortsTop {
        get {
            return NodeTitleHeight+0.5f*(iCS_Config.MinimumPortSeparation-DisplaySize.y);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the bottom most coordinate for a port on the vertical edge.
    public float VerticalPortsBottom {
        get {
            return 0.5f*(DisplaySize.y-iCS_Config.MinimumPortSeparation);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsLeft {
        get {
            return 0.5f*(iCS_Config.MinimumPortSeparation-DisplaySize.x);
        }
    }
    // ----------------------------------------------------------------------
    // Returns the left most coordinate for a port on the horizontal edge.
    public float HorizontalPortsRight {
        get {
            return 0.5f*(DisplaySize.x-iCS_Config.MinimumPortSeparation);
        }
    }

    // ======================================================================
    // Port layout utilities
}

