using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
	// ======================================================================
	// Node Drag
    // ----------------------------------------------------------------------
    static Vector2 ourNodeDragWrappingOffset= Vector2.zero;
	public void StartNodeDrag() {
        if(IsUnfoldedInLayout) {
            ourNodeDragWrappingOffset= WrappingOffset;
        }
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
        if(IsUnfoldedInLayout) {
            WrapAroundChildrenNodes();
            LocalAnchorPosition= LocalAnchorPosition-ourNodeDragWrappingOffset;
        }
		myIStorage.ForcedRelayoutOfTree(myIStorage.DisplayRoot);
        IsFloating= false;
        IsSticky= false;
		ForEachParentNode(
			p=> {
				p.IsSticky= false;
			}
		);
        ClearLayoutPriority();
	}
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeDragTo(Vector2 newPosition) {
		if(IsNode) {
            IStorage.StopAllAnimations();
            UserDragPosition= newPosition;
    		SetAsHighestLayoutPriority();
			LayoutParentNodesUntilDisplayRoot();
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }

	// ======================================================================
	// Node Relocate
    // ----------------------------------------------------------------------
	public void StartNodeRelocate() {
        if(IsUnfoldedInLayout) {
            ourNodeDragWrappingOffset= WrappingOffset;
            ReduceChildrenAnchorPosition();            
        }
		IsFloating= true;
		IsSticky= true;
		SetAsHighestLayoutPriority();
	}
    // ----------------------------------------------------------------------
	public void EndNodeRelocate() {
        if(IsUnfoldedInLayout) {
            WrapAroundChildrenNodes();
            LocalAnchorPosition= LocalAnchorPosition-ourNodeDragWrappingOffset;
        }
		IsFloating= false;
		IsSticky= false;
        ClearLayoutPriority();
	}
    // ----------------------------------------------------------------------
    // Forces a new position on the object being dragged by the uesr.
    public void NodeRelocateTo(Vector2 newPosition) {
		if(IsNode) {
            IStorage.StopAllAnimations();
            UserDragPosition= newPosition;
            var parent= ParentNode;
            if(parent != null) {
                CollisionOffset-= parent.WrappingOffset;
            }
		} else {
			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
		}
    }
}
