using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public class DSTitleView : DSView {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        GUIContent                      myTitle                   = null;
        bool                            myTitleSeperator          = false;
        GUIStyle                        myTitleGUIStyle           = null;
        GUIStyle                        mySeperatorGUIStyle       = null;
        Vector2                         myTitleSize               = Vector2.zero;
        DSCellView                      myMainView                = null;
        DSCellView                      myTitleSubview            = null;
        Action<DSTitleView,Rect>        myDisplayDelegate         = null;
    	Func<DSTitleView,Rect,Vector2>  myGetSizeToDisplayDelegate= null;
    	Rect                            myTitleArea;
 	
        // ======================================================================
        // Properties
        // ----------------------------------------------------------------------
        public GUIContent Title {
            get { return myTitle; }
            set {
                myTitle= value;
                if(myTitle != null) {
                    myTitleSize= TitleGUIStyle.CalcSize(myTitle);                
                } else {
                    myTitleSize= Vector2.zero;
                }
            }
        }
        public bool TitleSeperator {
            get { return myTitleSeperator; }
            set { myTitleSeperator= value; }
        }
        public AnchorEnum TitleAlignment {
            get { return myTitleSubview.Anchor; }
            set { myTitleSubview.Anchor= value; }
        }
        public GUIStyle TitleGUIStyle {
            get { return myTitleGUIStyle ?? EditorStyles.boldLabel; }
            set { myTitleGUIStyle= value; myTitleSize= TitleGUIStyle.CalcSize(myTitle); }
        }
        public GUIStyle SeperatorGUIStyle {
            get { return mySeperatorGUIStyle; }
            set { mySeperatorGUIStyle= value; }
        }
    
        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public DSTitleView(RectOffset margins, bool shouldDisplayFrame,
                           GUIContent title, AnchorEnum titleAlignment, bool titleSeperator,
                           Action<DSTitleView,Rect> displayDelegate= null,
                           Func<DSTitleView,Rect,Vector2> getSizeToDisplayDelegate= null) {
            // Create subviews
            myMainView    = new DSCellView(margins, shouldDisplayFrame, MainViewDisplay, MainViewGetSizeToDisplay);
            myTitleSubview= new DSCellView(new RectOffset(0,0,0,0), false, TitleViewDisplay, TitleViewGetSizeToDisplay);
            // initialize title related information.
            Title                     = title;
            TitleAlignment            = titleAlignment;
            TitleSeperator            = titleSeperator;
            myDisplayDelegate         = displayDelegate;
            myGetSizeToDisplayDelegate= getSizeToDisplayDelegate;        
        }
    	public DSTitleView(RectOffset margins, bool shouldDisplayFrame,
    					   GUIContent title, AnchorEnum titleAlignment, bool titleSeperator,
    					   DSView subview)
    	: this(margins, shouldDisplayFrame, title, titleAlignment, titleSeperator,
    		   (v,f)=> { subview.Display(f); },
    		   (v,f)=> { return subview.GetSizeToDisplay(f); }) {}
	
        // ======================================================================
        // DSView functionality implementation.
        // ----------------------------------------------------------------------
        public override void Display(Rect frameArea) { 
            myMainView.Display(frameArea);
        }
        public override Vector2 GetSizeToDisplay(Rect displayArea) {
            return myMainView.GetSizeToDisplay(displayArea);
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
            float titleHeight= TitleAreaHeight();
            float bodyHeight= displayArea.height-titleHeight;
            if(titleHeight >= displayArea.height) {
                titleHeight= displayArea.height;
                bodyHeight= 0;
            }
            myTitleArea= new Rect(displayArea.x, displayArea.y, displayArea.width, titleHeight);
            myTitleSubview.Display(myTitleArea);
            InvokeDisplayDelegate(new Rect(displayArea.x, displayArea.y+titleHeight, displayArea.width, bodyHeight));
        }
        Vector2 MainViewGetSizeToDisplay(DSCellView view, Rect displayArea) {
            // Adjust display area to remove title area.
            float titleAreaHeight= TitleAreaHeight();
            displayArea.y+= titleAreaHeight;
            displayArea.height-= titleAreaHeight; if(displayArea.height < 0) displayArea.height= 0;
            // Now compute using content size.
            var contentSize= InvokeGetSizeToDisplayDelegate(displayArea);
            float width= Mathf.Max(contentSize.x, myTitleSize.x);
            float height= contentSize.y+titleAreaHeight;
            return new Vector2(width, height);
        }
    
        // ======================================================================
        // TitleSubview implementation.
        // ----------------------------------------------------------------------
        void TitleViewDisplay(DSCellView view, Rect displayArea) {
            // Just return if we have nothing to display.
            if(myTitle == null || Math3D.IsZero(myTitleSize.x)) return;
            // Display title.
            GUI.Label(displayArea, Title, TitleGUIStyle);
            // Display title separator.
            if(!myTitleSeperator) return;
            float y= myTitleArea.y+myTitleSize.y;
            Rect seperatorRect= new Rect(myTitleArea.x, y, myTitleArea.width, 3f);
            if(mySeperatorGUIStyle != null) {
                GUI.Box(seperatorRect, "", mySeperatorGUIStyle);
            } else {
                GUI.Box(seperatorRect, "");            
            }
        }
        Vector2 TitleViewGetSizeToDisplay(DSCellView view, Rect displayArea) {
            return new Vector2(myTitleSize.x, TitleAreaHeight());
        }

        // ======================================================================
        // View area management.
        // ----------------------------------------------------------------------
        float TitleAreaHeight() {
            return myTitleSize.y+(myTitleSeperator ? 3f : 0);
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

}
