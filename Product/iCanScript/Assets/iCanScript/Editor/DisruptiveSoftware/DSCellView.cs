using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Internal.Editor {

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
    	DSView							mySubview                 = null;
       
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
    	public DSView Subview {
    		get { return mySubview; }
    		set { mySubview= value; }
    	}
        
        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public DSCellView(RectOffset margins, bool shouldDisplayFrame,
                          Action<DSCellView,Rect> displayDelegate= null,
                          Func<DSCellView,Rect,Vector2> getSizeToDisplayDelegate= null) {
            Margins                 = margins;
            ShouldDisplayFrame      = shouldDisplayFrame;
            DisplayDelegate         = displayDelegate;
            GetSizeToDisplayDelegate= getSizeToDisplayDelegate;
        }
        public DSCellView(RectOffset margins, bool shouldDisplayFrame, DSView subview)
    	: this(margins, shouldDisplayFrame, (v,f)=> subview.Display(f), (v,f)=> subview.GetSizeToDisplay(f)) {
    		mySubview= subview;
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
            myDisplayArea= FrameToDisplayArea(frameArea, Margins);
    
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
        public override Vector2 GetSizeToDisplay(Rect frameArea) {
            Rect availableArea= FrameToDisplayArea(frameArea, Margins);
            return MarginsSize+InvokeGetSizeToDisplayDelegate(availableArea);
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
    		return PerformAlignment(DisplayArea, displaySize, myAnchor);
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
    		mySubview= subview;
            myDisplayDelegate         = (v,f)=> subview.Display(f);
            myGetSizeToDisplayDelegate= (v,f)=> subview.GetSizeToDisplay(f);        
        }
        public bool RemoveSubview() {
    		mySubview= null;
            myDisplayDelegate         = null;
            myGetSizeToDisplayDelegate= null;
            return true;
        }    
    
    	// ======================================================================
        // Utilities.
        // ----------------------------------------------------------------------
        public static Rect FrameToDisplayArea(Rect frameArea, RectOffset margins) {
            Rect displayArea= frameArea;
            displayArea.x+= margins.left;
            displayArea.y+= margins.top;
            displayArea.width-= margins.horizontal; if(displayArea.width < 0) displayArea.width= 0;
            displayArea.height-= margins.vertical; if(displayArea.height < 0) displayArea.height= 0;
            return displayArea;
        }
        public static Rect DisplayToFrameArea(Rect displayArea, RectOffset margins) {
            Rect frameArea= displayArea;
            frameArea.x-= margins.left;
            frameArea.y-= margins.right;
            frameArea.width+= margins.horizontal;
            frameArea.height+= margins.vertical;
            return frameArea;
        }
    	public static Rect PerformAlignment(Rect displayArea, Vector2 displaySize, AnchorEnum alignment) {
            if(Math3D.IsZero(displaySize)) return displayArea;
        	float x= displayArea.x;
        	float y= displayArea.y;
        	float width= displayArea.width;
        	float height= displayArea.height;
        	if(displaySize.x < displayArea.width) {
        		width= displaySize.x;
        		switch(alignment) {
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
        		switch(alignment) {
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
}