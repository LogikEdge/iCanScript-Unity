using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//public class DSViewWithTitleCell : DSViewWithTitle {
//    // ======================================================================
//    // Fields
//    // ----------------------------------------------------------------------
// 	Action<DSView,Rect>         myDisplayDelegate       = null;
//	Func<DSView,Rect,Vector2>   myGetDisplaySizeDelegate= null;
//   
//    // ======================================================================
//    // Properties
//    // ----------------------------------------------------------------------
//	public Action<DSView,Rect> DisplayDelegate {
//	    get { return myDisplayDelegate; }
//	    set { myDisplayDelegate= value; }
//	}
//	public Func<DSView,Rect,Vector2> GetDisplaySizeDelegate {
//	    get { return myGetDisplaySizeDelegate; }
//	    set { myGetDisplaySizeDelegate= value; }
//	}
//    
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSViewWithTitleCell(RectOffset margins, bool shouldDisplayFrame,
//                               GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
//                               Action<DSView,Rect> displayDelegate= null,
//                               Func<DSView,Rect,Vector2> getDisplaySizeDelegate= null)
//    : base(margins, shouldDisplayFrame, title, titleAlignment, titleSeperator) {
//        DisplayDelegate       = displayDelegate;
//        GetDisplaySizeDelegate= getDisplaySizeDelegate;
//    }
//    
//	// ======================================================================
//    // DSView overrides.
//    // ----------------------------------------------------------------------
//    protected override void DoViewWithTitleDisplay(Rect displayArea) {
//		if(myDisplayDelegate != null) myDisplayDelegate(this, displayArea);        
//    }
//    protected override Vector2 DoViewWithTitleGetDisplaySize(Rect displayArea) {
//		return myGetDisplaySizeDelegate != null ? myGetDisplaySizeDelegate(this, displayArea) : Vector2.zero;        
//    }
//
//}
//