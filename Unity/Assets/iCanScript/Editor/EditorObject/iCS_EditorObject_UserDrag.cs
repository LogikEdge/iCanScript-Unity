using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
	// ======================================================================
	// Fields;
    // ----------------------------------------------------------------------
	static iCS_EditorObject ourLastDraggedNode= null;
	
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeDragTo(Vector2 newPosition) {
		if(IsNode) {
            StopAnimation();
            AnchorPosition= newPosition;
            LocalOffset= Vector2.zero;
			SetDragNodeLayoutPriority();
			LayoutUnfoldedParentNodesUsingAnimatedChildren();
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
			SetDragNodeLayoutPriority();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
    // ----------------------------------------------------------------------
	void SetDragNodeLayoutPriority() {
		if(ourLastDraggedNode != this) {
			ourLastDraggedNode= this;
			SetAsHighestLayoutPriority();
		}
	} 
}
