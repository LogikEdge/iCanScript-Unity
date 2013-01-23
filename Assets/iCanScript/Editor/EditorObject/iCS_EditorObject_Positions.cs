using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		   myDisplaySize     			  = Vector2.zero;
	private P.Animate<Vector2> myAnimatedGlobalDisplayPosition= new P.Animate<Vector2>();
	private P.Animate<Vector2> myAnimatedDisplaySize          = new P.Animate<Vector2>();
	
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
	public Vector2 LocalDisplayOffset {
		get { return EngineObject.LocalDisplayOffset; }
		set {
            var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalDisplayOffset, value)) return;
			engineObject.LocalDisplayOffset= value;
		}
	}

    // ======================================================================
	// High-order accessors
    // ----------------------------------------------------------------------
	public Vector2 LocalDisplayPosition {
		get {
			return LocalAnchorPosition+LocalDisplayOffset;
		}
		set {
			LocalDisplayOffset= value-LocalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------	
	public Vector2 GlobalAnchorPosition {
		get {
			var parent= Parent;
			if(parent == null) return LocalAnchorPosition;
			return parent.GlobalDisplayPosition+LocalAnchorPosition;
		}
		set {
			LocalDisplayOffset= GlobalDisplayPosition-value;
			var parent= Parent;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
			LocalAnchorPosition= value-parent.GlobalDisplayPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 GlobalDisplayPosition {
		get {
			return GlobalAnchorPosition+LocalDisplayOffset;
		}
		set {
			LocalDisplayOffset= value-GlobalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 AnimatedGlobalDisplayPosition {
		get {
			if(myAnimatedGlobalDisplayPosition.IsActive && !myAnimatedGlobalDisplayPosition.IsElapsed) {
				return myAnimatedGlobalDisplayPosition.CurrentValue;
			}
			Vector2 pos= GlobalDisplayPosition;
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
	public Vector2 AnimatedLocalDisplayPosition {
		get {
			var globalPos= AnimatedGlobalDisplayPosition;
			var parent= Parent;
			if(parent == null) return globalPos;
			return globalPos-parent.AnimateGlobalDisplayPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 AnimatedDisplaySize {
		get {
			if(myAnimatedDisplaySize.IsActive && !myAnimatedDisplaySize.IsElapsed) {
				return myAnimatedDisplaySize.CurrentValue;
			}
			Vector2 sze= DisplaySize;
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
 	public Rect AnimatedGlobalDisplayRect {
 		get {
             var pos= AnimatedGlobalDisplayPosition;
             var sze= AnimatedDisplaySize;
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
    public Rect AnimatedGlobalChildRect {
        get {
            var pos= AnimatedGlobalDisplayPosition;
            Rect childRect= new Rect(pos.x, pos.y, 0, 0);
            ForEachChildNode(
                c=> childRect= Math3D.Merge(childRect, c.AnimatedGlobalDisplayRect)
            );
            return childRect;
        }
    }
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
	public void SetGlobalAnchorAndDisplayPosition(Vector2 pos) {
		GlobalAnchorPosition= pos;
		GlobalDisplayPosition= pos;
	}
    // ----------------------------------------------------------------------
	public void AnimateGlobalDisplayPosition(P.TimeRatio timeRatio, Vector2 finalPosition) {
		myAnimatedGlobalDisplayPosition.Start(GlobalDisplayPosition,
                                              finalPosition,
                                              timeRatio,
                                              (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		GlobalDisplayPosition= finalPosition;	
	}
    // ----------------------------------------------------------------------
	public void AnimateDisplaySize(P.TimeRatio timeRatio, Vector2 finalSize) {
		myAnimatedDisplaySize.Start(DisplaySize,
                                    finalSize,
                                    timeRatio,
                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
		DisplaySize= finalSize;		
	}
    // ----------------------------------------------------------------------
	public void AnimateGlobalDisplayRect(P.TimeRatio timeRatio, Rect finalRect) {
	    var sze= new Vector2(finalRect.width, finalRect.height);
	    var pos= new Vector2(finalRect.x+0.5f*sze.x, finalRect.y+0.5f*sze.y);
		AnimateGlobalDisplayPosition(timeRatio, pos);
		AnimateDisplaySize(timeRatio, sze);
	}
}
