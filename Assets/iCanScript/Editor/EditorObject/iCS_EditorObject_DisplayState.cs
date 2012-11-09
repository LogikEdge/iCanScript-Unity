using UnityEngine;
using System.Collections;
using P=Prelude;

public partial class iCS_EditorObject {
    // ======================================================================
    // Display animation attributes -----------------------------------------
	private P.Animate<Rect>	myAnimatedPosition= new P.Animate<Rect>();
	public P.Animate<Rect> AnimatedPosition {
		get { return myAnimatedPosition; }
	}

    // Node layout attributes -----------------------------------------------
    public void Unfold()    { EngineObject.Unfold(); IsDirty= true; }
    public void Fold()      { EngineObject.Fold(); IsDirty= true; }
    public void Iconize()   { EngineObject.Iconize(); IsDirty= true; }
    public bool IsUnfolded  { get { return EngineObject.IsUnfolded; }}
    public bool IsFolded    { get { return EngineObject.IsFolded; }}
    public bool IsIconized  { get { return EngineObject.IsIconized; }}
    public bool IsVisible {
        get {
            var parent= Parent;
            if(parent == null) return true;    
            if(parent.IsIconized) return false;
            if(IsNode && parent.IsFolded) return false;
            return parent.IsVisible;            
        }
    }


    // ======================================================================
    public Vector2 DisplaySize {
		get {
			return EngineObject.DisplaySize;
		}
		set {
			EngineObject.DisplaySize= value;
		}
	}
    public Vector2 LocalPosition {
		get {
			return EngineObject.LocalPosition;
		}
		set {
			EngineObject.LocalPosition= value;
		}
	}
    public Rect LocalRect {
		get {
            var displaySize= DisplaySize;
			var localPosition= LocalPosition;
		    float x= localPosition.x-0.5f*displaySize.x;
		    float y= localPosition.y-0.5f*displaySize.y;
		    return new Rect(x, y, displaySize.x, displaySize.y);
		}
		set {
		    float x= value.x+0.5f*value.width;
		    float y= value.y+0.5f*value.height;
		    LocalPosition= new Vector2(x, y);
		    DisplaySize= new Vector2(value.width, value.height);
		}
	}
	public Rect AbsoluteRect {
		get {
			return myIStorage.Storage.GetAbsoluteRect(EngineObject);
		}
		set {
			if(!IsParentValid) {
			    LocalRect= value;
			    return;
		    }
			// First adjust parent display area.
			var parent= Parent;
			var parentRect= parent.AbsoluteRect;
			float margin= iCS_Config.MarginSize;
			var childRect= new Rect(value.x-margin, value.y-margin,
									value.width+2f*margin, value.height+2f*margin);
			var newParentRect= Math3D.Union(childRect, parentRect);
			if(Math3D.IsNotEqual(newParentRect, parentRect)) {
				parent.AbsoluteRect= newParentRect;
			}
			// The parent is now adjusted so we can now set our display area.
			LocalRect= new Rect(value.x-newParentRect.x, value.y-newParentRect.y, value.width, value.height);
		}
	}
    // ----------------------------------------------------------------------
    public Vector2 DisplaySizeWithMargin {
        get {
            var size= DisplaySize;
            var margin2= 2f*iCS_Config.MarginSize;
            return new Vector2(size.x+margin2, size.y+margin2);
        }
    }
    public Rect LocalRectWithMargin {
        get {
            var displaySize= DisplaySizeWithMargin;
			var localPosition= LocalPosition;
		    float x= localPosition.x-0.5f*displaySize.x;
		    float y= localPosition.y-0.5f*displaySize.y;
		    return new Rect(x, y, displaySize.x, displaySize.y);            
        }
    }
    public Rect AbsoluteRectWithMargin {
        get {
            var margin= iCS_Config.MarginSize;
            var margin2= 2f*margin;
            var rect= AbsoluteRect;
            return new Rect(rect.x-margin, rect.y-margin, rect.width+margin2, rect.height+margin2);
        }
    }
}
