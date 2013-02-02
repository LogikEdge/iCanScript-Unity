using UnityEngine;
using System.Collections;
using P=Prelude;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  LAYOUT
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // Accessors ============================================================
    public Vector2 LayoutSize {
		get {
			if(!IsVisibleInLayout) return Vector2.zero;
            return myLayoutSize;
		}
		set {
            // Avoid propagating change if we did not change size
            if(Math3D.IsEqual(myLayoutSize, value)) return;
            myLayoutSize= value;
            if(IsNode && !IsIconizedInLayout && IsVisibleInLayout) {
                LayoutPorts();
                if(IsParentValid) {
                    Parent.IsDirty= true;
                }
            }
		}
	}
//    // ----------------------------------------------------------------------
//    public Vector2 LocalPosition {
//		get {
//            if(!IsVisible) return Vector2.zero;
//            return myLocalPosition;
//		}
//		set {
//            if(Math3D.IsEqual(myLocalPosition, value)) return;
//            myLocalPosition= value;
//		}
//	}
//
//    // High-Order Accessors =================================================
//    public Rect LocalRect {
//		get {
//            var displaySize= LayoutSize;
//			var localPosition= LocalPosition;
//		    float x= localPosition.x-0.5f*displaySize.x;
//		    float y= localPosition.y-0.5f*displaySize.y;
//		    return new Rect(x, y, displaySize.x, displaySize.y);
//		}
//		set {
//		    float x= value.x+0.5f*value.width;
//		    float y= value.y+0.5f*value.height;
//		    LocalPosition= new Vector2(x, y);
//		    LayoutSize= new Vector2(value.width, value.height);
//		}
//	}
//    // ----------------------------------------------------------------------
//    public Vector2 GlobalPosition {
//        get {
//            if(!IsParentValid) return LocalPosition;
//            return Parent.GlobalPosition+LocalPosition;
//        }
//        set {
//            if(!IsParentValid) {
//                LocalPosition= value;
//                return;
//            }
//            LocalPosition= value-Parent.GlobalPosition;
//        }
//    }
//    // ----------------------------------------------------------------------
//	public Rect GlobalRect {
//		get {
//            var displaySize= LayoutSize;
//			var globalPosition= GlobalPosition;
//		    float x= globalPosition.x-0.5f*displaySize.x;
//		    float y= globalPosition.y-0.5f*displaySize.y;
//		    return new Rect(x, y, displaySize.x, displaySize.y);
//		}
//		set {
//		    float x= value.x+0.5f*value.width;
//		    float y= value.y+0.5f*value.height;
//		    GlobalPosition= new Vector2(x, y);
//		    LayoutSize= new Vector2(value.width, value.height);
//		}
//	}
//    // Accessor Modifiers ===================================================
//    public Rect LocalRectWithMargin {
//        get {
//			if(!IsVisible) return LocalRect;
//            return AddMargins(LocalRect);
//        }
//    }
//    // ----------------------------------------------------------------------
//    public Rect GlobalRectWithMargin {
//        get {
//			if(!IsVisible) return GlobalRect;
//            return AddMargins(GlobalRect);
//        }
//    }
}

