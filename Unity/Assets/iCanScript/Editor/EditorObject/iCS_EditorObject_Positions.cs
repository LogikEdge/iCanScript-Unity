using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 myLocalLayoutOffset      = Vector2.zero;
	private Vector2 myPreviousDisplaySize    = Vector2.zero;
	private Vector2 myPreviousDisplayPosition= Vector2.zero;
	
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	//								PORT POSITIONS
    // ======================================================================
    // Port Ratio
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
	// Port Anchor
    // ----------------------------------------------------------------------
	public Vector2 PortLocalAnchorPosition {
		get {
            return GetPortLocalAnchorPositionFromRatio();    			
		}
		set {
            // Don't update layout offset for port on iconized nodes.
			var parentNode= ParentNode;
			if(parentNode.IsIconizedOnDisplay || !parentNode.IsVisibleOnDisplay) {
				return;
			}
            // Transform to a position ratio between 0f and 1f.
			UpdatePortEdge(value);
			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);			
			IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------	
    // Ports are always relative to the display position.
	public Vector2 PortGlobalAnchorPosition {
		get {
		    return ParentNode.GlobalDisplayPosition+PortLocalAnchorPosition;			
		}
		set {
            PortLocalAnchorPosition= value-ParentNode.GlobalDisplayPosition;			
		}
	}
	
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	//							NODE POSITIONS
    // ======================================================================
    // Anchor
    // ----------------------------------------------------------------------
	public Vector2 LocalAnchorPosition {
		get {
            if(IsPort) {
                return PortLocalAnchorPosition;    
            }
            return EngineObject.LocalAnchorPosition;
		}
		set {
			if(IsPort) {
				PortLocalAnchorPosition= value;
				return;
			}
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) return;
			engineObject.LocalAnchorPosition= value;
			IsDirty= true;
		}
	}
    // ----------------------------------------------------------------------	
	public Vector2 GlobalAnchorPosition {
		get {
			if(IsPort) {
				return PortGlobalAnchorPosition;
			}
			var parent= ParentNode;
			if(parent == null) return LocalAnchorPosition;
    		return parent.GlobalLayoutPosition+LocalAnchorPosition;			    
		}
		set {
			if(IsPort) {
				PortGlobalAnchorPosition= value;
				return;
			}
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
            var sze= DisplaySize;
            var pos= LocalLayoutPosition;
            return new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
        }
        set {
            var sze= new Vector2(value.width, value.height);
            var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
            LocalLayoutPosition= pos;
            DisplaySize= sze;
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
            var sze= DisplaySize;
            return new Rect(pos.x-0.5f*sze.x, pos.y-0.5f*sze.y, sze.x, sze.y);
        }
        set {
            var sze= new Vector2(value.width, value.height);
            var pos= new Vector2(value.x+0.5f*sze.x, value.y+0.5f*sze.y);
            DisplaySize= sze;
            GlobalLayoutPosition= pos;
        }
    }

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
	public Vector2 GlobalDisplayPosition {
		get {
			var parent= ParentNode;
            Vector2 newPos;
			if(AnimatedPosition.IsActive && !AnimatedPosition.IsElapsed) {
				newPos= AnimatedPosition.CurrentValue;
			} else {
    			if(parent == null) {
    			    return GlobalLayoutPosition;
			    }
        		newPos= parent.GlobalDisplayPosition+LocalLayoutPosition;			    			        
			}
			if(IsNode && Math3D.IsNotEqual(newPos, myPreviousDisplayPosition)) {
			    myPreviousDisplayPosition= newPos;
			    parent.LayoutNode();
			}
			return newPos;
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
			var displaySize= AnimatedSize.CurrentValue;
            // Update ports to match parent node display size.
            if(IsNode && Math3D.IsNotEqual(displaySize, myPreviousDisplaySize)) {
                myPreviousDisplaySize= displaySize;
                LayoutPorts();
                var parent= ParentNode;
                if(parent != null) {
                    parent.LayoutNode();
                }
            }
			return displaySize;
		}
		set {
		    if(IsPort) return;
            var previousSize= AnimatedSize.CurrentValue;
		    AnimatedSize.Reset(value);
		    if(Math3D.IsNotEqual(previousSize, value)) {
		        LayoutPorts();
		    }
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
        DisplaySize= sze;
    }
    // ----------------------------------------------------------------------
    public void SetLocalAnchorAndLayoutRect(Rect r) {
        var sze= new Vector2(r.width, r.height);
        var pos= new Vector2(r.x+0.5f*sze.x, r.y+0.5f*sze.y);
        LocalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
        DisplaySize= sze;        
    }

	// ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    Rect BuildRect(Vector2 pos, Vector2 size) {
        return new Rect(pos.x-0.5f*size.x, pos.y-0.5f*size.y, size.x, size.y);
    }
    // ----------------------------------------------------------------------
    Vector2 PositionFrom(Rect r) {
        var sze= SizeFrom(r);
        return new Vector2(r.x+0.5f*sze.x, r.y+0.5f*sze.y);
    }
    // ----------------------------------------------------------------------
    Vector2 SizeFrom(Rect r) {
        return new Vector2(r.width, r.height);
    }
}
