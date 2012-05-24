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
    public DSScrollView(RectOffset margins, bool shouldDisplayFrame, bool useFullWidth= true,
                        Action<DSScrollView,Rect> displayDelegate= null,
                        Func<DSScrollView,Rect,Vector2> getSizeToDisplayDelegate= null) {
        myUseFullWidth= useFullWidth;
        myMainView= new DSCellView(margins, shouldDisplayFrame, MainViewDisplay, MainViewGetSizeToDisplay);
        myDisplayDelegate= displayDelegate;
        myGetSizeToDisplayDelegate= getSizeToDisplayDelegate;
    }
	public DSScrollView(RectOffset margins, bool shouldDisplayFrame, bool useFullWidth, DSView subview)
	: this(margins, shouldDisplayFrame, useFullWidth, (v,f)=> subview.Display(f), (v,f)=> subview.GetSizeToDisplay(f)) {}
		
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
        // Add scroller if the needed display size exceeds the display area.
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
