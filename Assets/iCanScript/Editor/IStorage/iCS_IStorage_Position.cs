using UnityEngine;
using System.Collections;

/*
    TODO: Cleanup Get/SetDisplayPosition.
*/
public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the global position of the given object.
    public Rect GetLayoutPosition(int id) {
        return EditorObjects[id].GlobalRect;
    }
    public Vector2 GetLayoutCenterPosition(iCS_EditorObject eObj) {
        return eObj.GlobalPosition;
    }

    // ----------------------------------------------------------------------
    // Returns the local position of the given object.
    public Rect GetLayoutLocalPosition(iCS_EditorObject eObj) {
        return eObj.LocalRect;
    }
//    // ----------------------------------------------------------------------
//    public void SetLayoutPosition(iCS_EditorObject node, Rect newPos) {
//        // Reposition node.
//        var globalRect= node.GlobalRect;
//        if(Math3D.IsEqual(globalRect, newPos)) return;
//        node.GlobalRect= newPos;
//        if(node.IsParentValid) {
//            // Adjust node size.
//            Rect deltaMove= new Rect(newPos.x-globalRect.x, newPos.y-globalRect.y, newPos.width-globalRect.width, newPos.height-globalRect.height);
//            float separationX= Math3D.IsNotZero(deltaMove.x) ? deltaMove.x : deltaMove.width;
//            float separationY= Math3D.IsNotZero(deltaMove.y) ? deltaMove.y : deltaMove.height;
//            var separationVector= new Vector2(separationX, separationY);
//            LayoutParent(node, separationVector);
//        }
//    }    
   	// ----------------------------------------------------------------------
    public Rect GetVisiblePosition(iCS_EditorObject edObj) {
		// Return the layout position if the object is visible.
		// Return the center of the most visible parent if not visible.
        return edObj.GlobalRect;
    }
}
