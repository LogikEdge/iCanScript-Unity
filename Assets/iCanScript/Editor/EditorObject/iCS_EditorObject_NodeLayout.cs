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
    Rect ChildrenArea {
        get {
            var displaySize= DisplaySize;
            Rect childArea= new Rect(-0.5f*displaySize.x, -0.5f*displaySize.y, displaySize.x, displaySize.y);
            // Remove top & bottom padding.
            var topPadding= NodeTopPadding;
            var bottomPadding= NodeBottomPadding;
            childArea.y+= topPadding;
            childArea.height-= topPadding+bottomPadding;
            // Remove left & right padding.
            var leftPadding= NodeLeftPadding;
            var rightPadding= NodeRightPadding;
            childArea.x+= leftPadding;
            childArea.width-= leftPadding+rightPadding;
            return childArea;
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
    // ----------------------------------------------------------------------
    public void LayoutPorts() {
        var halfSize= 0.5f*DisplaySize;
        var verticalTop    = VerticalPortsTop;
        var verticalBottom = VerticalPortsBottom;
        var horizontalLeft = HorizontalPortsLeft;
        var horizontalRight= HorizontalPortsRight;
        LayoutPortsOnVerticalEdge  (LeftPorts  , verticalTop   , verticalBottom, -halfSize.x);
        LayoutPortsOnVerticalEdge  (RightPorts , verticalTop   , verticalBottom,  halfSize.x);
        LayoutPortsOnHorizontalEdge(TopPorts   , horizontalLeft, horizontalRight, -halfSize.y);
        LayoutPortsOnHorizontalEdge(BottomPorts, horizontalLeft, horizontalRight,  halfSize.y);
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
    // ----------------------------------------------------------------------
    public static void LayoutPortsOnVerticalEdge(iCS_EditorObject[] ports,
                                                 float top, float bottom, float x) {
        // Layout ports on one dimension edge.
        float[] ys= LayoutPortsOnEdge(ports, top, bottom);
		// Update position from new layout.
        int nbPorts= ports.Length;
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalPosition= new Vector2(x, top+ys[i]);
		}
    }
    // ----------------------------------------------------------------------
    public static void LayoutPortsOnHorizontalEdge(iCS_EditorObject[] ports,
                                                 float left, float right, float y) {
        // Layout ports on one dimension edge.
        float[] xs= LayoutPortsOnEdge(ports, left, right);
		// Update position from new layout.
        int nbPorts= ports.Length;
		for(int i= 0; i < nbPorts; ++i) {
			ports[i].LocalPosition= new Vector2(left+xs[i], y);
		}
    }
    // ----------------------------------------------------------------------
    // Layouts out the ports on an one dimension edge.
    public static float[] LayoutPortsOnEdge(iCS_EditorObject[] ports,
                                         float minValue, float maxValue) {
        // Compute position according to ratio.
        int nbPorts= ports.Length;
        float diff= maxValue-minValue;
        float[] xs= GetPortPositionRatios(ref ports);
        for(int i= 0; i < nbPorts; ++i) {
            xs[i]*= diff;
        }
        // Resolve position according to collisions.
        ResolvePortCollisions(xs, diff);
        return xs;
    }

    // ======================================================================
    // Port layout utilities
    // ----------------------------------------------------------------------
    // Returns the sorted array of saved port ratios.  The input port array
    // is also updated to reflect the sort order.
	public static float[] GetPortPositionRatios(ref iCS_EditorObject[] ports) {
		// Extract ratios.
        int nbPorts= ports.Length;
        float[] rs= new float[nbPorts];
        for(int i= 0; i < nbPorts; ++i) {
            rs[i]= ports[i].PortPositionRatio;
        }
		// Sort port according to ratios
		for(int i= 0; i < nbPorts-1; ++i) {
			var v= rs[i];
			for(int j= i+1; j < nbPorts; ++j) {
				if(rs[i] > rs[j]) {
					rs[i]= rs[j]; rs[j]= v; v= rs[i];
					var tmp= ports[i]; ports[i]= ports[j]; ports[j]= tmp;
				}
			}
		}
		return rs;
	}
    // ----------------------------------------------------------------------
    // Resolves the port separartion on a given edge.  The position is local
    // and ranges from 0 to "maxPos".
    public static float[] ResolvePortCollisions(float[] pos, float maxPos) {
        int nbPorts= pos.Length;
        if(nbPorts < 2) return pos;
        float minSeparation= iCS_Config.MinimumPortSeparation;
        // Determine min/max position for each port.
        float[] minPositions= new float[nbPorts];
        float[] maxPositions= new float[nbPorts]; 
        for(int i= 0; i < nbPorts; ++i) {
            minPositions[i]= i*minSeparation;
            maxPositions[i]= maxPos-(nbPorts-1-i)*minSeparation;
        }
        // Iterate resolving collisions
        float[] collisions= new float[nbPorts-1];
        bool resolveCollisionNeeded= true;
        float allowedOverlap= 0.01f;
        for(int r= 0; r < nbPorts && resolveCollisionNeeded; ++r) {
            // Apply hard min/max position constraints.
            for(int i= 0; i < nbPorts; ++i) {
                if(Math3D.IsSmaller(pos[i], minPositions[i])) pos[i]= minPositions[i];
                if(Math3D.IsGreater(pos[i], maxPositions[i])) pos[i]= maxPositions[i];
            }
            // Cummulate collisions penetration.
            resolveCollisionNeeded= false;
            for(int i= 0; i < nbPorts-1; ++i) {
                float overlap= -(pos[i+1]-pos[i]-minSeparation);
                collisions[i]= overlap;
                if(Math3D.IsGreater(overlap, allowedOverlap)) resolveCollisionNeeded= true;
            }
            if(!resolveCollisionNeeded) continue;
            // Resolve collisions.
            for(int i= 0; i < nbPorts-1; ++i) {
                float overlap= collisions[i]; 
                if(Math3D.IsGreater(overlap, 0f)) {
                    pos[i]  -= 0.5f*overlap;
                    pos[i+1]+= 0.5f*overlap;
                }
            }
        }
        if(resolveCollisionNeeded) {
            Debug.LogWarning("iCanScript: Difficulty stabilizing port layout !!!");
        }
        return pos;        
    }    
}

