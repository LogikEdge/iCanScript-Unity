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
    RectOffset              myMargins               = new RectOffset(0,0,0,0);  // Content margins.
    bool                    myShouldDisplayFrame    = true;                     // A frame box is displayed when set to true.
    GUIStyle                myFrameGUIStyle         = null;                     // The style used for the frame box.
	AnchorEnum	            myAnchor			    = AnchorEnum.Center;		// Frame anchor position.
	Func<DSView,Vector2>    myGetContentSizeDelegate= null;
	Action<DSView,Rect>	    myDisplayDelegate       = null;
    Action<DSView>          myOnViewChangeDelegate  = null;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Rect FrameArea {
        get { return myFrameArea; }
        set { myFrameArea= value; ProcessFrameAreaChange(); }
    }
    public RectOffset Margins {
        get { return myMargins; }
        set { myMargins= value; ProcessFrameAreaChange(); }
    }
    public Rect DisplayArea {
        get {
            return new Rect(FrameArea.x+Margins.left, FrameArea.y+Margins.top,
                            FrameArea.width-Margins.horizontal, FrameArea.height-Margins.vertical);
        }
    }
	public Vector2 MarginsSize {
	    get { return new Vector2(Margins.horizontal, Margins.vertical); }
	} 
    public bool ShouldDisplayFrame {
        get { return myShouldDisplayFrame; }
        set { myShouldDisplayFrame= value; }
    }
    public GUIStyle FrameGUIStyle {
        get { return myFrameGUIStyle; }
        set { myFrameGUIStyle= value; }
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
	public Action<DSView> OnViewChangeDelegate {
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
                  Func<DSView,Vector2> getContentSizeDelegate= null,
                  Action<DSView> onViewChangeDelegate= null) {
        Margins               = margins;
        ShouldDisplayFrame    = shouldDisplayFrame;
        DisplayDelegate       = displayDelegate;
        GetContentSizeDelegate= getContentSizeDelegate;
        OnViewChangeDelegate  = onViewChangeDelegate;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public void Display(Rect frameArea) { FrameArea= frameArea; Display(); }
    public void Display() {
		DisplayFrame();
		Rect displayArea= DisplayArea;
		float x= displayArea.x;
		float y= displayArea.y;
		float width= displayArea.width;
		float height= displayArea.height;
		Vector2 contentSize= GetContentSize();
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
		DisplayContent(new Rect(x,y,width,height));
    }
    
    // ======================================================================
    // View area management.
    // ----------------------------------------------------------------------
    void ProcessFrameAreaChange() {
        if(myFrameArea.width < MarginsSize.x)  myFrameArea.width = MarginsSize.x;
        if(myFrameArea.height < MarginsSize.y) myFrameArea.height= MarginsSize.y;
        OnViewChange();    
    }
	void DisplayFrame() {
        if(myShouldDisplayFrame == false || FrameArea.width <= 0 || FrameArea.height <= 0) return;
        if(myFrameGUIStyle != null) {
            GUI.Box(FrameArea,"", myFrameGUIStyle);
        } else {
            GUI.Box(FrameArea,"");                    
        }		
	}

    // ======================================================================
    // Delegates.
    // ----------------------------------------------------------------------
	Vector2 GetContentSize() {
		return myGetContentSizeDelegate != null ? myGetContentSizeDelegate(this) : Vector2.zero;
	}
	void DisplayContent(Rect area) {
		if(myDisplayDelegate != null) myDisplayDelegate(this, area);
	}
	void OnViewChange() {
	    if(myOnViewChangeDelegate != null) myOnViewChangeDelegate(this);
	}
}
