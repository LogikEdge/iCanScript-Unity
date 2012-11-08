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
	public Vector2 RelativePosition {
		get {
			return EngineObject.RelativePosition;
		}
		set {
			EngineObject.RelativePosition= value;
		}
	}
    public Vector2 LocalPosition {
		get {
			return myIStorage.Storage.GetLocalPosition(EngineObject);
		}
		set {
			myIStorage.Storage.SetLocalPosition(EngineObject, value);
		}
	}
    public Rect LocalRect {
		get {
            var displaySize= EngineObject.DisplaySize;
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
			float gutter= iCS_Config.GutterSize;
			var childRect= new Rect(value.x-gutter, value.y-gutter,
									value.width+2f*gutter, value.height+2f*gutter);
			var newParentRect= Math3D.Union(childRect, parentRect);
			if(Math3D.IsNotEqual(newParentRect, parentRect)) {
				parent.AbsoluteRect= newParentRect;
			}
			// The parent is now adjusted so we can now set our display area.
			LocalRect= new Rect(value.x-newParentRect.x, value.y-newParentRect.y, value.width, value.height);
		}
	}

}
