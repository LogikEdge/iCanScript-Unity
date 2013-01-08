using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  LAYOUT UTILITIES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    /*
        TODO: Is "InitialGlobalPosition" valid for nodes ?
    */
    // Initializes the global position and saves the ratio =================
	public Vector2 InitialGlobalPosition {
		set {
			GlobalPosition= value;
			SavePosition();
			AnimatedPosition.Reset(GlobalRect);
		}
	}

    // Adds a margin around given rectangle ---------------------------------
    static Rect AddMargins(Rect r) {
        var m= iCS_Config.MarginSize;
        var m2= 2f*m;
        return new Rect(r.x-m, r.y-m, r.width+m2, r.height+m2);
    }
    // ----------------------------------------------------------------------
	// Returns true if a collision exists in the given node array.
	static bool DoesCollide(iCS_EditorObject[] nodes) {
		var len= nodes.Length;
		Rect[] rs= new Rect[len];
		for(int i= 0; i < len; ++ i) {
			rs[i]= nodes[i].GlobalRect;
		}
		for(int i= 0; i < len-1; ++i) {
			var r1= AddMargins(rs[i]);
			for(int j= i+1; j < len; ++j) {
				if(Math3D.DoesCollide(r1, rs[j])) {
					return true;
				}
			}
		}
		return false;
	} 
	
    // ----------------------------------------------------------------------
    // Updates the global Rect arround the children nodes.  It is assume that
    // the children have previously been layed out.  The parent node will not
    // be affect or marked for relayout if the node Rect has been modified.
    public void WrapAroundChildrenNodes() { 
		// Take a snapshot of the children position.
		var childPositions= new List<Vector2>();
		ForEachChildNode(c=> childPositions.Add(c.GlobalPosition));
        // Readjust parent size & position.
        NodeGlobalChildRect= ChildrenGlobalRectFromGlobalRect;
		// Reposition child to maintain their global positions.
		int i= 0;
		ForEachChildNode(c=> c.GlobalPosition= childPositions[i++]);
    }

    // ----------------------------------------------------------------------
    static Vector2 ComputeNodeRatio(Rect r, Vector2 p) {
        if(Math3D.IsZero(r.width) || Math3D.IsZero(r.height)) {
            Debug.LogWarning("iCanScript: Invalid area in which to compute node ratio");
            return new Vector2(0.5f, 0.5f);
        }
        var x= (p.x-r.x)/r.width;
        var y= (p.y-r.y)/r.height;
        return new Vector2(x,y);
    }
    // ----------------------------------------------------------------------
    static Vector2 ComputeNodePositionFromRatio(Rect rect, Vector2 ratio) {
        var x= rect.x+ratio.x*rect.width;
        var y= rect.y+ratio.y*rect.height;
        return new Vector2(x,y);
    }
}