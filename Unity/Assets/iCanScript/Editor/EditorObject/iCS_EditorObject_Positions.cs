using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
	// Fields
    // ----------------------------------------------------------------------
	private Vector2 myPreviousDisplaySize    = Vector2.zero;
	
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	//								PORT POSITIONS
    // ======================================================================
    // Port Ratio
    // ----------------------------------------------------------------------
    public float PortPositionRatio {
        get { return EngineObject.PortPositionRatio; }
		set {
            var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.PortPositionRatio, value)) {
			    return;
		    }
			engineObject.PortPositionRatio= value;
			IsDirty= true;  // Save new port ratio.
		}
    }
	
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	//							NODE POSITIONS
    // ======================================================================
    // Anchor
    // ----------------------------------------------------------------------
    // The anchor position is never animated.
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
    			if(!IsVisibleOnDisplay) {
                    Debug.LogWarning("iCanScript: Should not set port anchor position if not visible.");
    				return;
    			}
                // Transform to a position ratio between 0f and 1f.
    			UpdatePortEdge(value);
    			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);			
    			IsDirty= true;  // Save new anchor position.
				return;
			}
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) {
			    return;
		    }
			engineObject.LocalAnchorPosition= value;
			IsDirty= true;  // Save new anchor position.
		}
	}
    // ----------------------------------------------------------------------	
	public Vector2 GlobalAnchorPosition {
		get {
			var parent= ParentNode;
			if(parent == null) return LocalAnchorPosition;
    		return parent.GlobalDisplayPosition+LocalAnchorPosition;			    
		}
		set {
			var parent= ParentNode;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
    		LocalAnchorPosition= value-parent.GlobalDisplayPosition;                
		}
	}

    // ======================================================================
    // Layout
    // ----------------------------------------------------------------------
    // Offset from the anchor position.  This attribute is animated.
	public Vector2 LocalLayoutOffset {
		get {
			return AnimatedLayoutOffset.CurrentValue;
		}
		set {
            // Don't update layout offset for port on iconized nodes.
            if(IsPort) {
    			var parent= ParentNode;
    			if(parent.IsIconizedOnDisplay || !parent.IsVisibleOnDisplay) {
    			    return;
			    }
            }
			AnimatedLayoutOffset.Reset(value);
		}
	}

    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
	public Vector2 GlobalDisplayPosition {
		get {
			var parent= ParentNode;
			if(parent == null) {
			    return LocalAnchorPosition+LocalLayoutOffset;
		    }
    		return parent.GlobalDisplayPosition+LocalAnchorPosition+LocalLayoutOffset;			    			        
		}
		set {
            var offsetWithoutParent= value-LocalAnchorPosition;
            var parent= ParentNode;
		    if(parent == null) {
		        LocalLayoutOffset= offsetWithoutParent;
		        return;
		    }
	        LocalLayoutOffset= offsetWithoutParent-parent.GlobalDisplayPosition;
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
		set {
		    LocalLayoutOffset= value-LocalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 DisplaySize {
		get {
	        if(!IsVisibleOnDisplay) {
	            return Vector2.zero;
            }
		    if(IsPort) {
		        return iCS_EditorConfig.PortSize;
		    }
            // Update ports to match parent node display size.
			var displaySize= AnimatedSize.CurrentValue;
            if(IsNode && Math3D.IsNotEqual(displaySize, myPreviousDisplaySize)) {
                myPreviousDisplaySize= displaySize;
                LayoutPorts();
            }
			return displaySize;
		}
		set {
		    if(IsPort) {
                Debug.LogWarning("iCanScript: Should not set DisplaySize on ports.");
		        return;
	        }
		    AnimatedSize.Reset(value);
		}
	}
    // ----------------------------------------------------------------------
 	public Rect GlobalDisplayRect {
 		get {
            return BuildRect(GlobalDisplayPosition, DisplaySize);
 		}
 		set {
 		    GlobalDisplayPosition= PositionFrom(value);
 		    DisplaySize= SizeFrom(value);
 		}
 	}
    
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
    public void SetGlobalAnchorAndLayoutRect(Rect r) {
        SetGlobalAnchorAndLayoutPosition(PositionFrom(r));
        DisplaySize= SizeFrom(r);
    }
    // ----------------------------------------------------------------------
	public void SetGlobalAnchorAndLayoutPosition(Vector2 pos) {
		GlobalAnchorPosition= pos;
		LocalLayoutOffset= Vector2.zero;
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

	// ======================================================================
    // Child Position Utilities
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
}
