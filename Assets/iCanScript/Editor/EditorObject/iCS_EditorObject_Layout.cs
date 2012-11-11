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
			EngineObject.DisplaySize= value;
            // Update port position when parent node changes dimensions.
			if(IsNode) {
			    myIStorage.UpdatePortPositions(this);
			}
		}
	}
    // ----------------------------------------------------------------------
    public Vector2 LocalPosition {
		get {
			if(!IsVisible) {
				var size= Parent.DisplaySize;
				return new Vector2(0.5f*size.x, 0.5f*size.y);
			}
			return EngineObject.LocalPosition;
		}
		set {
			EngineObject.LocalPosition= value;
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
	public Rect GlobalRect {
		get {
			if(!IsParentValid) return LocalRect;
			var parentRect= Parent.GlobalRect;
			var localRect= LocalRect;
			return new Rect(parentRect.x+localRect.x, parentRect.y+localRect.y,
							localRect.width, localRect.height);
		}
		set {
			if(!IsParentValid) {
			    LocalRect= value;
			    return;
		    }
			// First adjust parent display area.
			var parent= Parent;
			var parentRect= parent.GlobalRect;
			// The parent is now adjusted so we can now set our display area.
			LocalRect= new Rect(value.x-parentRect.x, value.y-parentRect.y, value.width, value.height);
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

