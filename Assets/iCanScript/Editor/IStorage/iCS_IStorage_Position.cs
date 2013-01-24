using UnityEngine;
using System.Collections;

/*
    TODO: Cleanup Get/SetDisplayPosition.
*/
public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    // Returns the global position of the given object.
    public Rect GetLayoutPosition(int id) {
        return EditorObjects[id].GlobalDisplayRect;
    }
    public Vector2 GetLayoutCenterPosition(iCS_EditorObject eObj) {
        return eObj.GlobalDisplayPosition;
    }

    // ----------------------------------------------------------------------
    // Returns the local position of the given object.
    public Rect GetLayoutLocalPosition(iCS_EditorObject eObj) {
        return eObj.LocalDisplayRect;
    }
   	// ----------------------------------------------------------------------
    public Rect GetVisiblePosition(iCS_EditorObject edObj) {
		// Return the layout position if the object is visible.
		// Return the center of the most visible parent if not visible.
        return edObj.GlobalDisplayRect;
    }
}
