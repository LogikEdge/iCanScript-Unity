using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
//  DISPLAY STATE
// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
public partial class iCS_EditorObject {
    // ======================================================================
    public bool IsUnfolded          { get { return DisplayOption == iCS_DisplayOptionEnum.Unfolded; }}
    public bool IsFolded            { get { return DisplayOption == iCS_DisplayOptionEnum.Folded;   }}
    public bool IsIconized          { get { return DisplayOption == iCS_DisplayOptionEnum.Iconized; }}
    public void Iconize()           { DisplayOption= iCS_DisplayOptionEnum.Iconized; IsDirty= true; }
    public void Fold()              { DisplayOption= iCS_DisplayOptionEnum.Folded;   IsDirty= true; }
    public void Unfold()            { DisplayOption= iCS_DisplayOptionEnum.Unfolded; IsDirty= true; }

    // ======================================================================
    // High-order display state functions.
    // ----------------------------------------------------------------------
    // Returns true if the object object is visible excluding all animations.
	/*
		FIXME : Need to have a IsVisible for animated objects.
	*/
    public bool IsVisibleInLayout {
        get {
            var parent= Parent;
            if(parent == null) return true;    
            if(parent.IsIconized) return false;
            if(IsNode && parent.IsFolded) return false;
            return parent.IsVisibleInLayout;            
        }
    }
    // ----------------------------------------------------------------------
    public bool IsVisibleInAnimation {
        get {
            if(!IsAnimated) return IsVisibleInLayout;
            var area= Math3D.Area(AnimatedLayoutSize);
            return area > 0.1f;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsLayoutSizeAnimated {
        get {
            return  myAnimatedLayoutSize.IsActive;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsDisplayPositionAnimated {
        get {
            return  myAnimatedGlobalLayoutPosition.IsActive;            
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get {
            if(IsLayoutSizeAnimated) return true;
            return IsDisplayPositionAnimated;
        }
    }
}
