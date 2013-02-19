using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void UserDragTo(Vector2 newPosition) {
		ourDragObject= this;
		ourDragObjectParent= ParentNode;
		if(IsNode) {
			ourDragObjectDelta= newPosition-GlobalDisplayPosition;
            SetGlobalAnchorAndLayoutPosition(newPosition);
            LayoutParentNodesUntilTop();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
		ourDragObject      = null;
		ourDragObjectParent= null;
    }

}
