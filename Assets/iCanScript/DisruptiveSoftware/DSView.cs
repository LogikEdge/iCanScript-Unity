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
        set { myFrameArea= value; UpdateClientArea(); }
    }
    public Rect ContentArea {
        get { return myContentArea; }
    }
    public List<DSView> Subviews {
        get { return mySubviews; }
    }
    public RectOffset Margins {
        get { return myMargins; }
        set { myMargins= value; UpdateClientArea(); }
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
    
    // ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public void AddSubview(DSView subview) {
        mySubviews.Add(subview);
    }
    public bool RemoveSubview(DSView subview) {
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
    void UpdateClientArea() {
        myContentArea= myFrameArea;
        myContentArea.x+= myMargins.left;
        myContentArea.width-= myMargins.horizontal;
        myContentArea.y+= myMargins.top;
        myContentArea.height-= myMargins.vertical;
        ViewAreaDidChange();    
    }
    
    // ======================================================================
    // Notification methods.
    // ----------------------------------------------------------------------
    protected virtual void ViewAreaDidChange() {}
}
