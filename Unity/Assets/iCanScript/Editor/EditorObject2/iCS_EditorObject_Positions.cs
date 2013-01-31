using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		   myLayoutSize     			  = Vector2.zero;
	private P.Animate<Vector2> myAnimatedGlobalDisplayPosition= new P.Animate<Vector2>();
	private P.Animate<Vector2> myAnimatedDisplaySize          = new P.Animate<Vector2>();
	
    // ======================================================================
    // Engine Object Proxy Position Accessors
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPosition {
		get {
			// Port local anchor position getter.
            if(IsPort) return GetPortLocalAnchorPositionFromRatio();
            // Node local anchor position getter.
            return EngineObject.LocalAnchorPosition;
		}
		set {
			// Port local anchor position setter.
            if(IsPort) {
    			UpdatePortEdge();
    			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);
                return;
            }
            // Node local anchor position setter.
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
	public Vector2 GlobalDisplayPosition {
		get {
			if(myAnimatedGlobalDisplayPosition.IsActive && !myAnimatedGlobalDisplayPosition.IsElapsed) {
				return myAnimatedGlobalDisplayPosition.CurrentValue;
			}
			Vector2 pos= GlobalLayoutPosition;
			myAnimatedGlobalDisplayPosition.Reset(pos);
			return pos;
		}
		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateGlobalDisplayPosition(timeRatio, value);
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalDisplayPosition {
		get {
			var globalPos= GlobalDisplayPosition;
			var parent= Parent;
			if(parent == null) return globalPos;
			return globalPos-parent.GlobalDisplayPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 DisplaySize {
		get {
		    if(IsPort) {
		        if(!IsVisibleOnDisplay) return Vector2.zero;
		        return iCS_EditorConfig.PortSize;
		    }
			if(myAnimatedDisplaySize.IsActive && !myAnimatedDisplaySize.IsElapsed) {
				return myAnimatedDisplaySize.CurrentValue;
			}
			Vector2 sze= LayoutSize;
			myAnimatedDisplaySize.Reset(sze);
			return sze;
		}
		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateDisplaySize(timeRatio, value);
		}
	}
    // ----------------------------------------------------------------------
 	public Rect GlobalDisplayRect {
 		get {
             var pos= GlobalDisplayPosition;
             var sze= DisplaySize;
             var rect= new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
             return rect;
 		}
 		set {
			var timeRatio= new P.TimeRatio();
	        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
			AnimateGlobalDisplayRect(timeRatio, value);
 		}
 	}
 	// ----------------------------------------------------------------------
    public Rect GlobalDisplayChildRect {
        get {
            var pos= GlobalDisplayPosition;
            Rect childRect= new Rect(pos.x, pos.y, 0, 0);
            ForEachChildNode(
                c=> childRect= Math3D.Merge(childRect, c.GlobalDisplayRect)
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
	public void AnimateGlobalDisplayPosition(P.TimeRatio timeRatio, Vector2 finalPosition) {
		myAnimatedGlobalDisplayPosition.Start(GlobalLayoutPosition,
                                              finalPosition,
                                              timeRatio,
                                              (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		GlobalLayoutPosition= finalPosition;	
	}
    // ----------------------------------------------------------------------
	public void AnimateDisplaySize(P.TimeRatio timeRatio, Vector2 finalSize) {
		myAnimatedDisplaySize.Start(LayoutSize,
                                    finalSize,
                                    timeRatio,
                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		LayoutSize= finalSize;		
	}
    // ----------------------------------------------------------------------
	public void AnimateGlobalDisplayRect(P.TimeRatio timeRatio, Rect finalRect) {
	    var sze= new Vector2(finalRect.width, finalRect.height);
	    var pos= new Vector2(finalRect.x+0.5f*sze.x, finalRect.y+0.5f*sze.y);
		AnimateGlobalDisplayPosition(timeRatio, pos);
		AnimateDisplaySize(timeRatio, sze);
	}
}
