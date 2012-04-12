using UnityEngine;
using System;
using System.Collections;

public abstract class DSScrollView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2                     myScrollPosition        = Vector2.zero;
    Vector2                     myContentSize           = Vector2.zero;
   
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	Rect ContentArea { get { return new Rect(0,0,myContentSize.x,myContentSize.y); }}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSScrollView(RectOffset margins, bool shouldDisplayFrame= true)
    : base(margins, shouldDisplayFrame) {}
    
    // ======================================================================
    // DSView overrides.
    // ----------------------------------------------------------------------
    protected override void DoDisplay(Rect displayArea) {
        myScrollPosition= GUI.BeginScrollView(DisplayArea, myScrollPosition, ContentArea, false, false);
            DoScrollViewDisplay(displayArea);
        GUI.EndScrollView();
    }
    protected override Vector2 DoGetDisplaySize(Rect displayArea) {
		myContentSize= DoScrollViewGetDisplaySize(displayArea);
        // Add scroller if the needed display size exceeds the display area.
		var contentSize= myContentSize;        
        if(displayArea.width < contentSize.x) contentSize.y+= kScrollerSize;
        if(displayArea.height < contentSize.y) contentSize.x+= kScrollerSize;
        return contentSize;
    }

    // ======================================================================
    // Functions to override to create a concrete view.
    // ----------------------------------------------------------------------
    protected abstract void    DoScrollViewDisplay(Rect displayArea);
    protected abstract Vector2 DoScrollViewGetDisplaySize(Rect displayArea); 
}
