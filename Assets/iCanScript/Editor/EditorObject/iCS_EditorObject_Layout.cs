using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // Display animation attributes =========================================
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}

    // Accessors ============================================================
    public Vector2 DisplaySize {
		get {
			if(!IsVisible) return Vector2.zero;
			return EngineObject.DisplaySize;
		}
		set {
            // Avoid propagating change if we did not change size
            var engineObject= EngineObject;
            var previousSize= engineObject.DisplaySize;
            if(Math3D.IsEqual(previousSize, value)) return;
            // Set new size and update any size dependent values.
			engineObject.DisplaySize= value;
			IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------
    public Vector2 LocalPosition {
		get {
			return IsVisible ? EngineObject.LocalPosition : Vector2.zero;
		}
		set {
            // Avoid propagating change if we did not change position
            var engineObject= EngineObject;
            var previousPos= engineObject.LocalPosition;
            if(Math3D.IsEqual(previousPos, value)) return;
            // Set new local position and update any position dependent values.
			EngineObject.LocalPosition= value;
            IsDirty= true;
		}
	}

    // High-Order Accessors =================================================
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
    // ----------------------------------------------------------------------
    public Vector2 GlobalPosition {
        get {
            if(!IsParentValid) return LocalPosition;
            return Parent.GlobalPosition+LocalPosition;
        }
        set {
            if(!IsParentValid) {
                LocalPosition= value;
                return;
            }
            LocalPosition= value-Parent.GlobalPosition;
        }
    }
    // ----------------------------------------------------------------------
	public Rect GlobalRect {
		get {
            var displaySize= DisplaySize;
			var globalPosition= GlobalPosition;
		    float x= globalPosition.x-0.5f*displaySize.x;
		    float y= globalPosition.y-0.5f*displaySize.y;
		    return new Rect(x, y, displaySize.x, displaySize.y);
		}
		set {
		    float x= value.x+0.5f*value.width;
		    float y= value.y+0.5f*value.height;
		    GlobalPosition= new Vector2(x, y);
		    DisplaySize= new Vector2(value.width, value.height);
		}
	}

    // Accessor Modifiers ===================================================
    public Vector2 DisplaySizeWithMargin {
        get {
			if(!IsVisible) return DisplaySize;
            return AddMargins(DisplaySize);
        }
    }
    // ----------------------------------------------------------------------
    public Rect LocalRectWithMargin {
        get {
			if(!IsVisible) return LocalRect;
            return AddMargins(LocalRect);
        }
    }
    // ----------------------------------------------------------------------
    public Rect GlobalRectWithMargin {
        get {
			if(!IsVisible) return GlobalRect;
            return AddMargins(GlobalRect);
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

