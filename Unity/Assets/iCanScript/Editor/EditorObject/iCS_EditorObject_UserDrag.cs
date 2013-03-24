using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeDragTo(Vector2 newPosition) {
		if(IsNode) {
            StopAnimation();
            AnchorPosition= newPosition;
            LocalOffset= Vector2.zero;
            LayoutParentNodesUntilTop();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeRelocateTo(Vector2 newPosition) {
		if(IsNode) {
            StopAnimation();
            AnchorPosition= newPosition;
            LocalOffset= Vector2.zero;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }

}
