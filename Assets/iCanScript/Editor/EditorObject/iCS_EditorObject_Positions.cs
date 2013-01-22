using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 		myDisplaySize     = Vector2.zero;
	private P.Animate<Rect>	myAnimatedRect= new P.Animate<Rect>();
	
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
	public Rect GlobalAnimatedRect {
		get {
			if(myAnimatedRect.IsActive && !myAnimatedRect.IsElapsed) {
				return myAnimatedRect.CurrentValue;
			}
			Vector2 size= DisplaySize;
			Vector2 pos= GlobalDisplayPosition;
			var rect =new Rect(pos.x-0.5f*size.x, pos.y-0.5f*size.y, size.x, size.y);
			myAnimatedRect.Reset(rect);
			return rect;
		}
	}
}
