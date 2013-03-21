using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {   
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
                return IsNestedPort ? Parent.LocalAnchorPosition : GetPortLocalAnchorPositionFromRatio();    			
            }
            return EngineObject.LocalAnchorPosition;
		}
		set {
			if(IsPort) {
                // Update the parent if the port is a nested port (avoid circular loop).
			    if(IsNestedPort && Math3D.IsNotEqual(Parent.LocalAnchorPosition, value)) {
			        Parent.LocalAnchorPosition= value;
                    return;
			    }
                // Transform to a position ratio between 0f and 1f.
    			UpdatePortEdge(value);
    			PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);			
    			IsDirty= true;  // Save new anchor position.
                // Update nested ports
                ForEachChildPort(
                    p=> {
            			p.UpdatePortEdge(value);
            			p.PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);			
            			p.IsDirty= true;  // Save new anchor position.
                    }
                );
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
	public Vector2 AnchorPosition {
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
	public Vector2 LocalOffset {
		get {
			return AnimatedLayoutOffset.CurrentValue;
		}
		set {
            // Update parent port for nested ports.
            if(IsPort) {
                if(IsNestedPort) {
                    if(Math3D.IsNotEqual(Parent.LocalOffset, value)) {
                        Parent.LocalOffset= value;
                    }
                    return;
                }
                AnimatedLayoutOffset.Reset(value);
                ForEachChildPort(p=> p.AnimatedLayoutOffset.Reset(value));                
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
			    return LocalAnchorPosition+LocalOffset;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedOnDisplay) {
				return parent.GlobalDisplayPosition;
			}
    		return parent.GlobalDisplayPosition+LocalAnchorPosition+LocalOffset;			    			        
		}
		set {
            var offsetWithoutParent= value-LocalAnchorPosition;
            var parent= ParentNode;
		    if(parent == null) {
		        LocalOffset= offsetWithoutParent;
		        return;
		    }
	        LocalOffset= offsetWithoutParent-parent.GlobalDisplayPosition;
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
		    LocalOffset= value-LocalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 DisplaySize {
		get {
		    if(IsPort) {
		        return iCS_EditorConfig.PortSize;
		    }
			return AnimatedSize.CurrentValue;
		}
		set {
		    if(IsPort) return;
    		AnimatedSize.Reset(value);
            LayoutPorts();
		}
	}
    // ----------------------------------------------------------------------
 	public Rect GlobalDisplayRect {
 		get {
            var r= BuildRect(GlobalDisplayPosition, DisplaySize);
            return r;
 		}
 		set {
 		    GlobalDisplayPosition= PositionFrom(value);
 		    DisplaySize= SizeFrom(value);
 		}
 	}

    // ----------------------------------------------------------------------
	public Vector2 LayoutPosition {
		get {
			return GlobalDisplayPosition;
		}
		set {
			GlobalDisplayPosition= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LayoutSize {
		get {
			return DisplaySize;
		}
		set {
			DisplaySize= value;
		}
	}
    // ----------------------------------------------------------------------
	public Rect LayoutRect {
		get {
			return BuildRect(LayoutPosition, LayoutSize);
		}
		set {
			LayoutPosition= PositionFrom(value);
			LayoutSize= SizeFrom(value);
		}
	}
	
    // ----------------------------------------------------------------------
	public Vector2 AnimatedPosition {
		get {
			return PositionFrom(AnimatedRect);
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 AnimatedLocalPosition {
		get {
			var parent= ParentNode;
			if(parent == null) return AnimatedPosition;
			return AnimatedPosition-parent.AnimatedPosition;
		}
	}
//    // ----------------------------------------------------------------------
//	public Vector2 AnimatedSize {
//		get {
//			return SizeFrom(AnimatedRect);
//		}
//	}
    // ----------------------------------------------------------------------
    public Rect AnimatedRect {
		get {
			return LayoutRect;
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
		AnchorPosition= pos;
		LocalOffset= Vector2.zero;
	}

	// ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    public static Rect BuildRect(Vector2 pos, Vector2 size) {
        return new Rect(pos.x-0.5f*size.x, pos.y-0.5f*size.y, size.x, size.y);
    }
    // ----------------------------------------------------------------------
    public static Vector2 PositionFrom(Rect r) {
        return Math3D.Middle(r);
    }
    // ----------------------------------------------------------------------
    public static Vector2 SizeFrom(Rect r) {
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
                c=> {
                    if(!c.IsFloating) {
    					if(c.IsVisibleOnDisplay) {
        					childRect= Math3D.Merge(childRect, c.GlobalDisplayRect);											        
    					}
                    }
				}
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
