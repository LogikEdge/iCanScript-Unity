using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		   myLayoutSize     			  = Vector2.zero;
	private P.Animate<Vector2> myAnimatedDisplayPosition= new P.Animate<Vector2>();
	private P.Animate<Vector2> myAnimatedDisplaySize          = new P.Animate<Vector2>();
	
    // ======================================================================
    // Engine Object Proxy Position Accessors
    // ----------------------------------------------------------------------
    // The anchor position of a node is valid for an unfolded parent.
    // The anchor position of a port is valid for folded and unfolded parent.
	public Vector2 LocalAnchorPosition {
		get {
			// Port local anchor position getter.
            if(IsPort) {
                return GetPortLocalAnchorPositionFromRatio();
            }
            // Node local anchor position getter.
            return EngineObject.LocalAnchorPosition;
		}
		set {
			// Port local anchor position setter.
            if(IsPort) {
                // Don't update layout offset for port on iconized nodes.
				var parentNode= ParentNode;
				if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) return;
                // Transform to a position ratio between 0f and 1f.
    			UpdatePortEdge(value);
    			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);
                return;
            }
            // Node local anchor position setter.
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) return;
			engineObject.LocalAnchorPosition= value;
			IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------
    // Offset from the anchor position.
/*
	FIXME : should we continue to persist this value?
*/
	public Vector2 LocalLayoutOffset {
		get {
		    return EngineObject.LocalLayoutOffset;
		}
		set {
            // Don't update layout offset for port on iconized nodes.
            if(IsPort) {
    			var parentNode= ParentNode;
    			if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) return;                
            }
            // Update the persistant local layout offset.
			EngineObject.LocalLayoutOffset= value;
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
    public Vector2 LayoutSize {
		get {
			if(!IsVisibleInLayout) {
				return Vector2.zero;
			}
			if(IsNode && IsIconizedInLayout) {
				return iCS_Graphics.GetMaximizeIconSize(this);
			}
            return myLayoutSize;
		}
		set {
            // Avoid propagating change if we did not change size
            if(Math3D.IsEqual(myLayoutSize, value)) return;
            myLayoutSize= value;
            IsDirty= true;
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
			if(myAnimatedDisplayPosition.IsActive && !myAnimatedDisplayPosition.IsElapsed) {
				return myAnimatedDisplayPosition.CurrentValue;
			}
			Vector2 pos= GlobalLayoutPosition;
//			myAnimatedDisplayPosition.Reset(pos);
			return pos;
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
//			myAnimatedDisplaySize.Reset(sze);
			return sze;
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
    // ----------------------------------------------------------------------
    // Returns the global rectangle currently used by the children.
    public Rect GlobalDisplayChildRectWithMargins {
        get {
            var childRect= GlobalDisplayChildRect;
            if(Math3D.IsNotZero(Math3D.Area(childRect))) {
                childRect= AddMargins(childRect);
            }
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

	// ======================================================================
	// Animation
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayRectAnimation() {
		SetStartValueForDisplaySizeAnimation();
		SetStartValueForDisplayPositionAnimation();
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplaySizeAnimation() {
		if(!IsDisplaySizeAnimated) {
			myAnimatedDisplaySize.Reset(LayoutSize);
		}
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayPositionAnimation() {
		if(!IsDisplayPositionAnimated) {
			myAnimatedDisplayPosition.Reset(GlobalLayoutPosition);
		}
	}
    // ----------------------------------------------------------------------
	void StartDisplayRectAnimation() {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
		StartDisplaySizeAnimation(timeRatio);
		StartDisplayPositionAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplaySizeAnimation() {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
		StartDisplaySizeAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplayPositionAnimation() {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
		StartDisplayPositionAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplaySizeAnimation(P.TimeRatio timeRatio) {
		myAnimatedDisplaySize.Start(myAnimatedDisplaySize.CurrentValue,
							        LayoutSize,
							        timeRatio,
                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	}
    // ----------------------------------------------------------------------
	void StartDisplayPositionAnimation(P.TimeRatio timeRatio) {
		myAnimatedDisplayPosition.Start(myAnimatedDisplayPosition.CurrentValue,
										GlobalLayoutPosition,
										timeRatio,
		                                (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
	}
    // ----------------------------------------------------------------------
	public void UpdateAnimation() {
		if(myAnimatedDisplaySize.IsActive) {
			if(myAnimatedDisplaySize.IsElapsed) {
				myAnimatedDisplaySize.Reset(LayoutSize);
			} else {
				myAnimatedDisplaySize.Update();
			}
		}
		if(myAnimatedDisplayPosition.IsActive) {
			if(myAnimatedDisplayPosition.IsElapsed) {
				myAnimatedDisplayPosition.Reset(GlobalLayoutPosition);
			} else {
				myAnimatedDisplayPosition.Update();
			}
		}
	}
}
