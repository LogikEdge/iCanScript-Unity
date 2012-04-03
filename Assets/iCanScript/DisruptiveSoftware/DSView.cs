using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect            myFrameArea         = new Rect(0,0,0,0);        // Total area to use for display.
    RectOffset      myMargins           = new RectOffset(0,0,0,0);  // Content margins.
    bool            myShouldDisplayFrame= true;                     // A frame box is displayed when set to true.
    GUIStyle        myFrameGUIStyle     = null;                     // The style used for the frame box.
    List<DSView>    mySubviews          = new List<DSView>();       // All configured subviews.
    Rect            myDisplayArea       = new Rect(0,0,0,0);        // Display area for the content.
    Vector2         myDisplayRatio      = Vector2.zero;             // Desired display ratio (automatique if zero)

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Rect FrameArea {
        get { return myFrameArea; }
        set { myFrameArea= value; UpdateDisplayArea(); }
    }
    public RectOffset Margins {
        get { return myMargins; }
        set { myMargins= value; UpdateDisplayArea(); }
    }
    public Rect    DisplayArea           { get { return myDisplayArea; }}
    public bool    HasHorizontalScroller { get { return GetHasHorizontalScroller(); }}
    public bool    HasVerticalScroller   { get { return GetHasVerticalScroller(); }}
    public Vector2 MinimumFrameSize      { get { return GetMinimumFrameSize(); }}
    public Vector2 FullFrameSize         { get { return GetFullFrameSize(); }}
    public Vector2 FullFrameSizeWithScrollers {
        get {
            var result= FullFrameSize;
            if(GetHasHorizontalScroller()) {
                result.y+= kScrollerSize;
            }
            if(GetHasVerticalScroller()) {
                result.x+= kScrollerSize;
            }
            return result;
        }
    }
    public bool ShouldDisplayFrame {
        get { return myShouldDisplayFrame; }
        set { myShouldDisplayFrame= value; }
    }
    public GUIStyle FrameGUIStyle {
        get { return myFrameGUIStyle; }
        set { myFrameGUIStyle= value; }
    }
    public List<DSView> Subviews { get { return mySubviews; }}
    public Vector2 DisplayRatio {
        get { return myDisplayRatio; }
        set { myDisplayRatio= value; OnViewAreaChange(); }
    }
    
    // ======================================================================
    // Common view constants
    // ----------------------------------------------------------------------
    public const float   kHorizontalSpacer= 8f;
    public const float   kVerticalSpacer  = 8f;
    public const float   kHorizontalMargin= 10f;
    public const float   kVerticalMargin  = 10f;
    public const float   kScrollerSize = 16f;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSView(RectOffset margins, bool shouldDisplayFrame= true) {
        Margins= margins;
        ShouldDisplayFrame= shouldDisplayFrame;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public virtual void Display(Rect frameArea) { FrameArea= frameArea; Display(); }
    public virtual void Display() {
        if(myShouldDisplayFrame == false || FrameArea.width <= 0 || FrameArea.height <= 0) return;
        if(myFrameGUIStyle != null) {
            GUI.Box(FrameArea,"", myFrameGUIStyle);
        } else {
            GUI.Box(FrameArea,"");                    
        }
    }
    public virtual void ReloadData() {}
    
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
    // Methods to override to customize view behaviour.
    // ----------------------------------------------------------------------
    protected virtual void      OnViewAreaChange()          {}
    protected virtual Vector2   GetMinimumFrameSize()       { return new Vector2(Margins.horizontal, Margins.vertical); }
    protected virtual Vector2   GetFullFrameSize()          { return GetMinimumFrameSize(); }
    protected virtual bool      GetHasHorizontalScroller()  { return false; }
    protected virtual bool      GetHasVerticalScroller()    { return false; }

    // ======================================================================
    // View area management.
    // ----------------------------------------------------------------------
    void UpdateDisplayArea() {
        myDisplayArea= myFrameArea;
        myDisplayArea.x+= myMargins.left;
        myDisplayArea.width-= myMargins.horizontal;
        myDisplayArea.y+= myMargins.top;
        myDisplayArea.height-= myMargins.vertical;
		if(Math3D.IsSmallerOrEqual(myDisplayArea.width, 0f) || Math3D.IsSmallerOrEqual(myDisplayArea.height, 0f)) {
			myDisplayArea.x= myFrameArea.x;
			myDisplayArea.y= myFrameArea.y;
			myDisplayArea.width= 0f;
			myDisplayArea.height= 0f;
		}
        OnViewAreaChange();    
    }
}
