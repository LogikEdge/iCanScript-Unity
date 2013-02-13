using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Queries
    // ----------------------------------------------------------------------    
    public bool IsUnfoldedInLayout  { get { return DisplayOption == iCS_DisplayOptionEnum.Unfolded; }}
    public bool IsFoldedInLayout    { get { return DisplayOption == iCS_DisplayOptionEnum.Folded;   }}
    public bool IsIconizedInLayout  { get { return DisplayOption == iCS_DisplayOptionEnum.Iconized; }}

    // ======================================================================
    // High-order queries.
	public bool IsUnfoldedOnDisplay { get { return IsUnfoldedInLayout; }}
    // ----------------------------------------------------------------------
	public bool IsFoldedOnDisplay	{ get { return IsFoldedInLayout; }}
    // ----------------------------------------------------------------------
	public bool IsIconizedOnDisplay	{
		get { return IsIconizedInLayout && !IsDisplaySizeAnimated; }
	}
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
    public bool IsDisplaySizeAnimated {
        get {
            return  myAnimatedDisplaySize.IsActive;
        }
    }
    // ----------------------------------------------------------------------
    // Returns true if the display position is currently being animated.
    public bool IsDisplayPositionAnimated {
        get {
            return  myAnimatedDisplayPosition.IsActive;            
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

    // ======================================================================
    // Display State Change
    // ----------------------------------------------------------------------    
    public void Iconize() {
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) return;
		SetStartValueForDisplayRectAnimation();
		{
	        DisplayOption= iCS_DisplayOptionEnum.Iconized;
	        LayoutNode();
	        LayoutParentNodesUntilTop();			
		}
		StartDisplayRectAnimation();
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Fold() {
        if(DisplayOption == iCS_DisplayOptionEnum.Folded) return;
		SetStartValueForDisplayRectAnimation();
		{
	        DisplayOption= iCS_DisplayOptionEnum.Folded;
	        LayoutNode();
	        LayoutParentNodesUntilTop();	
		}
		StartDisplayRectAnimation();
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) return;
		SetStartValueForDisplayRectAnimation();
		{
	        DisplayOption= iCS_DisplayOptionEnum.Unfolded;
	        LayoutNode();
	        LayoutParentNodesUntilTop();			
		}
		StartDisplayRectAnimation();
        IsDirty= true;
    }

}
