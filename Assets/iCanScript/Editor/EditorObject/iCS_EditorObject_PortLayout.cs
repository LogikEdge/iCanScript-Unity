using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
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
            if(!IsFloating) return true;
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
            var result= new List<iCS_EditorObject>();
			ForEachTopChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] BottomPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachBottomChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] LeftPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachLeftChildPort(c=> result.Add(c));
			return result.ToArray();
		}
	}
	// ----------------------------------------------------------------------
    public iCS_EditorObject[] RightPorts {
		get {
            var result= new List<iCS_EditorObject>();
			ForEachRightChildPort(c=> result.Add(c));
			return result.ToArray();
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
    
}
