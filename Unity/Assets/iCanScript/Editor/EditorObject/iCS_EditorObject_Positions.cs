using UnityEngine;
using System.Collections;
using P= Prelude;
using Prefs= iCS_PreferencesController;

public partial class iCS_EditorObject {   
	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
	// Fields
    // ======================================================================
	Vector2		myLayoutSize = Vector2.zero;
	Vector2		myLocalOffset= Vector2.zero;
	Vector2		myAnchorCache= Vector2.zero;  // Used for undo/redo animation
	
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
			myAnchorCache= value;
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
    // Offset from the anchor position.  This attribute is animated.
	public Vector2 LocalOffset {
		get {
			return myLocalOffset;
		}
		set {
            // Update parent port for nested ports.
            if(IsPort) {
                if(IsNestedPort) {
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
			if(IsTransitionPort && parent.IsIconizedInLayout) {
				return parent.LayoutPosition;
			}
			if(IsPort) {
				return parent.LayoutPosition+LocalAnchorPosition+LocalOffset;
			}
			if(!IsVisibleInLayout) {
			    return parent.LayoutPosition;
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
    // ----------------------------------------------------------------------
	public Rect CachedLayoutRect {
		get {
			var parent= ParentNode;
			var localPos= myAnchorCache+myLocalOffset;
			if(parent == null) {
				return BuildRect(localPos, myLayoutSize);
			}
			var parentPos= PositionFrom(parent.CachedLayoutRect);
			return BuildRect(parentPos+localPos, myLayoutSize);
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
    // ----------------------------------------------------------------------
	public Rect AnimationStart {
		get {
			return myAnimatedRect.StartValue;
		}
		set {
			myAnimatedRect.StartValue= myAnimatedRect.IsActive ? myAnimatedRect.CurrentValue : value;
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
