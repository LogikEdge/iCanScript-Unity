using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // Port layout attributes -----------------------------------------------
    public bool IsOnLeftEdge      { get { return EngineObject.IsOnLeftEdge; }}
    public bool IsOnRightEdge     { get { return EngineObject.IsOnRightEdge; }}
    public bool IsOnTopEdge       { get { return EngineObject.IsOnTopEdge; }}
    public bool IsOnBottomEdge    { get { return EngineObject.IsOnBottomEdge; }}

    // Port Layout Utilities ================================================
    public void CleanupPortEdgePosition() {
        var size= Parent.DisplaySize;
        var lp= LocalPosition;
        switch(Edge) {
            case iCS_EdgeEnum.Top:      lp.y= 0; break; 
            case iCS_EdgeEnum.Bottom:   lp.y= size.y; break;
            case iCS_EdgeEnum.Left:     lp.x= 0; break;
            case iCS_EdgeEnum.Right:    lp.x= size.x; break;
        }
		LocalPosition= lp;
    }
    // ----------------------------------------------------------------------
    public bool IsPortOnParentEdge {
        get {
    		float maxDistance= 2f*iCS_Config.PortSize;
            float distance= 2f*maxDistance;
            var parentSize= Parent.DisplaySize;
            var edge= IsStatePort ? ClosestEdge : Edge;
            switch(edge) {
                case iCS_EdgeEnum.Top:
                    distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, 0f, parentSize.x, 0f);
                    break; 
                case iCS_EdgeEnum.Bottom:
                    distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, 0f, parentSize.x, parentSize.y);
                    break;
                case iCS_EdgeEnum.Left:
                    distance= Math3D.DistanceFromVerticalLineSegment(LocalPosition, 0f, parentSize.y, 0f);
                    break;
                case iCS_EdgeEnum.Right:
                    distance= Math3D.DistanceFromVerticalLineSegment(LocalPosition, 0f, parentSize.y, parentSize.x);
                    break;                
            }
            return distance <= maxDistance;
        }
    }
    // ----------------------------------------------------------------------
	public iCS_EdgeEnum ClosestEdge {
		get {
			// Don't change edge if parent is iconized.
			var parent= Parent;
			if(parent.IsIconized) return Edge;
            var parentSize= parent.DisplaySize;
			var edge= iCS_EdgeEnum.Top;
			float distance= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, 0f, parentSize.x, 0f);
			float d= Math3D.DistanceFromHorizontalLineSegment(LocalPosition, 0f, parentSize.x, parentSize.y);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Bottom;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, 0f, parentSize.y, 0f);
			if(d < distance) {
				distance= d;
				edge= iCS_EdgeEnum.Left;
			}
			d= Math3D.DistanceFromVerticalLineSegment(LocalPosition, 0f, parentSize.y, parentSize.x); 
			if(d < distance) {
				edge= iCS_EdgeEnum.Right;
			}
			return edge;
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
}
