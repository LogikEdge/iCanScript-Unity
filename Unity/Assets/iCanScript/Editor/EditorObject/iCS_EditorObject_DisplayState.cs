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
    public bool IsUnfoldedInLayout  {
        get {
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Unfolded;
        }
    }
    public bool IsFoldedInLayout {
        get {
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Folded;
        }
    }
    public bool IsIconizedInLayout {
        get {
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Iconized;
        }
    }

    // ======================================================================
    // High-order queries.
    // FIX: Fold/Unfold not properly functional when animated.
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
            if(parent.DisplayOption == iCS_DisplayOptionEnum.Iconized) return false;
            if(IsNode) {
                if(parent.DisplayOption == iCS_DisplayOptionEnum.Folded) return false;
                if(parent.IsObjectInstance && !IsObjectInstance) return false;                
            }
            return parent.IsVisibleInLayout;            
        }
    }
    // ----------------------------------------------------------------------
    public bool IsVisibleOnDisplay {
        get {
            if(IsPort) {
                var parentNode= ParentNode;
                if(!parentNode.IsVisibleOnDisplay) return false;
                
                
                
                // Don't display function "this" port if under object instance node.
                var instanceName= iCS_Strings.InstanceObjectName;
                if(Name == instanceName && parentNode.IsKindOfFunction) {
                    var grandParentNode= parentNode.ParentNode;
                    if(grandParentNode.IsObjectInstance && IsSourceValid && Source.ParentNode == grandParentNode && Source.Name == instanceName) {
                        return false;
                    }
                }
                return true;
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
        // Set the node has iconized.
		SetAsHighestLayoutPriority();
		AnimateGraph(
			o=> o.DisplayOption= iCS_DisplayOptionEnum.Iconized
		);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Fold() {
        // Nothing to do if already fold.
        if(DisplayOption == iCS_DisplayOptionEnum.Folded) return;
        // Set the node has folded.
		SetAsHighestLayoutPriority();
		AnimateGraph(
			o=> o.DisplayOption= iCS_DisplayOptionEnum.Folded
		);
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        // Nothing to do if already unfold.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) return;
        // Set the node has unfolded.
		SetAsHighestLayoutPriority();
		AnimateGraph(
			o=> {
				// Keep a copy of the anchor position.
				var savedAnchor= o.AnchorPosition;
		        o.DisplayOption= iCS_DisplayOptionEnum.Unfolded;
				// Relayout chidlren nodes.
				myIStorage.ForcedRelayoutOfTree(o);
				o.SetAnchorAndLayoutPosition(savedAnchor);
			}
		);
        IsDirty= true;
    }
    // ======================================================================
    // Animation High-Order Utilities
    // ----------------------------------------------------------------------
	void Hide(P.TimeRatio timeRatio) {
		var target= BuildRect(ParentNode.LayoutPosition, Vector2.zero);
		Animate(target, timeRatio);
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
    // We assume that our parent has just unfolded.
    public void Unhide() {
        var parent= ParentNode;
        if(parent ==  null) return;
		var start= BuildRect(ParentNode.LayoutPosition, Vector2.zero);
        // Unhide iconized node
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) {
			Animate(start, LayoutRect);
            return;
        }
        // Unhide folded node
        if(IsKindOfFunction || DisplayOption == iCS_DisplayOptionEnum.Folded) {
			Animate(start, LayoutRect);
            return;
        }
        // Unhide unfolded node
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Unhide());
			Animate(start, LayoutRect);
        }
    }

}
