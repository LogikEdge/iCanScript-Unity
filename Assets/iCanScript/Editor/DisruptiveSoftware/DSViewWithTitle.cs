using UnityEngine;
using UnityEditor;
using System.Collections;

//public abstract class DSViewWithTitle : DSView {
//    // ======================================================================
//    // Fields
//    // ----------------------------------------------------------------------
//    GUIContent      myTitle         = null;
//    TextAlignment   myTitleAlignment= TextAlignment.Center;
//    bool            myTitleSeperator= false;
//    GUIStyle        myTitleGUIStyle = null;
//    Vector2         myTitleSize     = Vector2.zero;
//    Rect            myTitleArea;
//    Rect            myBodyArea;
//    
//    // ======================================================================
//    // Properties
//    // ----------------------------------------------------------------------
//    public GUIContent Title {
//        get { return myTitle; }
//        set {
//            myTitle= value;
//            if(myTitle != null) {
//                myTitleSize= TitleGUIStyle.CalcSize(myTitle);                
//            } else {
//                myTitleSize= Vector2.zero;
//            }
//        }
//    }
//    public bool TitleSeperator {
//        get { return myTitleSeperator; }
//        set { myTitleSeperator= value; }
//    }
//    public TextAlignment TitleAlignment {
//        get { return myTitleAlignment; }
//        set { myTitleAlignment= value; }
//    }
//    public Rect TitleArea   { get { return myTitleArea; }}
//    public Rect BodyArea    { get { return myBodyArea; }}
//    public GUIStyle TitleGUIStyle {
//        get { return myTitleGUIStyle ?? EditorStyles.boldLabel; }
//        set { myTitleGUIStyle= value; myTitleSize= TitleGUIStyle.CalcSize(myTitle); }
//    }
//    
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSViewWithTitle(RectOffset margins, bool shouldDisplayFrame,
//                           GUIContent title, TextAlignment titleAlignment, bool titleSeperator)
//     : base(margins, shouldDisplayFrame) {
//        Title= title;
//        TitleSeperator= titleSeperator;
//        TitleAlignment= titleAlignment;
//    }
//    
//    // ======================================================================
//    // Display
//    // ----------------------------------------------------------------------
//    public void DisplayTitle() {
//        // Just return if we have nothing to display.
//        if(FrameArea.width <= 0 || FrameArea.height <= 0) return;
//        if(myTitle == null || Math3D.IsZero(myTitleSize.x)) return;
//        // Compute title horizontal alignment.
//        Rect titleRect= new Rect(0, TitleArea.y, myTitleSize.x, myTitleSize.y);
//        switch(TitleAlignment) {
//            case TextAlignment.Left:  { titleRect.x= TitleArea.x; break; }
//            case TextAlignment.Right: { titleRect.x= TitleArea.xMax-myTitleSize.x; break; }
//            case TextAlignment.Center:
//            default:                  { titleRect.x= TitleArea.x+0.5f*(TitleArea.width-myTitleSize.x); break; }
//        } 
//        // Display title.
//        titleRect= Math3D.Intersection(FrameArea, titleRect);
//        if(titleRect.width > 0 && titleRect.height > 0) GUI.Label(titleRect, Title, TitleGUIStyle);
//    }
//    // ----------------------------------------------------------------------
//    public void DisplayTitleSeperator() {
//        if(!myTitleSeperator) return;
//        if(FrameArea.width <= 0 || FrameArea.height <= 0) return;
//        float x= FrameArea.x;
//        float y= FrameArea.y+myTitleSize.y;
//        Rect seperatorRect= new Rect(x, y, FrameArea.width, 3f);
//        GUI.Box(seperatorRect, "");
//    }
//    
//    // ======================================================================
//    // View area management.
//    // ----------------------------------------------------------------------
//    void UpdateTitleAndBodyArea(Rect displayArea) {
//        // Update title & subview coordinates.
//        myTitleArea= displayArea;
//        myBodyArea= displayArea;
//        myTitleArea.height= 0f;
//        if(myTitle != null && myTitleSize.y != 0) {
//            myTitleArea.height+= myTitleSize.y;
//            myBodyArea.y+= myTitleSize.y;
//            myBodyArea.height-= myTitleSize.y;
//        }
//        if(myTitleSeperator) {
//            myTitleArea.height+= 3f;
//            myBodyArea.y+= 3f;
//            myBodyArea.height-= 3f;
//        }
//        // Validate adjusted cordinates.
//        if(myTitleArea.width < 0f) myTitleArea.width= 0f;
//        if(myTitleArea.height > displayArea.height) myTitleArea.height= DisplayArea.height;
//        if(myBodyArea.width < 0f) myBodyArea.width= 0f;
//        if(myBodyArea.x > displayArea.xMax) myBodyArea.x= displayArea.xMax;
//        if(myBodyArea.y > displayArea.yMax) myBodyArea.y= displayArea.yMax;
//        if(myBodyArea.xMax > displayArea.xMax) myBodyArea.xMax= displayArea.xMax;
//        if(myBodyArea.yMax > displayArea.yMax) myBodyArea.yMax= displayArea.yMax;
//    }
//    
//	// ======================================================================
//    // DSView overrides.
//    // ----------------------------------------------------------------------
//    protected override void DoDisplay(Rect displayArea) {
//        UpdateTitleAndBodyArea(displayArea);
//        DisplayTitle();
//        DisplayTitleSeperator();
//        DoViewWithTitleDisplay(BodyArea);
//    }
//    protected override Vector2 DoGetDisplaySize(Rect displayArea) {
//        var contentSize= DoViewWithTitleGetDisplaySize(displayArea);
//		return contentSize;        
//    }
//
//    // ======================================================================
//    // Functions to override to create a concrete view.
//    // ----------------------------------------------------------------------
//    protected abstract void    DoViewWithTitleDisplay(Rect displayArea);
//    protected abstract Vector2 DoViewWithTitleGetDisplaySize(Rect displayArea); 
//}
//