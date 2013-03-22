/*
   iCS_EditorObject_DisplayState.cs
   iCanScript
   
   Created by Reinual on 2013-02-15.
   Copyright 2013 Infaunier. All rights reserved.
*/
using UnityEngine;
using System;
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
			if(!IsAnimated) {
				if(!IsVisibleInLayout) return false;
				return IsIconizedInLayout;
			}
            var area= Math3D.Area(AnimatedSize);
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
                return ParentNode.IsVisibleOnDisplay;
            }
            if(!IsAnimated) return IsVisibleInLayout;
            var area= Math3D.Area(AnimatedSize);
            return Math3D.IsGreater(area, iCS_EditorConfig.kMinIconicArea);
        }
    }

    // ======================================================================
    // Display State Change
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
		var timeRatio= AnimateGraph(
			o=> o.DisplayOption= iCS_DisplayOptionEnum.Iconized
		);
        // Animate children if previous state was unfolded.
        if(animateChildren) {
            ForEachChildNode(c=> c.Hide(timeRatio));
        }
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
		var timeRatio= AnimateGraph(
			o=> o.DisplayOption= iCS_DisplayOptionEnum.Folded
		);
        // Animate children if previous state was unfolded.
        if(animateChildren) {
            ForEachChildNode(c=> c.Hide(timeRatio));
        }
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        // Nothing to do if already unfold.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) return;
		// Prepare to hide visible children
        ForEachChildNode(c=> c.PrepareToUnhide());
        // Set the node has unfolded.
		var timeRatio= AnimateGraph(
			o=> {
		        o.DisplayOption= iCS_DisplayOptionEnum.Unfolded;
				// Perform layout on children first.
				o.ForEachChildRecursiveDepthFirst(
					(c,fnc)=> {
						if(!c.IsNode) return false;
						if(c.DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
							return true;
						}
						fnc(c);
						return false;
					},
					c=> {
						c.LayoutNode(iCS_AnimationControl.None);
					}
				);
			}
		);
        // Animate children if previous state was unfolded.
        ForEachChildNode(c=> c.Unhide(timeRatio));
        IsDirty= true;
    }
    // ======================================================================
    // Animation High-Order Utilities
    // ----------------------------------------------------------------------
	public P.TimeRatio AnimateGraph(Action<iCS_EditorObject> fnc) {
		AnimationStart= LayoutRect;
		PrepareToAnimateParents();
		fnc(this);
		LayoutNode(iCS_AnimationControl.None);
		LayoutParentNodesUntilTop(iCS_AnimationControl.None);
		var newRect= LayoutRect;
		var timeRatio= BuildTimeRatioFromRect(AnimationStart, newRect);		
		Animate(LayoutRect, timeRatio);
		AnimateParents(timeRatio);
		return timeRatio;
	}
    // ----------------------------------------------------------------------
	void PrepareToHide() {
        // Nothing to do if we are not visible.
        if(!IsVisibleOnDisplay) return;
        // Reposition at parent center.
        AnimationStart= LayoutRect;
        // First hide all children.
        ForEachChildNode(c=> c.PrepareToHide());
	}
    // ----------------------------------------------------------------------
	void Hide(P.TimeRatio timeRatio) {
		var target= BuildRect(ParentNode.LayoutPosition, Vector2.zero);
		Animate(target, timeRatio);
		IsAlphaAnimated= true;
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.Hide(timeRatio));
		}
	}
    // ----------------------------------------------------------------------
	void PrepareToUnhide() {
		AnimationStart= BuildRect(ParentNode.LayoutPosition, Vector2.zero);
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.PrepareToUnhide());
		}		
	}
    // ----------------------------------------------------------------------
	void Unhide(P.TimeRatio timeRatio) {
		Animate(LayoutRect, timeRatio);
		IsAlphaAnimated= true;
		if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
			ForEachChildNode(c=> c.Unhide(timeRatio));
		}
	}
    // ----------------------------------------------------------------------
    // We assume that our parent has just unfolded.
    public void Unhide() {
		var start= BuildRect(ParentNode.LayoutPosition, Vector2.zero);
        // Unhide iconized node
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) {
			Animate(start, LayoutRect);
            return;
        }
        // Unhide folded node
        if(IsFunction || DisplayOption == iCS_DisplayOptionEnum.Folded) {
			Animate(start, LayoutRect);
            return;
        }
        // Unhide unfolded node
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Unhide());
			Animate(start, LayoutRect);
        }
    }
    // ----------------------------------------------------------------------
    public void PrepareToAnimateParents() {
        var parent= ParentNode;
        if(parent == null) return;
        parent.AnimationStart= parent.LayoutRect;
        parent.PrepareToAnimateParents();
    }
    // ----------------------------------------------------------------------
    public void AnimateParents(P.TimeRatio timeRatio) {
        var parent= ParentNode;
        if(parent == null) return;
        parent.Animate(parent.LayoutRect, timeRatio);
        parent.AnimateParents(timeRatio);        
    }
    

}
