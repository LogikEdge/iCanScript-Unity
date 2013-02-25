using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void UserDragTo(Vector2 newPosition) {
		if(IsNode) {
            StopAnimations();
            GlobalAnchorPosition= newPosition;
            LocalLayoutOffset= Vector2.zero;
            LayoutParentNodesUntilTop();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }

}
