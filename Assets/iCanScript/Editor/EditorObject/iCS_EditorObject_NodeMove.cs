using UnityEngine;
using System.Collections;

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
        // Readjust parent size & position.
        var previousGlobalRect= parent.GlobalRect;
        var childrenGlobalRect= parent.NeededChildrenGlobalRect;
        parent.NodeGlobalChildRect= childrenGlobalRect;
        var newGlobalRect= parent.GlobalRect;
        if(Math3D.IsEqual(previousGlobalRect, newGlobalRect)) return;
        parent.NodeAdjustAfterDrag(Math3D.Middle(newGlobalRect)-Math3D.Middle(previousGlobalRect));
    }
}
