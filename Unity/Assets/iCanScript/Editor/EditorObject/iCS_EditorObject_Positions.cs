using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		   myLocalLayoutOffset      = Vector2.zero;
	private Vector2 		   myLayoutSize     		= Vector2.zero;
	private P.Animate<Vector2> myAnimatedDisplayPosition= new P.Animate<Vector2>();
	private P.Animate<Vector2> myAnimatedDisplaySize    = new P.Animate<Vector2>();
	
    // ======================================================================
    // Ratio
    // ----------------------------------------------------------------------
    public float PortPositionRatio {
        get { return EngineObject.PortPositionRatio; }
		set {
            var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.PortPositionRatio, value)) return;
			engineObject.PortPositionRatio= value;
			IsDirty= true;
		}
    }
    // ======================================================================
    // Anchor
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPosition {
		get {
			return IsPort ? LocalAnchorPositionPort : LocalAnchorPositionNode;
		}
		set {
			if(IsPort) {
				LocalAnchorPositionPort= value;
			} else {
				LocalAnchorPositionNode= value;
			}
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPositionPort {
		get { return GetPortLocalAnchorPositionFromRatio(); }
		set {
            // Don't update layout offset for port on iconized nodes.
			var parentNode= ParentNode;
			if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) return;
            // Transform to a position ratio between 0f and 1f.
			UpdatePortEdge(value);
			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPositionNode {
		get { return EngineObject.LocalAnchorPosition; }
		set {
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) return;
			engineObject.LocalAnchorPosition= value;
			IsDirty= true;
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

    // ======================================================================
    // Layout
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
            myLayoutSize= value;
		}
	}
    // ----------------------------------------------------------------------
    // Offset from the anchor position.
	public Vector2 LocalLayoutOffset {
		get {
            if(!IsVisibleInLayout) {
                return Vector2.zero;
            }
			return myLocalLayoutOffset;
		}
		set {
            // Don't update layout offset for port on iconized nodes.
            if(IsPort) {
    			var parentNode= ParentNode;
    			if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) return;                
            }
			myLocalLayoutOffset= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalLayoutPosition {
		get {
            if(!IsVisibleInLayout) {
                return Vector2.zero;
            }
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
	public Vector2 GlobalLayoutPosition {
		get {
            if(!IsVisibleInLayout) return ParentNode.GlobalLayoutPosition;
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

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
	public Vector2 GlobalDisplayPosition {
		get {
			if(myAnimatedDisplayPosition.IsActive && !myAnimatedDisplayPosition.IsElapsed) {
				return myAnimatedDisplayPosition.CurrentValue;
			}
			Vector2 pos= GlobalLayoutPosition;
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
	void SetStartValueForDisplayRectAnimation(Rect r) {
		SetStartValueForDisplaySizeAnimation(new Vector2(r.width, r.height));
		SetStartValueForDisplayPositionAnimation(Math3D.Middle(r));
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplaySizeAnimation(Vector2 startSize) {
		if(!IsDisplaySizeAnimated) {
			myAnimatedDisplaySize.Reset(startSize);
		}
	}
    // ----------------------------------------------------------------------
	void SetStartValueForDisplayPositionAnimation(Vector2 startPos) {
		if(!IsDisplayPositionAnimated) {
			myAnimatedDisplayPosition.Reset(startPos);
		}
	}
    // ----------------------------------------------------------------------
	static P.TimeRatio BuildStandardAnimationTimer() {
		var timeRatio= new P.TimeRatio();
        timeRatio.Start(iCS_PreferencesEditor.AnimationTime);
		return timeRatio;
	}
    // ----------------------------------------------------------------------
	void StartDisplayRectAnimation() {
		var timeRatio= BuildStandardAnimationTimer();
		StartDisplayRectAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplaySizeAnimation() {
		var timeRatio= BuildStandardAnimationTimer(); 
		StartDisplaySizeAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplayPositionAnimation() {
		var timeRatio= BuildStandardAnimationTimer();
		StartDisplayPositionAnimation(timeRatio);
	}
    // ----------------------------------------------------------------------
	void StartDisplayRectAnimation(P.TimeRatio timeRatio) {
		StartDisplaySizeAnimation(timeRatio);
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
