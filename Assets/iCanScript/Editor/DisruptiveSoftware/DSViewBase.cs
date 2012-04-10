using UnityEngine;
using System;
using System.Collections;

public class DSViewBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	Action<DSViewBase,Rect>		myDisplayDelegate       = null;
    Action<DSViewBase,Rect>     myOnViewChangeDelegate  = null;
	Func<DSViewBase,Vector2>	myGetContentSizeDelegate= null;
	Rect						myDisplayArea           = new Rect(0,0,0,0);

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public Action<DSViewBase,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSViewBase,Vector2> GetContentSizeDelegate {
	    get { return myGetContentSizeDelegate; }
	    set { myGetContentSizeDelegate= value; }
	}
	public Action<DSViewBase,Rect> OnViewChangeDelegate {
	    get { return myOnViewChangeDelegate; }
	    set { myOnViewChangeDelegate= value; }
	}

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	public DSViewBase(Action<DSViewBase,Rect> displayDelegate= null,
        			  Action<DSViewBase> onViewChangeDelegate= null,
		              Func<DSViewBase,Rect> getDisplaySizeDelegate= null) {
		myDisplayDelegate= displayDelegate;
		myOnViewChangeDelegate= onViewChangeDelegate;
		myGetContentSizeDelegate= getDisplaySizeDelegate;
	}
	
    // ======================================================================
    // Base functionality.
    // ----------------------------------------------------------------------
	public void Display(Rect displayArea) {	Display(null, displayArea); }
    public virtual void Display(DSViewBase parent, Rect displayArea) {
        if(Math3D.IsNotEqual(displayArea, myDisplayArea)) {
			OnViewChange(displayArea);            
        }
		InvokeDisplayDelegate(displayArea);
    }
    public virtual void OnViewChange(DSViewBase parent, Rect displayArea) {
        myDisplayArea= displayArea;
        InvokeDisplayDelegate(displayArea);
    }
    public virtual Vector2 GetContentSize(DSViewBase parent) {
        return InvokeGetContentSizeDelegate();
    }
    
    // ======================================================================
    // Subview management.
    // ----------------------------------------------------------------------
	public void AddSubview(DSViewBase subview) {
	    DisplayDelegate       += subview.Display;
	    OnViewChangeDelegate  += subview.OnViewChange;
	    GetContentSizeDelegate+= subview.GetContentSize;
	}
	public void RemoveSubview(DSViewBase subview) {
	    DisplayDelegate       -= subview.Display;
	    OnViewChangeDelegate  -= subview.OnViewChange;
	    GetContentSizeDelegate-= subview.GetContentSize;
	}
	
    // ======================================================================
    // Deleagate functionality.
    // ----------------------------------------------------------------------
	Vector2 InvokeGetContentSizeDelegate() {
		return myGetContentSizeDelegate != null ? myGetContentSizeDelegate(this) : Vector2.zero;
	}
	void InvokeDisplayContentDelegate(Rect area) {
		if(myDisplayContentDelegate != null) myDisplayContentDelegate(this, area);
	}
	void InvokeOnViewChangeDelegate(Rect area) {
	    if(myOnViewChangeDelegate != null) myOnViewChangeDelegate(this, area);
	}
}
