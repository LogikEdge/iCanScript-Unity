using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		   myLayoutSize     			  = Vector2.zero;
	private P.Animate<Vector2> myAnimatedGlobalLayoutPosition= new P.Animate<Vector2>();
	private P.Animate<Vector2> myAnimatedLayoutSize          = new P.Animate<Vector2>();
	
    // ======================================================================
    // Engine Object Proxy Position Accessors
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPosition {
		get { return EngineObject.LocalAnchorPosition; }
		set {
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) return;
			engineObject.LocalAnchorPosition= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalLayoutOffset {
		get { return EngineObject.LocalLayoutOffset; }
		set {
            var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalLayoutOffset, value)) return;
			engineObject.LocalLayoutOffset= value;
		}
	}

    // ======================================================================
	// High-order accessors
    // ----------------------------------------------------------------------
	public Vector2 LocalLayoutPosition {
		get {
			return LocalAnchorPosition+LocalLayoutOffset;
		}
		set {
			LocalLayoutOffset= value-LocalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
    public Rect LocalLayoutRect {
        get {
            var sze= LayoutSize;
            var pos= LocalLayoutPosition;
            return new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
        }
        set {
            var sze= new Vector2(value.width, value.height);
            var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
            LocalLayoutPosition= pos;
            LayoutSize= sze;
        }
    }
    // ----------------------------------------------------------------------	
    public Rect LocalAnchorRect {
        get {
            var sze= LayoutSize;
            var pos= LocalAnchorPosition;
            return new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
        }
        set {
            var sze= new Vector2(value.width, value.height);
            var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
            LocalAnchorPosition= pos;
            LayoutSize= sze;
        }
    }
    // ----------------------------------------------------------------------	
	public Vector2 GlobalAnchorPosition {
		get {
			var parent= Parent;
			if(parent == null) return LocalAnchorPosition;
			return parent.GlobalLayoutPosition+LocalAnchorPosition;
		}
		set {
			LocalLayoutOffset= GlobalLayoutPosition-value;
			var parent= Parent;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
			LocalAnchorPosition= value-parent.GlobalLayoutPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 GlobalLayoutPosition {
		get {
			return GlobalAnchorPosition+LocalLayoutOffset;
		}
		set {
			LocalLayoutOffset= value-GlobalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
    public Rect GlobalLayoutRect {
        get {
            var pos= GlobalLayoutPosition;
            var sze= LayoutSize;
            return new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
        }
        set {
            var sze= new Vector2(value.width, value.height);
            var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
            LayoutSize= sze;
            GlobalLayoutPosition= pos;
        }
    }
    // ----------------------------------------------------------------------
	public Vector2 AnimatedGlobalLayoutPosition {
		get {
			if(myAnimatedGlobalLayoutPosition.IsActive && !myAnimatedGlobalLayoutPosition.IsElapsed) {
				return myAnimatedGlobalLayoutPosition.CurrentValue;
			}
			Vector2 pos= GlobalLayoutPosition;
			myAnimatedGlobalLayoutPosition.Reset(pos);
			return pos;
		}
		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateGlobalLayoutPosition(timeRatio, value);
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 AnimatedLocalLayoutPosition {
		get {
			var globalPos= AnimatedGlobalLayoutPosition;
			var parent= Parent;
			if(parent == null) return globalPos;
			return globalPos-parent.AnimatedGlobalLayoutPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 AnimatedLayoutSize {
		get {
			if(myAnimatedLayoutSize.IsActive && !myAnimatedLayoutSize.IsElapsed) {
				return myAnimatedLayoutSize.CurrentValue;
			}
			Vector2 sze= LayoutSize;
			myAnimatedLayoutSize.Reset(sze);
			return sze;
		}
		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateLayoutSize(timeRatio, value);
		}
	}
    // ----------------------------------------------------------------------
 	public Rect AnimatedGlobalLayoutRect {
 		get {
             var pos= AnimatedGlobalLayoutPosition;
             var sze= AnimatedLayoutSize;
             var rect= new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
             return rect;
 		}
 		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateGlobalLayoutRect(timeRatio, value);
 		}
 	}
 	// ----------------------------------------------------------------------
    public Rect AnimatedGlobalChildRect {
        get {
            var pos= AnimatedGlobalLayoutPosition;
            Rect childRect= new Rect(pos.x, pos.y, 0, 0);
            ForEachChildNode(
                c=> childRect= Math3D.Merge(childRect, c.AnimatedGlobalLayoutRect)
            );
            return childRect;
        }
    }
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
	public void SetGlobalAnchorAndLayoutPosition(Vector2 pos) {
		GlobalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
	}
    // ----------------------------------------------------------------------
	public void SetLocalAnchorAndLayoutPosition(Vector2 pos) {
		LocalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
	}
    // ----------------------------------------------------------------------
    public void SetGlobalAnchorAndLayoutRect(Rect r) {
        var sze= new Vector2(r.width, r.height);
        var pos= new Vector2(r.x+0.5f*sze.x, r.y+0.5f*sze.y);
        GlobalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
        LayoutSize= sze;
    }
    // ----------------------------------------------------------------------
    public void SetLocalAnchorAndLayoutRect(Rect r) {
        var sze= new Vector2(r.width, r.height);
        var pos= new Vector2(r.x+0.5f*sze.x, r.y+0.5f*sze.y);
        LocalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
        LayoutSize= sze;        
    }
    // ----------------------------------------------------------------------
	public void AnimateGlobalLayoutPosition(P.TimeRatio timeRatio, Vector2 finalPosition) {
		myAnimatedGlobalLayoutPosition.Start(GlobalLayoutPosition,
                                              finalPosition,
                                              timeRatio,
                                              (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		GlobalLayoutPosition= finalPosition;	
	}
    // ----------------------------------------------------------------------
	public void AnimateLayoutSize(P.TimeRatio timeRatio, Vector2 finalSize) {
		myAnimatedLayoutSize.Start(LayoutSize,
                                    finalSize,
                                    timeRatio,
                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		LayoutSize= finalSize;		
	}
    // ----------------------------------------------------------------------
	public void AnimateGlobalLayoutRect(P.TimeRatio timeRatio, Rect finalRect) {
	    var sze= new Vector2(finalRect.width, finalRect.height);
	    var pos= new Vector2(finalRect.x+0.5f*sze.x, finalRect.y+0.5f*sze.y);
		AnimateGlobalLayoutPosition(timeRatio, pos);
		AnimateLayoutSize(timeRatio, sze);
	}
}
