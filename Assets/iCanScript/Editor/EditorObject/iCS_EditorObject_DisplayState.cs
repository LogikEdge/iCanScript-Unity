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
    public bool IsVisible {
        get {
            var parent= Parent;
            if(parent == null) return true;    
            if(parent.IsIconized) return false;
            if(IsNode && parent.IsFolded) return false;
            return parent.IsVisible;            
        }
    }
    // ----------------------------------------------------------------------
    public bool IsVisibleWithAnimation {
        get {
            var area= Math3D.Area(AnimatedDisplaySize);
            if(area < 0.1f) return false;
            return Parent.IsVisibleWithAnimation;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsDisplaySizeAnimated {
        get {
            return  myAnimatedDisplaySize.IsActive;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsDisplayPositionAnimated {
        get {
            return  myAnimatedGlobalDisplayPosition.IsActive;            
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size or position are being animated.
    public bool IsAnimated {
        get {
            if(IsDisplaySizeAnimated) return true;
            return IsDisplayPositionAnimated;
        }
    }
}
