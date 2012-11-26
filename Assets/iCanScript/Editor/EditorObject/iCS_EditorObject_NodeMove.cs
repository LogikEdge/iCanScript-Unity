using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    public void NodeUserDrag(Vector2 newPosition) {
        NodeUserDragDelta(newPosition-GlobalPosition);
    }
    // ----------------------------------------------------------------------
    public void NodeUserDragDelta(Vector2 delta) {
        LocalPosition+= delta;
        NodeAdjustAfterDrag(delta);
    }
    // ----------------------------------------------------------------------
    void NodeAdjustAfterDrag(Vector2 delta) {
        // Nothing else to do if this is the root object.
        if(!IsParentValid) return;
        var parent= Parent;
        // Resolve collision with siblings.
        myIStorage.ResolveCollisionOnChildren(parent, delta);
		// Take a snapshot of the children position.
		var childPositions= new List<Vector2>();
		ForEachChildNode(c=> childPositions.Add(c.GlobalPosition));
        // Readjust parent size & position.
        var previousGlobalRect= parent.GlobalRect;
        var childrenGlobalRect= parent.NeededChildrenGlobalRect;
        parent.NodeGlobalChildRect= childrenGlobalRect;
        var newGlobalRect= parent.GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) return;
		// Reposition child to maintain their global positions.
		int i= 0;
		ForEachChildNode(c=> c.GlobalPosition= childPositions[i++]);
		// Ask parent to do the same...
		delta= Math3D.Middle(newGlobalRect)-Math3D.Middle(previousGlobalRect);
        parent.NodeAdjustAfterDrag(delta);
    }
}
