using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect            myFrameArea;
    Rect            myContentArea;
    RectOffset      myMargins;
    List<DSView>    mySubviews= new List<DSView>();

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Rect FrameArea {
        get { return myFrameArea; }
        set { myFrameArea= value; UpdateContentArea(); }
    }
    public Rect ContentArea {
        get { return myContentArea; }
		set { FrameArea= new Rect(value.x-myMargins.left, value.y-myMargins.top, value.width+myMargins.horizontal, value.height+myMargins.vertical); UpdateContentArea(); }
    }
    public List<DSView> Subviews {
        get { return mySubviews; }
    }
    public RectOffset Margins {
        get { return myMargins; }
        set { myMargins= value; UpdateContentArea(); }
    }

    // ======================================================================
    // Common view constants
    // ----------------------------------------------------------------------
    const float   kHorizontalSpacer= 8f;
    const float   kVerticalSpacer  = 8f;
    const float   kHorizontalMargin= 10f;
    const float   kVerticalMargin  = 10f;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSView(RectOffset margins, Rect frameArea) {
        Margins= margins;
        FrameArea= frameArea;
    }
    public DSView(RectOffset margins) : this(margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public virtual void Display(Rect frameArea) { FrameArea= frameArea; Display(); }
    public virtual void Display()               {}

    // ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public virtual void AddSubview(DSView subview) {
        mySubviews.Add(subview);
    }
    public virtual bool RemoveSubview(DSView subview) {
        return mySubviews.Remove(subview);
    }
    public void ForEachSubview(Action<DSView> action) {
        foreach(var sv in mySubviews) {
            action(sv);
        }
    }
    public bool SearchInSubviews(Func<DSView,bool> fnc) {
        foreach(var sv in mySubviews) {
            if(fnc(sv)) return true;
        }
        return false;
    }
    
    // ======================================================================
    // View area management.
    // ----------------------------------------------------------------------
    void UpdateContentArea() {
        myContentArea= myFrameArea;
        myContentArea.x+= myMargins.left;
        myContentArea.width-= myMargins.horizontal;
        myContentArea.y+= myMargins.top;
        myContentArea.height-= myMargins.vertical;
		if(Math3D.IsSmallerOrEqual(myContentArea.width, 0f) || Math3D.IsSmallerOrEqual(myContentArea.height, 0f)) {
			myContentArea.x= myFrameArea.x;
			myContentArea.y= myFrameArea.y;
			myContentArea.width= 0f;
			myContentArea.height= 0f;
		}
        ViewAreaDidChange();    
    }
    
    // ======================================================================
    // Notification methods.
    // ----------------------------------------------------------------------
    protected virtual void ViewAreaDidChange() {}
}
