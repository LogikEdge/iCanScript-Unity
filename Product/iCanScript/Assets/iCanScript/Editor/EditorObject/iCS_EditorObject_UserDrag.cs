using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
    	// ======================================================================
    	// Node Drag
        // ----------------------------------------------------------------------
    	public void StartNodeDrag() {
            IStorage.StopAllAnimations();
            IsFloating= false;
    		IsSticky= true;
    		SetAsHighestLayoutPriority();
    	}
        // ----------------------------------------------------------------------
    	public void EndNodeDrag() {
    		myIStorage.ForcedRelayoutOfTree();
            myIStorage.ReduceCollisionOffset();
            IsFloating= false;
            IsSticky= false;
            ClearLayoutPriority();
    	}
        // ----------------------------------------------------------------------
        // Forces a new position on the object being dragged by the uesr.
        public void NodeDragTo(Vector2 newPosition) {
    		if(IsNode) {
        		SetAsHighestLayoutPriority();
        		LocalAnchorFromGlobalPosition= newPosition;
    			IStorage.ForcedRelayoutOfTree();
    		} else {
    			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
    		}
        }

    	// ======================================================================
    	// Node Relocate
        // ----------------------------------------------------------------------
    	public void StartNodeRelocate() {
            IStorage.StopAllAnimations();
    		IsFloating= true;
    		IsSticky= true;
    		SetAsHighestLayoutPriority();
    	}
        // ----------------------------------------------------------------------
    	public void EndNodeRelocate() {
    		IsFloating= false;
    		IsSticky= false;
            ClearLayoutPriority();
    	}
        // ----------------------------------------------------------------------
        // Forces a new position on the object being dragged by the uesr.
        public void NodeRelocateTo(Vector2 newPosition) {
    		if(IsNode) {
                GlobalAnchorPosition= newPosition-CollisionOffset;
    		} else {
    			Debug.LogWarning("iCanScript: UserDragTo not implemented for ports.");
    		}
        }
    }
}
