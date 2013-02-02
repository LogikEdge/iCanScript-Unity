using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    public bool IsUnfoldedInLayout  { get { return DisplayOption == iCS_DisplayOptionEnum.Unfolded; }}
    public bool IsFoldedInLayout    { get { return DisplayOption == iCS_DisplayOptionEnum.Folded;   }}
    public bool IsIconizedInLayout  { get { return DisplayOption == iCS_DisplayOptionEnum.Iconized; }}
    public void Iconize()           { DisplayOption= iCS_DisplayOptionEnum.Iconized; IsDirty= true; }
    public void Fold()              { DisplayOption= iCS_DisplayOptionEnum.Folded;   IsDirty= true; }
    public void Unfold()            { DisplayOption= iCS_DisplayOptionEnum.Unfolded; IsDirty= true; }

    // ======================================================================
    // High-order display state functions.
    // ----------------------------------------------------------------------
	public bool IsUnfoldedOnDisplay { get { return IsUnfoldedInLayout; }}
    // ----------------------------------------------------------------------
	public bool IsFoldedOnDisplay	{ get { return IsFoldedInLayout; }}
    // ----------------------------------------------------------------------
	public bool IsIconizedOnDisplay	{ get { return IsIconizedInLayout; }}
    // ----------------------------------------------------------------------
    // Returns true if the object object is visible excluding all animations.
    public bool IsVisibleInLayout {
        get {
            var parent= Parent;
            if(parent == null) return true;    
            if(parent.IsIconizedInLayout) return false;
            if(IsNode && parent.IsFoldedInLayout) return false;
            return parent.IsVisibleInLayout;            
        }
    }
    // ----------------------------------------------------------------------
    public bool IsVisibleOnDisplay {
        get {
            if(!IsAnimated) return IsVisibleInLayout;
            var area= Math3D.Area(DisplaySize);
            return area > 0.1f;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display size is currently being animated.
    public bool IsLayoutSizeAnimated {
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
            if(IsLayoutSizeAnimated) return true;
            return IsDisplayPositionAnimated;
        }
    }

}
