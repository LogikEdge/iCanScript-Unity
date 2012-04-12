using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSViewCell : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
 	Action<DSView,Rect>         myDisplayDelegate       = null;
	Func<DSView,Rect,Vector2>   myGetDisplaySizeDelegate= null;
   
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public Action<DSView,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSView,Rect,Vector2> GetDisplaySizeDelegate {
	    get { return myGetDisplaySizeDelegate; }
	    set { myGetDisplaySizeDelegate= value; }
	}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSViewCell(RectOffset margins, bool shouldDisplayFrame= true,
                      Action<DSView,Rect> displayDelegate= null,
                      Func<DSView,Rect,Vector2> getDisplaySizeDelegate= null)
    : base(margins, shouldDisplayFrame) {
        DisplayDelegate       = displayDelegate;
        GetDisplaySizeDelegate= getDisplaySizeDelegate;
    }
    
	// ======================================================================
    // DSView overrides.
    // ----------------------------------------------------------------------
    protected override void DoDisplay(Rect displayArea) {
		if(myDisplayDelegate != null) myDisplayDelegate(this, displayArea);        
    }
    protected override Vector2 DoGetDisplaySize(Rect displayArea) {
		return myGetDisplaySizeDelegate != null ? myGetDisplaySizeDelegate(this, displayArea) : Vector2.zero;        
    }

}
