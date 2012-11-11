using UnityEngine;
using System.Collections;

/*
    TODO: Cleanup Get/SetDisplayPosition.
*/
public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the global position of the given object.
    public Rect GetLayoutPosition(iCS_EditorObject eObj) {
        return eObj.GlobalRect;
    }
    public Rect GetLayoutPosition(int id) {
        return EditorObjects[id].GlobalRect;
    }
    public Vector2 GetLayoutCenterPosition(iCS_EditorObject eObj) {
        return Math3D.Middle(GetLayoutPosition(eObj));
    }

    // ----------------------------------------------------------------------
    // Returns the local position of the given object.
    public Rect GetLayoutLocalPosition(iCS_EditorObject eObj) {
        return eObj.LocalRect;
    }
    // ----------------------------------------------------------------------
    public void SetLayoutPosition(iCS_EditorObject node, Rect _newPos) {
        // Reposition node.
        var globalRect= node.GlobalRect;
        node.GlobalRect= _newPos;
        if(node.IsParentValid) {
            // Adjust node size.
            Rect deltaMove= new Rect(_newPos.x-globalRect.x, _newPos.y-globalRect.y, _newPos.width-globalRect.width, _newPos.height-globalRect.height);
            float separationX= Math3D.IsNotZero(deltaMove.x) ? deltaMove.x : deltaMove.width;
            float separationY= Math3D.IsNotZero(deltaMove.y) ? deltaMove.y : deltaMove.height;
            var separationVector= new Vector2(separationX, separationY);
            LayoutParent(node, separationVector);
        }
    }    
   	// ----------------------------------------------------------------------
    public Rect GetVisiblePosition(iCS_EditorObject edObj) {
		// Return the layout position if the object is visible.
		if(edObj.IsVisible) return GetLayoutPosition(edObj);
		// Return the center of the most visible parent if not visible.
		var parent= edObj.Parent;
		for(; parent != null && !parent.IsVisible; parent= parent.Parent);
		if(parent != null) {
            Vector2 midPoint= Math3D.Middle(GetLayoutPosition(parent));
            return new Rect(midPoint.x, midPoint.y, 0, 0);							
		}
		return GetLayoutPosition(edObj);
    }
}
