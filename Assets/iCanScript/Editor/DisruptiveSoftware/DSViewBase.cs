using UnityEngine;
using System;
using System.Collections;

public class DSViewBase {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	Action<DSViewBase,Rect>		myDisplayContentDelegate= null;
    Action<DSViewbase>        	myOnViewChangeDelegate  = null;
	Func<DSViewBase,Vector2>	myGetContentSizeDelegate= null;
	Rect						myDisplayArea           = new Rect(0,0,0,0);

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
	public Action<DSView,Rect> DisplayContentDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSView,Vector2> GetContentSizeDelegate {
	    get { return myGetContentSizeDelegate; }
	    set { myGetContentSizeDelegate= value; }
	}
	public Action<DSView> OnViewChangeDelegate {
	    get { return myOnViewChangeDelegate; }
	    set { myOnViewChangeDelegate= value; }
	}

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
	public DSViewBase(Action<DSViewBase,Rect> displayDelegate= null,
        			  Action<DSViewBase> onViewChangeDelegate= null,
		              Func<DSViewBase,Rect> getDisplaySizeDelegate= null) {
		myDisplayContentDelegate= displayDelegate;
		myOnViewChangeDelegate= onViewChangeDelegate;
		myGetContentSizeDelegate= getDisplaySizeDelegate;
	}
	
    // ======================================================================
    // Base functionality.
    // ----------------------------------------------------------------------
	public virtual void Display(Rect displayArea) {		
		if(Math3D.IsNotEqual(displayArea.width, myDisplayArea.width) ||
		   Math3D.IsNotEqual(displayArea.height, myDisplayArea.height)) {
			myDisplayArea= displayArea;
			OnViewChange();
		}
		DisplayContent(displayArea);
	}
	
    // ======================================================================
    // Deleagate functionality.
    // ----------------------------------------------------------------------
	Vector2 GetContentSize() {
		return myGetContentSizeDelegate != null ? myGetContentSizeDelegate(this) : Vector2.zero;
	}
	void DisplayContent(Rect area) {
		if(myDisplayContentDelegate != null) myDisplayContentDelegate(this, area);
	}
	void OnViewChange() {
	    if(myOnViewChangeDelegate != null) myOnViewChangeDelegate(this);
	}
}
