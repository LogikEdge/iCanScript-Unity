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
			myAnimatedGlobalDisplayPosition.Start(GlobalDisplayPosition,
	                                              value,
	                                              timeRatio,
	                                              (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
			GlobalDisplayPosition= value;
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
			myAnimatedDisplaySize.Start(GlobalDisplayPosition,
	                                    value,
	                                    timeRatio,
	                                    (start,end,ratio)=>Math3D.Lerp(start,end,ratio));
			DisplaySize= value;
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
 		    var sze= new Vector2(value.width, value.height);
 		    var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
 		    AnimatedGlobalDisplayPosition= pos;
 		    AnimatedDisplaySize= sze;
 		}
 	}
 			
                c=> childRect= Math3D.Merge(childRect, c.AnimatedGlobalDisplayRect)
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
	public void SetGlobalAnchorAndDisplayPosition(Vector2 pos) {
		GlobalAnchorPosition= pos;
		GlobalDisplayPosition= pos;
	}
}
