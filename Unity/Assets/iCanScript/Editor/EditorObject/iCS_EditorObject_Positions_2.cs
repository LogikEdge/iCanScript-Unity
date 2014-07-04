using UnityEngine;
using System.Collections;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_EditorObject {   
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    //                  POSITION MODIFYING OPERATIONS
    // ======================================================================
    // It is assumed that the position is modified in the following order:
    // 1) User drag / relocate
    // 2) Wrapping is performed on agregate nodes
    // 3) Collision is resolved.
    // ----------------------------------------------------------------------
    public Vector2 UserDragLocalPosition {
        get {
            return LocalAnchorPosition;
        }
        set {
            LocalAnchorPosition= value;
        }
    }
    // ----------------------------------------------------------------------
    // @ Called when user moves a node or port
    public Vector2 UserDragPosition {
        get {
            var parent= ParentNode;
            if(parent == null) return UserDragLocalPosition;
            return parent.LayoutPosition+UserDragLocalPosition;
        }
        set {
            WrappingOffset= Vector2.zero;
            CollisionOffset= Vector2.zero;
            var parent= ParentNode;
            if(parent == null) {
                LocalAnchorPosition= value;
            }
            LocalAnchorPosition= value-parent.LayoutPosition+parent.WrappingOffset;
        }
    }
    // ----------------------------------------------------------------------
    // @ Called when an agreagte node is wrapping around its children.
    public Vector2 WrappingPosition {
        get {
            return UserDragPosition+WrappingOffset;
        }
        set {
            CollisionOffset= Vector2.zero;
            WrappingOffset= value-UserDragPosition;
        }
    }
    // ----------------------------------------------------------------------
    // @ Called when a collision is resolved.
    public Vector2 CollisionPosition {
        get {
            return WrappingPosition+CollisionOffset;
        }
        set {
            CollisionOffset= value-WrappingPosition;
        }
    }
}
