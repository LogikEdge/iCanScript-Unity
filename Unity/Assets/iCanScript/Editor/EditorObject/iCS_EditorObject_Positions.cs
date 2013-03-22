using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {   
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	// Fields
    // ======================================================================
	Vector2		myLayoutSize = Vector2.zero;
	Vector2		myLocalOffset= Vector2.zero;
	
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
    public Vector2 AnimatedAnchorPosition {
		get {
			var parent= ParentNode;
			if(parent == null) return LocalAnchorPosition;
    		return parent.AnimatedPosition+LocalAnchorPosition;			    
		}
		set {
			var parent= ParentNode;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
    		LocalAnchorPosition= value-parent.AnimatedPosition;                
		}        
    }
    // ----------------------------------------------------------------------
    public Vector2 LayoutAnchorPosition {
		get {
			var parent= ParentNode;
			if(parent == null) return LocalAnchorPosition;
    		return parent.LayoutPosition+LocalAnchorPosition;			    
		}
		set {
			var parent= ParentNode;
			if(parent == null) {
				LocalAnchorPosition= value;
				return;
			}
    		LocalAnchorPosition= value-parent.LayoutPosition;                
		}                
    }
    // ----------------------------------------------------------------------	
	public Vector2 AnchorPosition {
		get {
            return LayoutAnchorPosition;
		}
		set {
            LayoutAnchorPosition= value;
		}
	}

    // ======================================================================
    // Layout
    // ----------------------------------------------------------------------
    // Offset from the anchor position.  This attribute is animated.
	public Vector2 LocalOffset {
		get {
			return myLocalOffset;
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
                myLocalOffset= value;
                ForEachChildPort(p=> p.myLocalOffset= value);                
            }
			myLocalOffset= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalLayoutPosition {
		get {
			return LocalAnchorPosition+LocalOffset;
		}
		set {
			LocalOffset= value-LocalAnchorPosition;
		}
	}
    // ======================================================================
	// Layout
    // ----------------------------------------------------------------------
	public Vector2 LayoutPosition {
		get {
			var parent= ParentNode;
			if(parent == null) {
			    return LocalAnchorPosition+LocalOffset;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedOnDisplay) {
				return parent.AnimatedPosition;
			}
			if(IsPort) {
				return parent.AnimatedPosition+LocalAnchorPosition+LocalOffset;
			}
    		return parent.LayoutPosition+LocalAnchorPosition+LocalOffset;			    			        
		}
		set {
            var offsetWithoutParent= value-LocalAnchorPosition;
            var parent= ParentNode;
		    if(parent == null) {
		        LocalOffset= offsetWithoutParent;
		        return;
		    }
			if(IsPort) {
				LocalOffset= offsetWithoutParent-parent.AnimatedPosition;
			}
	        LocalOffset= offsetWithoutParent-parent.LayoutPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LayoutSize {
		get {
		    if(IsPort) {
		        return iCS_EditorConfig.PortSize;
		    }
			return myLayoutSize;
		}
		set {
		    if(IsPort) return;
    		myLayoutSize= value;
            LayoutPorts();
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
	
	// ======================================================================
	// Animation
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
    // ----------------------------------------------------------------------
	public Vector2 AnimatedSize {
		get {
			return SizeFrom(AnimatedRect);
		}
	}
    // ----------------------------------------------------------------------
    public Rect AnimatedRect {
		get {
			if(IsAnimated) {
    			return myAnimatedRect.CurrentValue;
			}
			var parent= ParentNode;
			if(parent == null || (IsNode && (IsFloating || IsSticky))) {
			    return LayoutRect;
		    }
		    var parentPos= parent.AnimatedPosition;
		    var size= LayoutSize;
		    // Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedOnDisplay) {
				return BuildRect(parentPos, size);
			}
            // FIXME: This is problematic because we are using the Anchor+Offset from the Layout.
            var pos= parentPos+LocalAnchorPosition+LocalOffset;
            var r= BuildRect(pos, size);
			return r;
		}
	}
    // ----------------------------------------------------------------------
	public Rect AnimationStart {
		get {
			return myAnimatedRect.StartValue;
		}
		set {
			myAnimatedRect.StartValue= value;
		}
	}
    // ----------------------------------------------------------------------
	public Rect AnimationTarget {
		get {
			return myAnimatedRect.TargetValue;
		}
		set {
			myAnimatedRect.TargetValue= value;
		}
	}
    // ----------------------------------------------------------------------
	public void Animate(Rect start, Rect target) {
		AnimationStart= start;
		Animate(target);
	}
    // ----------------------------------------------------------------------
	public void Animate(Rect start, Rect target, P.TimeRatio timeRatio) {
		AnimationStart= start;
		Animate(target, timeRatio);
	}
    // ----------------------------------------------------------------------
	public void Animate(Rect target, P.TimeRatio timeRatio) {
		AnimationTarget= target;
		Animate(timeRatio);
	}
    // ----------------------------------------------------------------------
	public void Animate(Rect target) {
		AnimationTarget= target;
		Animate();
	}
    // ----------------------------------------------------------------------
	public void Animate() {
		Animate(BuildTimeRatioFromRect(myAnimatedRect.StartValue, myAnimatedRect.TargetValue));
	}
    // ----------------------------------------------------------------------
	public void Animate(P.TimeRatio timeRatio) {
		myAnimatedRect.Start(timeRatio);
	}
	
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
    public void SetAnchorAndLayoutRect(Rect r) {
        SetAnchorAndLayoutPosition(PositionFrom(r));
        LayoutSize= SizeFrom(r);
    }
    // ----------------------------------------------------------------------
	public void SetAnchorAndLayoutPosition(Vector2 pos) {
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
            Rect childRect= BuildRect(AnimatedPosition, Vector2.zero);
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
    					if(c.IsVisibleOnDisplay) {
        					childRect= Math3D.Merge(childRect, c.AnimatedRect);											        
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
