using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DSCellView : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    Rect                            myFrameArea               = new Rect(0,0,0,0);        // Total area to use for display.
    bool                            myShouldDisplayFrame      = true;                     // A frame box is displayed when set to true.
    GUIStyle                        myFrameGUIStyle           = null;                     // The style used for the frame box.
    RectOffset                      myMargins                 = new RectOffset(0,0,0,0);  // Content margins.
    Rect                            myDisplayArea             = new Rect(0,0,0,0);
    AnchorEnum                      myAnchor			      = AnchorEnum.TopLeft;		// Frame anchor position.
 	Action<DSCellView,Rect>         myDisplayDelegate         = null;
	Func<DSCellView,Rect,Vector2>   myGetSizeToDisplayDelegate= null;
   
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public Rect DisplayArea {
        get { return myDisplayArea; }
    }
    public Rect FrameArea {
        get { return myFrameArea; }
    }
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
	public Action<DSCellView,Rect> DisplayDelegate {
	    get { return myDisplayDelegate; }
	    set { myDisplayDelegate= value; }
	}
	public Func<DSCellView,Rect,Vector2> GetSizeToDisplayDelegate {
	    get { return myGetSizeToDisplayDelegate; }
	    set { myGetSizeToDisplayDelegate= value; }
	}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSCellView(RectOffset margins, bool shouldDisplayFrame= true,
                      Action<DSCellView,Rect> displayDelegate= null,
                      Func<DSCellView,Rect,Vector2> getSizeToDisplayDelegate= null) {
        Margins                 = margins;
        ShouldDisplayFrame      = shouldDisplayFrame;
        DisplayDelegate         = displayDelegate;
        GetSizeToDisplayDelegate= getSizeToDisplayDelegate;
    }
    
    // ======================================================================
    // DSView functionality implementation.
    // ----------------------------------------------------------------------
    public override void Display(Rect frameArea) { 
        // Don't display if display area is smaller then margins.
        if(frameArea.width < Margins.horizontal) return;
        if(frameArea.height < Margins.vertical) return;

        // Compute available display area.
        myFrameArea= frameArea;
        Rect displayArea= new Rect(frameArea.x+Margins.left,
                                   frameArea.y+Margins.top,
                                   frameArea.width-Margins.horizontal,
                                   frameArea.height-Margins.vertical);
        
        if(Math3D.IsNotEqual(displayArea, myDisplayArea)) {
            myDisplayArea= displayArea;            
        }

        // Compute needed display area.
        Rect anchoredDisplayArea= ComputeAnchoredDisplayArea();

        // Display frame border around anchored display area.
        frameArea.x= anchoredDisplayArea.x-Margins.left;
        frameArea.y= anchoredDisplayArea.y-Margins.top;
        frameArea.xMax= anchoredDisplayArea.xMax+Margins.right;
        frameArea.yMax= anchoredDisplayArea.yMax+Margins.bottom;
        DisplayFrame(frameArea);
        InvokeDisplayDelegate(anchoredDisplayArea);
    }
    public override Vector2 GetSizeToDisplay(Rect displayArea) {
        return MarginsSize+InvokeGetSizeToDisplayDelegate(displayArea);
    }
    public override AnchorEnum GetAnchor() {
        return myAnchor;
    }
    public override void SetAnchor(AnchorEnum anchor) {
        myAnchor= anchor;
    }

    // ======================================================================
    // DSViewCell display functionality.
    // ----------------------------------------------------------------------
    void DisplayFrame(Rect frameArea) {
        if(myShouldDisplayFrame == false || frameArea.width <= 0 || frameArea.height <= 0) return;
        if(myFrameGUIStyle != null) {
            GUI.Box(frameArea,"", myFrameGUIStyle);
        } else {
            GUI.Box(frameArea,"");                    
        }		
    }
    // ----------------------------------------------------------------------
    protected Rect ComputeAnchoredDisplayArea() {
    	Vector2 displaySize= InvokeGetSizeToDisplayDelegate(DisplayArea);
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

	// ======================================================================
    // Delegates.
    // ----------------------------------------------------------------------
    protected void InvokeDisplayDelegate(Rect displayArea) {
		if(myDisplayDelegate != null) myDisplayDelegate(this, displayArea);        
    }
    protected Vector2 InvokeGetSizeToDisplayDelegate(Rect displayArea) {
		return myGetSizeToDisplayDelegate != null ? myGetSizeToDisplayDelegate(this, displayArea) : Vector2.zero;        
    }
}
