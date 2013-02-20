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
	private Vector2            myPreviousDisplaySize    = Vector2.zero;
	
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
            if(IsPort) {
                return GetPortLocalAnchorPositionFromRatio();    
            }
            return EngineObject.LocalAnchorPosition;
		}
		set {
			if(IsPort) {
                // Don't update layout offset for port on iconized nodes.
    			var parentNode= ParentNode;
    			if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) return;
                // Transform to a position ratio between 0f and 1f.
    			UpdatePortEdge(value);
    			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);
			} else {
    			var engineObject= EngineObject;
    			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) return;
    			engineObject.LocalAnchorPosition= value;
    			IsDirty= true;
			}
		}
	}
    // ----------------------------------------------------------------------	
	public Vector2 GlobalAnchorPosition {
		get {
			var parent= ParentNode;
			if(parent == null) return LocalAnchorPosition;
			if(IsPort) {
                // Ports are always relative to the display position.
			    return parent.GlobalDisplayPosition+LocalAnchorPosition;
			}
    		return parent.GlobalLayoutPosition+LocalAnchorPosition;			    
		}
		set {
			var parent= Parent;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
            if(IsPort) {
                // Ports are always relative to the display position.
                LocalAnchorPosition= value-parent.GlobalDisplayPosition;
            } else {
    			LocalAnchorPosition= value-parent.GlobalLayoutPosition;                
            }
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
            if(IsPort) {
                if(!IsVisibleOnDisplay) {
                    return Vector2.zero;
                }
            } else {
                if(!IsVisibleInLayout) {
                    return Vector2.zero;
                }
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
            if(IsPort) {
                if(!IsVisibleOnDisplay) {
                    return Vector2.zero;
                }                
            } else {
                if(!IsVisibleInLayout) {
                    return Vector2.zero;
                }                
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
            if(IsPort) {
                if(!IsVisibleOnDisplay) {
                    return ParentNode.GlobalLayoutPosition;
                }                
            } else {
                if(!IsVisibleInLayout) {
                    return ParentNode.GlobalLayoutPosition;
                }
            }
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
			var parent= ParentNode;
			if(parent == null) return GlobalLayoutPosition;
			return parent.GlobalDisplayPosition+LocalLayoutPosition;
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
		    Vector2 displaySize;
			if(myAnimatedDisplaySize.IsActive && !myAnimatedDisplaySize.IsElapsed) {
				displaySize= myAnimatedDisplaySize.CurrentValue;
			} else {
    			displaySize= LayoutSize;			    
			}
            // Update ports to match parent node display size.
            if(IsNode && Math3D.IsNotEqual(displaySize, myPreviousDisplaySize)) {
                myPreviousDisplaySize= displaySize;
                LayoutPorts();
            }
			return displaySize;
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

}
