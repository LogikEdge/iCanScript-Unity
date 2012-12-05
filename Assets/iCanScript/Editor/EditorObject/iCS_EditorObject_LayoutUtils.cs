using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  LAYOUT UTILITIES
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
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
    void WrapAroundChildrenNodes() { 
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
	Vector2[] BuildListOfRatios(iCS_EditorObject[] children, Rect globalRect) {
		var len= children.Length;
		var result= new Vector2[len];
		if(len == 0) return result;
		var globalPosition= Math3D.Middle(globalRect);
		var size= 0.5f*new Vector2(globalRect.width, globalRect.height);
		for(int i= 0; i < len; ++i) {
			var localPos= children[i].GlobalPosition-globalPosition;
			result[i]= new Vector2(localPos.x/size.x, localPos.y/size.y);
		}
		return result;
	}

}