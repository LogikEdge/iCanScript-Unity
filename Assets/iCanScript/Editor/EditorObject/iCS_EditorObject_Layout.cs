using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
    // Display animation attributes -----------------------------------------
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}

    // ======================================================================
    public Vector2 DisplaySize {
		get {
			return EngineObject.DisplaySize;
		}
		set {
			EngineObject.DisplaySize= value;
		}
	}
    public Vector2 LocalPosition {
		get {
			return EngineObject.LocalPosition;
		}
		set {
			EngineObject.LocalPosition= value;
		}
	}
    public Rect LocalRect {
		get {
            var displaySize= DisplaySize;
			var localPosition= LocalPosition;
		    float x= localPosition.x-0.5f*displaySize.x;
		    float y= localPosition.y-0.5f*displaySize.y;
		    return new Rect(x, y, displaySize.x, displaySize.y);
		}
		set {
		    float x= value.x+0.5f*value.width;
		    float y= value.y+0.5f*value.height;
		    LocalPosition= new Vector2(x, y);
		    DisplaySize= new Vector2(value.width, value.height);
		}
	}
	public Rect AbsoluteRect {
		get {
			return myIStorage.Storage.GetAbsoluteRect(EngineObject);
		}
		set {
			if(!IsParentValid) {
			    LocalRect= value;
			    return;
		    }
			// First adjust parent display area.
			var parent= Parent;
			var parentRect= parent.AbsoluteRect;
			var childRect= AddMargins(value);
			var newParentRect= Math3D.Union(childRect, parentRect);
			if(Math3D.IsNotEqual(newParentRect, parentRect)) {
				parent.AbsoluteRect= newParentRect;
			}
			// The parent is now adjusted so we can now set our display area.
			LocalRect= new Rect(value.x-newParentRect.x, value.y-newParentRect.y, value.width, value.height);
		}
	}
    // ----------------------------------------------------------------------
    public Vector2 DisplaySizeWithMargin {
        get {
            return AddMargins(DisplaySize);
        }
    }
    public Rect LocalRectWithMargin {
        get {
            return AddMargins(LocalRect);
        }
    }
    public Rect AbsoluteRectWithMargin {
        get {
            return AddMargins(AbsoluteRect);
        }
    }

    // ======================================================================
    // Layout Utilities
    // ======================================================================
    // Adds a margin around given rectangle ---------------------------------
    public static Rect AddMargins(Rect r) {
        var m= iCS_Config.MarginSize;
        var m2= 2f*m;
        return new Rect(r.x-m, r.y-m, r.width+m2, r.height+m2);
    }
    // Adds a margin to the given size --------------------------------------
    public static Vector2 AddMargins(Vector2 size) {
        var m2= 2f*iCS_Config.MarginSize;
        return new Vector2(size.x+m2, size.y+m2);
    }
}
