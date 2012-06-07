using UnityEngine;
using System;
using System.Collections;

public class DSScrollView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Vector2                         myScrollPosition          = Vector2.zero;
    Vector2                         myContentSize             = Vector2.zero;
    DSCellView                      myMainView                = null;
    bool                            myUseFullWidth            = true;
    bool                            myUseFullHeight           = true;
 	Action<DSScrollView,Rect>       myDisplayDelegate         = null;
	Func<DSScrollView,Rect,Vector2> myGetSizeToDisplayDelegate= null;
   
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	Rect ContentArea { get { return new Rect(0,0,myContentSize.x,myContentSize.y); }}
	public Action<DSScrollView,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSScrollView,Rect,Vector2> GetSizeToDisplayDelegate {
	    get { return myGetSizeToDisplayDelegate; }
	    set { myGetSizeToDisplayDelegate= value; }
	}
	
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSScrollView(RectOffset margins, bool shouldDisplayFrame, bool useFullWidth= true, bool useFullHeight= true,
                        Action<DSScrollView,Rect> displayDelegate= null,
                        Func<DSScrollView,Rect,Vector2> getSizeToDisplayDelegate= null) {
        myUseFullWidth= useFullWidth;
        myUseFullHeight= useFullHeight;
        myMainView= new DSCellView(margins, shouldDisplayFrame, MainViewDisplay, MainViewGetSizeToDisplay);
        myDisplayDelegate= displayDelegate;
        myGetSizeToDisplayDelegate= getSizeToDisplayDelegate;
    }
	public DSScrollView(RectOffset margins, bool shouldDisplayFrame, bool useFullWidth, bool useFullHeight, DSView subview)
	: this(margins, shouldDisplayFrame, useFullWidth, useFullHeight, (v,f)=> subview.Display(f), (v,f)=> subview.GetSizeToDisplay(f)) {}
		
    // ======================================================================
    // DSView implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) {
        myMainView.Display(frameArea);
    }
    public override Vector2 GetSizeToDisplay(Rect frameArea) {
        return myMainView.GetSizeToDisplay(frameArea);
    }
    public override AnchorEnum GetAnchor() {
        return myMainView.Anchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myMainView.Anchor= anchor;
    }

    // ======================================================================
    // Service.
    // ----------------------------------------------------------------------
    public void MakeVisible(Rect area, Rect displayArea) {
        // Calculate visible area.
        Rect visibleArea= new Rect(myScrollPosition.x, myScrollPosition.y, displayArea.width, displayArea.height);
        if(IsHorizontalScrollbarVisble(displayArea)) visibleArea.height-= kScrollbarSize;
        float topY= area.y;
        float bottomY= area.yMax;
        bool isTopVisible= topY > visibleArea.y && topY < visibleArea.yMax;
        bool isBottomVisible= bottomY > visibleArea.y && bottomY < visibleArea.yMax;
        if(isTopVisible) {
            if(isBottomVisible) return;
            float moveOffset= bottomY-visibleArea.yMax;
            myScrollPosition.y+= moveOffset;
            return;
        }
        if(isBottomVisible) {
            myScrollPosition.y= topY;
            return;
        }
        if(Mathf.Abs(topY-visibleArea.y) < Mathf.Abs(topY-visibleArea.yMax)) {
            myScrollPosition.y= topY;
        } else {
            myScrollPosition.y+= bottomY-visibleArea.yMax;
        }
    }
    public bool IsHorizontalScrollbarVisble(Rect displayArea) {
        return myContentSize.x > displayArea.width;
    }
    public bool IsVerticalScrollbarVisible(Rect displayArea) {
        return myContentSize.y > displayArea.height;
    }
    
    // ======================================================================
    // MainView implementation.
    // ----------------------------------------------------------------------
    void MainViewDisplay(DSCellView view, Rect displayArea) {
        myScrollPosition= GUI.BeginScrollView(displayArea, myScrollPosition, ContentArea, false, false);
            InvokeDisplayDelegate(ContentArea);
        GUI.EndScrollView();
    }
    Vector2 MainViewGetSizeToDisplay(DSCellView view, Rect displayArea) {
		myContentSize= InvokeGetSizeToDisplayDelegate(displayArea);
		if(myUseFullWidth && myContentSize.x < displayArea.width) {
		    myContentSize.x= displayArea.width;
		}
		if(myUseFullHeight && myContentSize.y < displayArea.height) {
		    myContentSize.y= displayArea.height;
		}
        // Add scroller if the needed display size exceeds the display area.
        /*
            FIXME: The horizontal scroller shows up everytime the vertical scroller shows up even when not nedded.
        */
		var contentSize= myContentSize;        
        if(myContentSize.x > displayArea.width) {
            contentSize.y+= kScrollbarSize;
        }
        if(myContentSize.y >= displayArea.height) contentSize.x+= kScrollbarSize;
        return contentSize;
    }
    
    // ======================================================================
    // Delegates.
    // ----------------------------------------------------------------------
    protected void InvokeDisplayDelegate(Rect displayArea) {
    	if(myDisplayDelegate != null) myDisplayDelegate(this, displayArea);        
    }
    protected Vector2 InvokeGetSizeToDisplayDelegate(Rect displayArea) {
    	return myGetSizeToDisplayDelegate != null ? myGetSizeToDisplayDelegate(this, displayArea) : Vector2.zero;        
    }

	// ======================================================================
    // Subview management
    // ----------------------------------------------------------------------
    public void SetSubview(DSView subview) {
        myDisplayDelegate         = (v,f)=> subview.Display(f);
        myGetSizeToDisplayDelegate= (v,f)=> subview.GetSizeToDisplay(f);        
    }
    public bool RemoveSubview() {
        myDisplayDelegate         = null;
        myGetSizeToDisplayDelegate= null;
        return true;
    }    
}
