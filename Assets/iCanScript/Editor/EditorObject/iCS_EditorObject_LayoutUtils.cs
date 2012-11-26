using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Updates the global Rect arround the children nodes.  It is assume that
    // the children have previously been layed out.  The parent node will not
    // be affect or marked for relayout if the node Rect has been modified.
    void WrapAroundChildrenNodes() { 
		// Take a snapshot of the children position.
		var childPositions= new List<Vector2>();
		ForEachChildNode(c=> childPositions.Add(c.GlobalPosition));
        // Readjust parent size & position.
        NodeGlobalChildRect= NeededChildrenGlobalRect;
		// Reposition child to maintain their global positions.
		int i= 0;
		ForEachChildNode(c=> c.GlobalPosition= childPositions[i++]);
    }

    // Adds a margin around given rectangle ---------------------------------
    public static Rect AddMargins(Rect r) {
        var m= iCS_Config.MarginSize;
        var m2= 2f*m;
        return new Rect(r.x-m, r.y-m, r.width+m2, r.height+m2);
    }

}