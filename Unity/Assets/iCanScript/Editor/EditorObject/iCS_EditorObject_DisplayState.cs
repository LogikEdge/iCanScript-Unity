/*
   iCS_EditorObject_DisplayState.cs
   iCanScript
   
   Created by Reinual on 2013-02-15.
   Copyright 2013 Infaunier. All rights reserved.
*/

using UnityEngine;
using System.Collections;

public partial class iCS_EditorObject {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------    
    bool myInvisibleBeforeAnimation= false;
    
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
    // We assume that our parent has just unfolded.
    public void Unhide() {
        // Unhide iconized node
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) {
            LocalLayoutOffset= Vector2.zero;
            DisplaySize= Vector2.zero;
            AnimateSize(iCS_Graphics.GetMaximizeIconSize(this));
            return;
        }
        // Unhide folded node
        if(DisplayOption == iCS_DisplayOptionEnum.Folded) {
            LocalLayoutOffset= Vector2.zero;
            DisplaySize= Vector2.zero;
            AnimateSize(SizeFrom(FoldedNodeRect()));
            return;
        }
        // Unhide unfolded node
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Unhide());
            LayoutNode();
        }
    }
    // ----------------------------------------------------------------------
    public void Iconize() {
        // Nothing to do if already iconized.
        if(DisplayOption == iCS_DisplayOptionEnum.Iconized) return;
        // Animate children if previous state was unfolded.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Hide());
        }
        // Set the node has iconized.
        DisplayOption= iCS_DisplayOptionEnum.Iconized;
        LayoutNode();
        LayoutParentNodesUntilTop();
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Fold() {
        // Nothing to do if already fold.
        if(DisplayOption == iCS_DisplayOptionEnum.Folded) return;
        // Animate children node if previous state was unfolded.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) {
            ForEachChildNode(c=> c.Hide());            
        }
        // Set the node has folded.
        DisplayOption= iCS_DisplayOptionEnum.Folded;
        LayoutNode();
        LayoutParentNodesUntilTop();
        IsDirty= true;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        // Nothing to do if already unfold.
        if(DisplayOption == iCS_DisplayOptionEnum.Unfolded) return;
        // Set the node has unfolded.
        DisplayOption= iCS_DisplayOptionEnum.Unfolded;
        // Unhide all children nodes.
        ForEachChildNode(c=> c.Unhide());
        LayoutNode();
        LayoutParentNodesUntilTop();
        IsDirty= true;
    }

}
