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
	Vector2 myWrappingOffset  = Vector2.zero;
	
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
    } // @done
	
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
    } // @done
    // ----------------------------------------------------------------------
    public Vector2 GlobalAnchorPosition {
		get {
			if(IsDisplayRoot) {
                return LocalAnchorPosition;
            }
			var parent= ParentNode;
    		return parent.GlobalPosition+LocalAnchorPosition;			    
		}
		set {
            if(IsDisplayRoot) {
				LocalAnchorPosition= value;
				return;
			}
			var parent= ParentNode;
    		LocalAnchorPosition= value-parent.GlobalPosition;                
		}                
    }

    // ======================================================================
    // Layout
    // ----------------------------------------------------------------------
	public Vector2 CollisionOffset {
		get {
			return myCollisionOffset;
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
	public Vector2 WrappingOffset {
		get {
			return myWrappingOffset;
		}
		set {
			myWrappingOffset= value;
		}
	}
//    // ----------------------------------------------------------------------
//    // Offset from the anchor position.  This attribute is animated.
//	public Vector2 LocalOffset {
//		get {
//			return CollisionOffset;
//		}
//		set {
//            // Update parent port for nested ports.
//            if(IsPort) {
//                if(IsNestedPort) {
//                    return;
//                }
//                myCollisionOffset= value;
//                ForEachChildPort(p=> p.myCollisionOffset= value);                
//            }
//			CollisionOffset= value;
//		}
//	}
    // ----------------------------------------------------------------------
	public Vector2 LocalPosition {
		get {
			return LocalAnchorPosition+CollisionOffset;
		}
//		set {
//			LocalOffset= value-LocalAnchorPosition;
//		}
	}
    // ======================================================================
	// Layout
    // ----------------------------------------------------------------------
	public Vector2 GlobalPosition {
		get {
			var parent= ParentNode;
			if(IsDisplayRoot || parent == null) {
			    return LocalPosition;
		    }
			// Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedInLayout) {
				return parent.GlobalPosition;
			}
			if(IsPort) {
				return parent.GlobalPosition+LocalPosition;
			}
			if(!IsVisibleInLayout) {
			    return parent.GlobalPosition;
			}
    		return parent.GlobalPosition+LocalPosition;
		}
//		set {
//            var offsetWithoutParent= value-LocalAnchorPosition;
//            var parent= ParentNode;
//		    if(IsDisplayRoot || parent == null) {
//		        LocalOffset= offsetWithoutParent;
//		        return;
//		    }
//			if(IsPort) {
//				LocalOffset= offsetWithoutParent-parent.GlobalPosition;
//			}
//	        LocalOffset= offsetWithoutParent-parent.GlobalPosition;
//		}
	}
    // ----------------------------------------------------------------------
	public Vector2 LocalSize {
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
	public Rect GlobalRect {
		get {
			return BuildRect(GlobalPosition, LocalSize);
		}
//		set {
//			GlobalPosition= PositionFrom(value);
//			LocalSize= SizeFrom(value);
//		}
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
			    return GlobalRect;
		    }
		    var parentPos= parent.AnimatedPosition;
		    var size= LocalSize;
		    // Special case for iconized transition module ports.
			if(IsTransitionPort && parent.IsIconizedOnDisplay) {
				return BuildRect(parentPos, size);
			}
            var pos= parentPos+LocalPosition;
            var r= BuildRect(pos, size);
			return r;
		}
	}
	
	// ======================================================================
    // High-order functions
    // ----------------------------------------------------------------------
    public Vector2 LocalAnchorFromGlobalPosition {
        set {
            var parent= ParentNode;
            if(IsDisplayRoot || parent == null) {
                LocalAnchorPosition= value;
                return;
            }
            LocalAnchorPosition= value-parent.GlobalPosition;
            CollisionOffset= Vector2.zero;            
        }
    }
    // ----------------------------------------------------------------------
    public Vector2 CollisionOffsetFromGlobalPosition {
        set {
            var parent= ParentNode;
            if(IsDisplayRoot || parent == null) {
                CollisionOffset= value-LocalAnchorPosition;
                return;
            }
            CollisionOffset= value-parent.GlobalPosition-LocalAnchorPosition;            
        }
    }
    // ----------------------------------------------------------------------
    public Vector2 CollisionOffsetFromLocalPosition {
        set {
            CollisionOffset= value-LocalAnchorPosition;            
        }
    }
    // ----------------------------------------------------------------------
    public Rect LocalAnchorFromGlobalRect {
        set {
            LocalAnchorFromGlobalPosition= PositionFrom(value);
            LocalSize= SizeFrom(value);            
        }
    }
    // ----------------------------------------------------------------------
    public Rect CollisionOffsetFromGlobalRect {
        set {
            CollisionOffsetFromGlobalPosition= PositionFrom(value);
            LocalSize= SizeFrom(value);            
        }
    }
    // ----------------------------------------------------------------------
    public void SetInitialPosition(Vector2 pos) {
        var r= BuildRect(pos, Vector2.zero);
        LocalAnchorFromGlobalRect= r;
        AnimationStartRect= r;
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
    // ----------------------------------------------------------------------
	// Node priority when resolving collisions.
	public int LayoutPriority {
		get { return EngineObject.LayoutPriority; }
		set { EngineObject.LayoutPriority= value; }
	}
    
	// ======================================================================
    // Child Position Utilities
 	// ----------------------------------------------------------------------
    public Rect ChildRect {
        get {
            Rect childRect= BuildRect(GlobalPosition, Vector2.zero);
            ForEachChildNode(
                c=> {
                    if(!c.IsFloating) {
    					if(c.IsVisibleInLayout) {
        					childRect= Math3D.Merge(childRect, c.GlobalRect);											        
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
