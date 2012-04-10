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
	Func<DSView,Vector2>	myGetContentSizeDelegate= null;
   
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
        set { myMargins= value; ProcessFrameAreaChange(); }
    }
	public Vector2 MarginsSize {
	    get { return new Vector2(Margins.horizontal, Margins.vertical); }
	} 
    public AnchorEnum Anchor {
		get { return myAnchor; }
		set { myAnchor= value; OnViewChange(); }
	}
	public Action<DSView,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSView,Vector2> GetContentSizeDelegate {
	    get { return myGetContentSizeDelegate; }
	    set { myGetContentSizeDelegate= value; }
	}
	public Action<DSView,Rect> OnViewChangeDelegate {
	    get { return myOnViewChangeDelegate; }
	    set { myOnViewChangeDelegate= value; }
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
    public DSView(RectOffset margins, bool shouldDisplayFrame= true,
                  Action<DSView,Rect> displayDelegate= null,
                  Action<DSView> onViewChangeDelegate= null,
                  Func<DSView,Vector2> getContentSizeDelegate= null)
	: base(displayDelegate, getContentSizeDelegate, onViewChangeDelegate) {
        Margins               = margins;
        ShouldDisplayFrame    = shouldDisplayFrame;
        DisplayDelegate       = displayDelegate;
        OnViewChangeDelegate  = onViewChangeDelegate;
        GetContentSizeDelegate= getContentSizeDelegate;
    }
    
	// ======================================================================
    // Base functionality.
    // ----------------------------------------------------------------------
	public void Display(Rect frameArea) { Display(null, frameArea); }
    public virtual void Display(DSView parent, Rect frameArea) {
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
			OnViewChange(displayArea);            
        }

        // Display frame and content.
        DisplayFrame();
		InvokeDisplayDelegate(ComputeDisplayRect());
    }
    public virtual void OnViewChange(DSView parent, Rect displayArea) {
        myDisplayArea= displayArea;
        InvokeDisplayDelegate(displayArea);
    }
    public virtual Vector2 GetContentSize(DSView parent) {
        return InvokeGetContentSizeDelegate();
    }
    
    // ======================================================================
    // Subview management.
    // ----------------------------------------------------------------------
	public void AddSubview(DSView subview) {
	    DisplayDelegate       += subview.Display;
	    OnViewChangeDelegate  += subview.OnViewChange;
	    GetContentSizeDelegate+= subview.GetContentSize;
	}
	public void RemoveSubview(DSView subview) {
	    DisplayDelegate       -= subview.Display;
	    OnViewChangeDelegate  -= subview.OnViewChange;
	    GetContentSizeDelegate-= subview.GetContentSize;
	}
	
    // ======================================================================
    // Deleagate functionality.
    // ----------------------------------------------------------------------
	protected Vector2 InvokeGetContentSizeDelegate() {
		return myGetContentSizeDelegate != null ? myGetContentSizeDelegate(this) : Vector2.zero;
	}
	protected void InvokeDisplayContentDelegate(Rect area) {
		if(myDisplayContentDelegate != null) myDisplayContentDelegate(this, area);
	}
	protected void InvokeOnViewChangeDelegate(Rect area) {
	    if(myOnViewChangeDelegate != null) myOnViewChangeDelegate(this, area);
	}

    // ======================================================================
    // Frame alignment.
    // ----------------------------------------------------------------------
	void DisplayFrame() {
        if(myShouldDisplayFrame == false || FrameArea.width <= 0 || FrameArea.height <= 0) return;
        if(myFrameGUIStyle != null) {
            GUI.Box(FrameArea,"", myFrameGUIStyle);
        } else {
            GUI.Box(FrameArea,"");                    
        }		
	}
    // ----------------------------------------------------------------------
    protected Rect ComputeDisplayRect() {
		Vector2 contentSize= GetContentSize();
        if(Math3D.IsZero(contentSize)) return DisplayArea;
		Rect displayArea= DisplayArea;
		float x= displayArea.x;
		float y= displayArea.y;
		float width= displayArea.width;
		float height= displayArea.height;
		if(contentSize.x < displayArea.width) {
			width= contentSize.x;
			switch(myAnchor) {
				case AnchorEnum.Center:
				case AnchorEnum.TopCenter:
				case AnchorEnum.BottomCenter: {
					x+= 0.5f*(displayArea.width - contentSize.x);
					break;
				}
				case AnchorEnum.TopRight:
				case AnchorEnum.CenterRight:
				case AnchorEnum.BottomRight: {
					x= displayArea.xMax-contentSize.x;
					break;
				}
			}
		}
		if(contentSize.y < displayArea.height) {
			height= contentSize.y;
			switch(myAnchor) {
				case AnchorEnum.Center:
				case AnchorEnum.CenterRight:
				case AnchorEnum.CenterLeft: {
					y+= 0.5f*(displayArea.height - contentSize.y);
					break;
				}
				case AnchorEnum.BottomRight:
				case AnchorEnum.BottomCenter:
				case AnchorEnum.BottomLeft: {
					y= displayArea.yMax-contentSize.y;
					break;
				}
			}
		}
		return new Rect(x,y,width,height);	
	}
}
