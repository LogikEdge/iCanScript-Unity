using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSView {
    // ======================================================================
    // Types
    // ----------------------------------------------------------------------
	public enum AnchorEnum { TopLeft, TopCenter, TopRight,
		                     CenterLeft, Center, CenterRight,
						     BottomLeft, BottomCenter, BottomRight };
	
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect                    myFrameArea             = new Rect(0,0,0,0);        // Total area to use for display.
    bool                    myShouldDisplayFrame    = true;                     // A frame box is displayed when set to true.
    GUIStyle                myFrameGUIStyle         = null;                     // The style used for the frame box.
    RectOffset              myMargins               = new RectOffset(0,0,0,0);  // Content margins.
    Rect                    myDisplayArea           = new Rect(0,0,0,0);
	AnchorEnum	            myAnchor			    = AnchorEnum.TopLeft;		// Frame anchor position.
 	Action<DSView,Rect>     myDisplayDelegate       = null;
    Action<DSView,Rect>     myOnViewChangeDelegate  = null;
	Func<DSView,Vector2>	myGetDisplaySizeDelegate= null;
   
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public bool ShouldDisplayFrame {
        get { return myShouldDisplayFrame; }
        set { myShouldDisplayFrame= value; }
    }
    public GUIStyle FrameGUIStyle {
        get { return myFrameGUIStyle; }
        set { myFrameGUIStyle= value; }
    }
    public RectOffset Margins {
        get { return myMargins; }
        set { myMargins= value; }
    }
	public Vector2 MarginsSize {
	    get { return new Vector2(Margins.horizontal, Margins.vertical); }
	} 
    public AnchorEnum Anchor {
		get { return myAnchor; }
		set { myAnchor= value; }
	}
	public Action<DSView,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSView,Vector2> GetDisplaySizeDelegate {
	    get { return myGetDisplaySizeDelegate; }
	    set { myGetDisplaySizeDelegate= value; }
	}
	public Action<DSView,Rect> OnViewChangeDelegate {
	    get { return myOnViewChangeDelegate; }
	    set { myOnViewChangeDelegate= value; }
	}
	public Rect DisplayArea {
	    get { return myDisplayArea; }
	}
    public Rect FrameArea {
        get { return myFrameArea; }
    }
    
    // ======================================================================
    // Common view constants
    // ----------------------------------------------------------------------
    public const float   kHorizontalSpacer= 8f;
    public const float   kVerticalSpacer  = 8f;
    public const float   kHorizontalMargin= 10f;
    public const float   kVerticalMargin  = 10f;
    public const float   kScrollerSize    = 16f;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSView(RectOffset margins, bool shouldDisplayFrame= true,
                  Action<DSView,Rect> displayDelegate= null,
                  Action<DSView,Rect> onViewChangeDelegate= null,
                  Func<DSView,Vector2> getDisplaySizeDelegate= null) {
        Margins               = margins;
        ShouldDisplayFrame    = shouldDisplayFrame;
        DisplayDelegate       = displayDelegate;
        OnViewChangeDelegate  = onViewChangeDelegate;
        GetDisplaySizeDelegate= getDisplaySizeDelegate;
    }
    
	// ======================================================================
    // Base functionality.
    // ----------------------------------------------------------------------
	public void Display(Rect frameArea) { Display(null, frameArea); }
    protected virtual void Display(DSView parent, Rect frameArea) {
        // Don't display if display area is smaller then margins.
        if(frameArea.width < Margins.horizontal) return;
        if(frameArea.height < Margins.vertical) return;
        
        // Recompute display area.
        myFrameArea= frameArea;
        Rect displayArea= new Rect(frameArea.x+Margins.left,
                                   frameArea.y+Margins.top,
                                   frameArea.width-Margins.horizontal,
                                   frameArea.height-Margins.vertical);
        if(Math3D.IsNotEqual(displayArea, myDisplayArea)) {
			OnViewChange(parent,displayArea);            
        }

        // Display frame and content.
        DisplayFrame();
		InvokeDisplayDelegate(ComputeDisplayRect(parent));
    }
    protected virtual void OnViewChange(DSView parent, Rect displayArea) {
        myDisplayArea= displayArea;
        InvokeDisplayDelegate(displayArea);
    }
    protected virtual Vector2 GetDisplaySize(DSView parent) {
        return InvokeGetDisplaySizeDelegate();
    }
    
    // ======================================================================
    // Subview management.
    // ----------------------------------------------------------------------
	public virtual void AddSubview(DSView subview) {
	    DisplayDelegate       += subview.Display;
	    OnViewChangeDelegate  += subview.OnViewChange;
	    GetDisplaySizeDelegate+= subview.GetDisplaySize;
	}
	public virtual bool RemoveSubview(DSView subview) {
	    DisplayDelegate       -= subview.Display;
	    OnViewChangeDelegate  -= subview.OnViewChange;
	    GetDisplaySizeDelegate-= subview.GetDisplaySize;
        return true;
	}
	
    // ======================================================================
    // Deleagate functionality.
    // ----------------------------------------------------------------------
	protected Vector2 InvokeGetDisplaySizeDelegate() {
		return myGetDisplaySizeDelegate != null ? myGetDisplaySizeDelegate(this) : Vector2.zero;
	}
	protected void InvokeDisplayDelegate(Rect area) {
		if(myDisplayDelegate != null) myDisplayDelegate(this, area);
	}
	protected void InvokeOnViewChangeDelegate(Rect area) {
	    if(myOnViewChangeDelegate != null) myOnViewChangeDelegate(this, area);
	}

    // ======================================================================
    // Frame alignment.
    // ----------------------------------------------------------------------
	void DisplayFrame() {
        if(myShouldDisplayFrame == false || myFrameArea.width <= 0 || myFrameArea.height <= 0) return;
        if(myFrameGUIStyle != null) {
            GUI.Box(myFrameArea,"", myFrameGUIStyle);
        } else {
            GUI.Box(myFrameArea,"");                    
        }		
	}
    // ----------------------------------------------------------------------
    protected Rect ComputeDisplayRect(DSView parent) {
		Vector2 displaySize= GetDisplaySize(parent);
        if(Math3D.IsZero(displaySize)) return DisplayArea;
		Rect displayArea= DisplayArea;
		float x= displayArea.x;
		float y= displayArea.y;
		float width= displayArea.width;
		float height= displayArea.height;
		if(displaySize.x < displayArea.width) {
			width= displaySize.x;
			switch(myAnchor) {
				case AnchorEnum.Center:
				case AnchorEnum.TopCenter:
				case AnchorEnum.BottomCenter: {
					x+= 0.5f*(displayArea.width - displaySize.x);
					break;
				}
				case AnchorEnum.TopRight:
				case AnchorEnum.CenterRight:
				case AnchorEnum.BottomRight: {
					x= displayArea.xMax-displaySize.x;
					break;
				}
			}
		}
		if(displaySize.y < displayArea.height) {
			height= displaySize.y;
			switch(myAnchor) {
				case AnchorEnum.Center:
				case AnchorEnum.CenterRight:
				case AnchorEnum.CenterLeft: {
					y+= 0.5f*(displayArea.height - displaySize.y);
					break;
				}
				case AnchorEnum.BottomRight:
				case AnchorEnum.BottomCenter:
				case AnchorEnum.BottomLeft: {
					y= displayArea.yMax-displaySize.y;
					break;
				}
			}
		}
		return new Rect(x,y,width,height);	
	}
}
