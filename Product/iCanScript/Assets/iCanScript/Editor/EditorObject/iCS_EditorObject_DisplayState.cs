/*
   iCS_EditorObject_DisplayState.cs
   iCanScript
   
   Created by Reinual on 2013-02-15.
   Copyright 2013 Infaunier. All rights reserved.
*/
using UnityEngine;
using System;
using System.Collections;
using P=iCanScript.Internal.Prelude;
using iCanScript;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public partial class iCS_EditorObject {
        // ======================================================================
        // Queries
        // ----------------------------------------------------------------------    
        public bool IsHidden {
            get {
                return IsTypeCast;
            }
        }
        public bool IsUnfoldedInLayout  {
            get {
                if(this == IStorage.DisplayRoot) return true;
                if(IsHidden) return false;
                return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Unfolded;
            }
        }
        public bool IsFoldedInLayout {
            get {
                if(this == IStorage.DisplayRoot) return false;
                if(IsHidden) return false;
                return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Folded;
            }
        }
        public bool IsIconizedInLayout {
            get {
                if(this == IStorage.DisplayRoot) return false;
                if(IsHidden) return false;
                return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Iconized;
            }
        }

        // ======================================================================
        // High-order queries.
        // ----------------------------------------------------------------------
    	public bool IsIconizedOnDisplay	{
    		get {
                if(IsPort) return false;
                if(this == IStorage.DisplayRoot) return false;
    			if(!IsAnimated) {
    				if(!IsVisibleInLayout) return false;
    				return IsIconizedInLayout;
    			}
                var area= Math3D.Area(AnimatedSize);
                if(Math3D.IsZero(area)) return false;
                var iconSize= iCS_Graphics.GetMaximizeIconSize(this);
                var iconArea= Math3D.Area(iconSize);
                return Math3D.IsSmallerOrEqual(area, iconArea) &&
                       Math3D.IsSmallerOrEqual(AnimatedSize.x, iconSize.x) &&
                       Math3D.IsSmallerOrEqual(AnimatedSize.y, iconSize.y) &&
                       Math3D.IsGreater(area, iCS_EditorConfig.kMinIconicArea);
    		}
    	}
        // ----------------------------------------------------------------------
        // Returns true if the object object is visible excluding all animations.
        public bool IsVisibleInLayout {
            get {
                if(this == IStorage.DisplayRoot) return true;
                if(IsHidden) return false;
                var parent= Parent;
                if(parent == null) return true;
                if(parent == IStorage.DisplayRoot) return true;    
                if(parent.DisplayOption == iCS_DisplayOptionEnum.Iconized) return false;
                if(IsNode) {
                    if(parent.DisplayOption == iCS_DisplayOptionEnum.Folded) return false;
                    if(parent.IsInstanceNode && !IsInstanceNode) return false;                
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
                    return true;
                }
                if(this == IStorage.DisplayRoot) return true;
                if(!IsAnimated) return IsVisibleInLayout;
                var area= Math3D.Area(AnimatedSize);
                return Math3D.IsGreater(area, iCS_EditorConfig.kMinIconicArea);
            }
        }

        // ======================================================================
        // Display State Change
        // ----------------------------------------------------------------------
        public void Iconize() {
            if(DisplayOption != iCS_DisplayOptionEnum.Iconized) {
                WrappingOffset= Vector2.zero;
            	ReduceChildrenAnchorPosition();
        		DisplayOption= iCS_DisplayOptionEnum.Iconized;
            }
        }
        // ----------------------------------------------------------------------    
        public void Fold() {
    		if(DisplayOption != iCS_DisplayOptionEnum.Folded) {
                WrappingOffset= Vector2.zero;
    			ReduceChildrenAnchorPosition();
        		DisplayOption= iCS_DisplayOptionEnum.Folded;
    		}
        }
        // ----------------------------------------------------------------------    
        public void Unfold() {
            if(IsInstanceNode || IsKindOfFunction) {
                Fold();
                return;
            }
            if(DisplayOption != iCS_DisplayOptionEnum.Unfolded) {
                WrappingOffset= Vector2.zero;
                ReduceChildrenAnchorPosition();
                DisplayOption= iCS_DisplayOptionEnum.Unfolded;
            }
        }
    }
}

