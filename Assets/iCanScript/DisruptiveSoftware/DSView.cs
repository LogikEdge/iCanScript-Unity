using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect            myViewArea;
    List<DSView>    mySubviews= new List<DSView>();

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Rect ViewArea {
        get { return myViewArea; }
        set { myViewArea= value; }
    }
    public List<DSView> Subviews {
        get { return mySubviews; }
    }

    // ======================================================================
    // Common view constants
    // ----------------------------------------------------------------------
    const float   kHorizontalSpacer= 8f;
    const float   kVerticalSpacer  = 8f;
    const float   kHorizontalMargin= 10f;
    const float   kVerticalMargin  = 10f;

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
    // Display Utility
    // ----------------------------------------------------------------------
    public Vector2  Center              { get { return Math3D.Middle(ViewArea); }}    
    public float    HorizontalCenter    { get { return 0.5f*(ViewArea.x+ViewArea.xMax); }}
    public float    VerticalCenter      { get { return 0.5f*(ViewArea.y+ViewArea.yMax); }}
    public float    CenterHorizontaly(float width) {
        return ViewArea.x+0.5f*(ViewArea.width-width);
    }
    public float    CenterVerticalyCenter(float height) {
        return ViewArea.y+0.5f*(ViewArea.height-height);
    }
}
