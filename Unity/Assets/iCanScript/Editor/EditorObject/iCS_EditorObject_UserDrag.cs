using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
	// ======================================================================
	// Node Drag
    // ----------------------------------------------------------------------
	public void StartNodeDrag() {
        IsFloating= false;
		IsSticky= true;
		SetAsHighestLayoutPriority();
		ForEachParentNode(
			p=> {
				p.IsSticky= true;
			}
		);
	}
    // ----------------------------------------------------------------------
	public void EndNodeDrag() {
		myIStorage.ForcedRelayoutOfTree(myIStorage.DisplayRoot);
        IsFloating= false;
        IsSticky= false;
		ForEachParentNode(
			p=> {
				p.IsSticky= false;
			}
		);
	}
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeDragTo(Vector2 newPosition) {
		if(IsNode) {
            IStorage.StopAllAnimations();
            UserDragPosition= newPosition;
			LayoutParentNodesUntilTop();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }

	// ======================================================================
	// Node Relocate
    // ----------------------------------------------------------------------
	public void StartNodeRelocate() {
		IsFloating= true;
		IsSticky= true;
		SetAsHighestLayoutPriority();
	}
    // ----------------------------------------------------------------------
	public void EndNodeRelocate() {
		IsFloating= false;
		IsSticky= false;
	}
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeRelocateTo(Vector2 newPosition) {
		if(IsNode) {
            IStorage.StopAllAnimations();
            UserDragPosition= newPosition;
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
}
