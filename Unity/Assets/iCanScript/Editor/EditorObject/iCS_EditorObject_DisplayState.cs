/*
   iCS_EditorObject_DisplayState.cs
   iCanScript
   
   Created by Reinual on 2013-02-15.
   Copyright 2013 Infaunier. All rights reserved.
*/
using UnityEngine;
using System.Collections;
using P=Prelude;

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
		get {
            if(IsPort) return false;
            var area= Math3D.Area(DisplaySize);
            if(Math3D.IsZero(area)) return false;
            var iconArea= Math3D.Area(iCS_Graphics.GetMaximizeIconSize(this));
            return Math3D.IsSmallerOrEqual(area, iconArea) &&
                   Math3D.IsGreater(area, iCS_EditorConfig.kMinIconicArea);
		}
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
            if(IsPort) {
                var parent= ParentNode;
                return parent.IsVisibleOnDisplay;
            }
            if(!IsAnimated) return IsVisibleInLayout;
            var area= Math3D.Area(DisplaySize);
            return Math3D.IsGreater(area, iCS_EditorConfig.kMinIconicArea);
        }
    }

    // ======================================================================
    // Display State Change
    // ----------------------------------------------------------------------
	void PrepareToHide() {
        // Nothing to do if we are not visible.
        if(!IsVisibleOnDisplay) return;
        // Reposition at parent center.
        PrepareToAnimateRect();
        // First hide all children.
        ForEachChildNode(c=> c.Hide());
	}
    // ----------------------------------------------------------------------
	void Hide(P.TimeRatio timeRatio) {
		LocalLayoutOffset= -LocalAnchorPosition;
		DisplaySize= Vector2.zero;
		AnimateRect(timeRatio);
		IsAlphaAnimated= true;
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.Hide(timeRatio));
		}
	}
    // ----------------------------------------------------------------------
    public void Hide() {
        // Nothing to do if we are not visible.
        if(!IsVisibleOnDisplay) return;
        // Reposition at parent center.
        LocalLayoutOffset= -LocalAnchorPosition;
        // First hide all children.
        ForEachChildNode(c=> c.Hide());
        // ... then hide ourself.
        AnimateSize(Vector2.zero);
    }
    // ----------------------------------------------------------------------
	void PrepareToUnhide() {
		AnimatedLayoutOffset.StartValue= -LocalAnchorPosition;
		AnimatedSize.StartValue= Vector2.zero;
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.PrepareToUnhide());
		}		
	}
    // ----------------------------------------------------------------------
	void Unhide(P.TimeRatio timeRatio) {
		AnimateRect(timeRatio);
		IsAlphaAnimated= true;
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.Unhide(timeRatio));
		}
	}
    // ----------------------------------------------------------------------
    // We assume that our parent has just unfolded.
    public void Unhide() {
        // Unhide iconized node
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) {
            LocalLayoutOffset= Vector2.zero;
            DisplaySize= Vector2.zero;
            AnimateSize(IconizedSize());
            return;
        }
        // Unhide folded node
        if(IsFunction || DisplayOption == iCS_DisplayOptionEnum.Folded) {
            LocalLayoutOffset= Vector2.zero;
            DisplaySize= Vector2.zero;
            AnimateRect(FoldedNodeRect());
            return;
        }
        // Unhide unfolded node
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Unhide());
            AnimateRect(UnfoldedNodeRect());
        }
    }
    // ----------------------------------------------------------------------
    public void Iconize() {
        // Nothing to do if already iconized.
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) return;
		// Prepare to hide visible children
		bool animateChildren= DisplayOption == iCS_DisplayOptionEnum.Unfolded;
        if(animateChildren) {
            ForEachChildNode(c=> c.PrepareToHide());
        }
        // Set the node has iconized.
		var prevRect= GlobalDisplayRect;
		PrepareToAnimateRect();
        DisplayOption= iCS_DisplayOptionEnum.Iconized;
		LayoutNode();
		var newRect= GlobalDisplayRect;
		var timeRatio= BuildTimeRatioFromRect(prevRect, newRect);
        // Animate children if previous state was unfolded.
        if(animateChildren) {
            ForEachChildNode(c=> c.Hide(timeRatio));
        }
		AnimateRect(timeRatio);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Fold() {
        // Nothing to do if already fold.
        if(DisplayOption == iCS_DisplayOptionEnum.Folded) return;
		// Prepare to hide visible children
		bool animateChildren= DisplayOption == iCS_DisplayOptionEnum.Unfolded;
        if(animateChildren) {
            ForEachChildNode(c=> c.PrepareToHide());
        }
        // Set the node has folded.
		var prevRect= GlobalDisplayRect;
		PrepareToAnimateRect();
        DisplayOption= iCS_DisplayOptionEnum.Folded;
		LayoutNode();
		var newRect= GlobalDisplayRect;
		var timeRatio= BuildTimeRatioFromRect(prevRect, newRect);
        // Animate children if previous state was unfolded.
        if(animateChildren) {
            ForEachChildNode(c=> c.Hide(timeRatio));
        }
		AnimateRect(timeRatio);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        // Nothing to do if already unfold.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) return;
		// Prepare to hide visible children
        ForEachChildNode(c=> c.PrepareToUnhide());
        // Set the node has unfolded.
		var prevRect= GlobalDisplayRect;
		PrepareToAnimateRect();
        DisplayOption= iCS_DisplayOptionEnum.Unfolded;
		// Perform layout on children first.
		ForEachChildRecursiveDepthFirst(
			(c,fnc)=> {
				if(!c.IsNode) return false;
				if(c.DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
					return true;
				}
				fnc(c);
				return false;
			},
			c=> {
				c.LayoutNode();
			}
		);
		LayoutNode();
		var newRect= GlobalDisplayRect;
		Debug.Log("Unfold Rect= "+newRect);
		var timeRatio= BuildTimeRatioFromRect(prevRect, newRect);
        // Animate children if previous state was unfolded.
        ForEachChildNode(c=> c.Unhide(timeRatio));
		AnimateRect(timeRatio);
        IsDirty= true;
    }

}
