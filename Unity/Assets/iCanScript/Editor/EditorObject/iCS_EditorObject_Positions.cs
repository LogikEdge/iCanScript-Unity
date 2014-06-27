using UnityEngine;
using System.Collections;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_EditorObject {   
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	// Fields
    // ======================================================================
	Vector2	myLayoutSize      = Vector2.zero;
	Vector2	myCollisionOffset = Vector2.zero;
	Vector2	myWrappingOffset  = Vector2.zero;
	
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
                // Update nested ports
                ForEachChildPort(
                    p=> {
            			p.UpdatePortEdge(value);
            			p.PortPositionRatio= GetPortRatioFromLocalAnchorPosition(value);			
                    }
                );
				return;
			}
			var engineObject= EngineObject;
			if(Math3D.IsEqual(engineObject.LocalAnchorPosition, value)) {
			    return;
		    }
			engineObject.LocalAnchorPosition= value;
		}
	}
    // ----------------------------------------------------------------------
    public Vector2 AnchorPosition {
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

    // ======================================================================
    // Layout
    // ----------------------------------------------------------------------
	// Node priority when resolving collisions.
	public int LayoutPriority {
		get { return EngineObject.LayoutPriority; }
		set { EngineObject.LayoutPriority= value; }
	}
    // ----------------------------------------------------------------------
	public Vector2 CollisionOffset {
		get {
			return myCollisionOffset;
		}
		set {
			myCollisionOffset= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 WrappingOffset {
		get {
			return myWrappingOffset;
		}
		set {
			myWrappingOffset= value;
		}
	}
    // ----------------------------------------------------------------------
    // Offset from the anchor position.  This attribute is animated.
	public Vector2 LocalOffset {
		get {
			return CollisionOffset+WrappingOffset;
		}
		set {
            // Update parent port for nested ports.
            if(IsPort) {
                if(IsNestedPort) {
                    return;
                }
                myCollisionOffset= value;
                ForEachChildPort(p=> p.myCollisionOffset= value);                
            }
			CollisionOffset= value;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalCollisionPosition {
		get {
			return LocalAnchorPosition+CollisionOffset;
		}
		set {
			CollisionOffset= value-LocalAnchorPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalWrappingPosition {
		get {
			return LocalAnchorPosition+CollisionOffset+WrappingOffset;
		}
		set {
			WrappingOffset= value-LocalAnchorPosition-CollisionOffset;
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
	public Vector2 CollisionPosition {
		get {
			var parent= ParentNode;
			if(parent == null) {
			    return LocalCollisionPosition;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedInLayout) {
				return parent.CollisionPosition;
			}
			if(IsPort) {
				return parent.LayoutPosition+LocalCollisionPosition;
			}
			if(!IsVisibleInLayout) {
			    return parent.LayoutPosition;
			}
    		return parent.LayoutPosition+LocalCollisionPosition;			    			        
		}
		set {
            var offsetWithoutParent= value-LocalAnchorPosition;
            var parent= ParentNode;
		    if(parent == null) {
		        CollisionOffset= offsetWithoutParent;
		        return;
		    }
			if(IsPort) {
				CollisionOffset= offsetWithoutParent-parent.LayoutPosition;
			}
	        CollisionOffset= offsetWithoutParent-parent.LayoutPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 WrappingPosition {
		get {
			if(IsPort) {
				return CollisionPosition;
			}
			if(!IsVisibleInLayout) {
			    return CollisionPosition;
			}
            var parent= ParentNode;
		    if(parent == null) {
		        return CollisionPosition+WrappingOffset;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedInLayout) {
				return CollisionPosition;
			}
    		return CollisionPosition+WrappingOffset;
		}
		set {
			if(IsPort) {
				WrappingOffset= Vector2.zero;
                return;
			}
            var WrappingOffset= value-CollisionPosition;
		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LayoutPosition {
		get {
			var parent= ParentNode;
			if(parent == null) {
			    return LocalAnchorPosition+LocalOffset;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedInLayout) {
				return parent.LayoutPosition;
			}
			if(IsPort) {
				return parent.LayoutPosition+LocalLayoutPosition;
			}
			if(!IsVisibleInLayout) {
			    return parent.LayoutPosition;
			}
    		return parent.LayoutPosition+LocalLayoutPosition;			    			        
		}
		set {
            var offsetWithoutParent= value-LocalAnchorPosition;
            var parent= ParentNode;
		    if(parent == null) {
		        LocalOffset= offsetWithoutParent;
		        return;
		    }
			if(IsPort) {
				LocalOffset= offsetWithoutParent-parent.LayoutPosition;
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
		    if(!IsVisibleInLayout) {
		        return Vector2.zero;
		    }
			return myLayoutSize;
		}
		set {
		    if(IsPort) return;
			// Update animated target position if active.
            if(Math3D.IsNotEqual(myLayoutSize, value)) {
        		myLayoutSize= value;
                LayoutPorts();                
            }
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
			if(IStorage.IsAnimationPlaying) {
    			return myAnimatedRect.CurrentValue;
			}
			var parent= ParentNode;
			if(parent == null || (IsFloating || IsSticky)) {
			    return LayoutRect;
		    }
		    var parentPos= parent.AnimatedPosition;
		    var size= LayoutSize;
		    // Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedOnDisplay) {
				return BuildRect(parentPos, size);
			}
            var pos= parentPos+LocalAnchorPosition+LocalOffset;
            var r= BuildRect(pos, size);
			return r;
		}
	}
	
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
    public void SetInitialPosition(Vector2 pos) {
        var r= BuildRect(pos, Vector2.zero);
        SetAnchorAndLayoutRect(r);
        AnimationStartRect= r;
    }
    // ----------------------------------------------------------------------
    public void SetAnchorAndLayoutRect(Rect r) {
        SetAnchorAndLayoutPosition(PositionFrom(r));
        LayoutSize= SizeFrom(r);
    }
    // ----------------------------------------------------------------------
	public void SetAnchorAndLayoutPosition(Vector2 pos) {
		AnchorPosition = pos;
		CollisionOffset= Vector2.zero;
		WrappingOffset = Vector2.zero;
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
    public Rect ChildRect {
        get {
            Rect childRect= BuildRect(LayoutPosition, Vector2.zero);
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
    					if(c.IsVisibleInLayout) {
        					childRect= Math3D.Merge(childRect, c.LayoutRect);											        
    					}
                    }
				}
            );
            return childRect;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the global rectangle currently used by the children.
    public Rect ChildRectWithMargins {
        get {
            var childRect= ChildRect;
            if(Math3D.IsNotZero(Math3D.Area(childRect))) {
                childRect= AddMargins(childRect);
            }
            return childRect;
        }
    }
}
