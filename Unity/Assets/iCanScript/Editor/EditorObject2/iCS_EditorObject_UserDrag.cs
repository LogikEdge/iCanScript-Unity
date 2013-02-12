using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void UserDragTo(Vector2 newPosition) {
		if(IsNode) {
            IsSticky= true;
            SetGlobalAnchorAndLayoutPosition(newPosition);
            LayoutParentNodesUntilTop();
            IsSticky= false;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }

}
