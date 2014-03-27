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
            if(this == IStorage.DisplayRoot) return true;
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Unfolded;
        }
    }
    public bool IsFoldedInLayout {
        get {
            if(this == IStorage.DisplayRoot) return false;
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Folded;
        }
    }
    public bool IsIconizedInLayout {
        get {
            if(this == IStorage.DisplayRoot) return false;
            return IsVisibleInLayout && DisplayOption == iCS_DisplayOptionEnum.Iconized;
        }
    }

    // ======================================================================
    // High-order queries.
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
                          
                // Don't display function "this" port if under object instance node.
                var instanceName= iCS_Strings.DefaultInstanceName;
                if(Name == instanceName && parentNode.IsKindOfFunction) {
                    var grandParentNode= parentNode.ParentNode;
                    if(grandParentNode.IsInstanceNode && IsSourceValid && ProviderPort.ParentNode == grandParentNode && ProviderPort.Name == instanceName) {
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
		DisplayOption= iCS_DisplayOptionEnum.Iconized;
    }
    // ----------------------------------------------------------------------    
    public void Fold() {
		DisplayOption= iCS_DisplayOptionEnum.Folded;
    }
    // ----------------------------------------------------------------------    
    public void Unfold() {
        DisplayOption= iCS_DisplayOptionEnum.Unfolded;
    }

}
