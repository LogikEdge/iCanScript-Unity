using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  PORT LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Port layout attributes ===============================================
	public bool IsOnTopEdge         { get { return Edge == iCS_EdgeEnum.Top; }}
    public bool IsOnBottomEdge      { get { return Edge == iCS_EdgeEnum.Bottom; }}
    public bool IsOnRightEdge       { get { return Edge == iCS_EdgeEnum.Right; }}
    public bool IsOnLeftEdge        { get { return Edge == iCS_EdgeEnum.Left; }}
    public bool IsOnHorizontalEdge  { get { return IsOnTopEdge   || IsOnBottomEdge; }}
    public bool IsOnVerticalEdge    { get { return IsOnRightEdge || IsOnLeftEdge; }}
    

    // Port Layout Utilities ================================================
    public void CleanupPortEdgePosition() {
        var size= Parent.DisplaySize;
        var lp= LocalPosition;
        switch(Edge) {
            case iCS_EdgeEnum.Top:      lp.y= -0.5f*size.y; break; 
            case iCS_EdgeEnum.Bottom:   lp.y=  0.5f*size.y; break;
            case iCS_EdgeEnum.Left:     lp.x= -0.5f*size.x; break;
            case iCS_EdgeEnum.Right:    lp.x=  0.5f*size.x; break;
        }
		LocalPosition= lp;
    }
    // ----------------------------------------------------------------------
    public bool IsPortOnParentEdge {
        get {
            var edge= IsStatePort ? ClosestEdge : Edge;
			return IsPortOnNodeEdge(Parent, edge);
        }
    }
    // ----------------------------------------------------------------------
    public bool IsPortOnNodeEdge(iCS_EditorObject node, iCS_EdgeEnum edge) {
		return IsPortOnRectEdge(node.GlobalRect, edge);
    }
    // ----------------------------------------------------------------------
    public bool IsPortOnRectEdge(Rect r, iCS_EdgeEnum edge) {
		return IsPositionOnRectEdge(GlobalPosition, r, edge);
    }
/*
	FIXME : The following method should be moved to NodeLayout or all the
			edge stuff put together in another file...
*/
    // ----------------------------------------------------------------------
	// Return true if the position is on the edge of the node.
	public bool IsPositionOnEdge(Vector2 position, iCS_EdgeEnum edge) {
		return IsPositionOnRectEdge(position, GlobalRect, edge);
	}
    // ----------------------------------------------------------------------
    public static bool IsPositionOnRectEdge(Vector2 pos, Rect r, iCS_EdgeEnum edge) {
		float maxDistance= iCS_Config.PortSize;
        float distance= 2f*maxDistance;
        float leftX  = r.xMin;
        float rightX = r.xMax;
        float topY   = r.yMin;
        float bottomY= r.yMax;
        switch(edge) {
            case iCS_EdgeEnum.Top:
                distance= Math3D.DistanceFromHorizontalLineSegment(pos, leftX, rightX, topY);
                break; 
            case iCS_EdgeEnum.Bottom:
                distance= Math3D.DistanceFromHorizontalLineSegment(pos, leftX, rightX, bottomY);
                break;
            case iCS_EdgeEnum.Left:
                distance= Math3D.DistanceFromVerticalLineSegment(pos, topY, bottomY, leftX);
                break;
            case iCS_EdgeEnum.Right:
                distance= Math3D.DistanceFromVerticalLineSegment(pos, topY, bottomY, rightX);
                break;                
        }
        return distance <= maxDistance;
    }
    // ----------------------------------------------------------------------
	public iCS_EdgeEnum ClosestEdge {
		get {
			// Don't change edge if parent is iconized.
			var parent= Parent;
			if(parent.IsIconized) return Edge;
            var parentSize= parent.DisplaySize;
            float leftX  = -0.5f*parentSize.x;
            float rightX =  0.5f*parentSize.x;
            float topY   = -0.5f*parentSize.y;
            float bottomY=  0.5f*parentSize.y;
			var edge= iCS_EdgeEnum.Top;
			float distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, topY);
			float d= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, leftX, rightX, bottomY);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Bottom;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, leftX);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Left;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, topY, bottomY, rightX); 
			if(d < distance) {
				edge= iCS_EdgeEnum.Right;
			}
			return edge;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfTopPorts {
		get {
			int cnt= 0;
			ForEachTopChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfBottomPorts {
		get {
			int cnt= 0;
			ForEachBottomChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfLeftPorts {
		get {
			int cnt= 0;
			ForEachLeftChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public int NbOfRightPorts {
		get {
			int cnt= 0;
			ForEachRightChildPort(_=> ++cnt);
			return cnt;
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] TopPorts {
		get {
			return BuildListOfChildPorts(c=> c.IsOnTopEdge && !c.IsFloating);
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] BottomPorts {
		get {
			return BuildListOfChildPorts(c=> c.IsOnBottomEdge && !c.IsFloating);
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] LeftPorts {
		get {
			return BuildListOfChildPorts(c=> c.IsOnLeftEdge && !c.IsFloating);
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] RightPorts {
		get {
			return BuildListOfChildPorts(c=> c.IsOnRightEdge && !c.IsFloating);
		}
	}
	
	// ======================================================================
    // Layout from iCS_Port
    // ----------------------------------------------------------------------
    public void UpdatePortEdge() {
        // Enable ports are always on top of the node.
        if(IsEnablePort) {
            Edge= iCS_EdgeEnum.Top;
        }
        // Data ports are always on the left or right depending on input/output direction.
        else if(IsDataPort) {
            Edge= IsInputPort ? iCS_EdgeEnum.Left : iCS_EdgeEnum.Right;
        }
        // Selected closest edge.
        else {
            Edge= ClosestEdge;            
        }
    }

    // ======================================================================
    // ----------------------------------------------------------------------
    // Returns the Y coordinate for a port on the vertical edge given its
    // ratio.
    public float VerticalPortYCoordinateFromRatio() {
        var parent= Parent;
        var ratio= PortPositionRatio;
        return parent.VerticalPortsTop+parent.AvailableHeightForPorts*ratio;
    }
    // ----------------------------------------------------------------------
    // Returns the X coordinate for a port on the horizontal edge given its
    // ratio.
    public float HorizontalPortXCoordinateFromRatio() {
        var parent= Parent;
        var ratio= PortPositionRatio;
        return parent.HorizontalPortsLeft+parent.AvailableWidthForPorts*ratio;
    }

}
