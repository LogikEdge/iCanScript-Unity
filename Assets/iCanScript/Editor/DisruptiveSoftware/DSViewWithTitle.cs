using UnityEngine;
using UnityEditor;
using System.Collections;

//public class DSViewWithTitle : DSView {
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
//            UpdateTitleAndBodyArea();
//        }
//    }
//    public bool TitleSeperator {
//        get { return myTitleSeperator; }
//        set { myTitleSeperator= value; UpdateTitleAndBodyArea(); }
//    }
//    public TextAlignment TitleAlignment {
//        get { return myTitleAlignment; }
//        set { myTitleAlignment= value; }
//    }
//    public Rect TitleArea   { get { return myTitleArea; }}
//    public Rect BodyArea    { get { return myBodyArea; }}
//    public GUIStyle TitleGUIStyle {
//        get { return myTitleGUIStyle ?? EditorStyles.boldLabel; }
//        set { myTitleGUIStyle= value; UpdateTitleAndBodyArea(); }
//    }
//    
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSViewWithTitle(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
//                           RectOffset margins, bool shouldDisplayFrame= true)
//     : base(margins, shouldDisplayFrame) {
//        Title= title;
//        TitleSeperator= titleSeperator;
//        TitleAlignment= titleAlignment;
//    }
//    
//    // ======================================================================
//    // Display
//    // ----------------------------------------------------------------------
//    public override void Display(DSView parent, Rect frameArea) {
//        base.Display();
//        DisplayTitle();
//        DisplayTitleSeperator();
//    }
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
//    public void DisplayTitleSeperator() {
//        if(FrameArea.width <= 0 || FrameArea.height <= 0) return;
//        if(!myTitleSeperator) return;
//        float x= FrameArea.x;
//        float y= FrameArea.y+myTitleSize.y;
//        Rect seperatorRect= new Rect(x, y, FrameArea.width, 3f);
//        GUI.Box(seperatorRect, "");
//    }
//    
//    // ======================================================================
//    // View overrides.
//    // ----------------------------------------------------------------------
//    public override void OnViewChange(DSView parent, Rect displayArea) {
//        base.OnViewChange(parent, displayArea);
//        UpdateTitleAndBodyArea();
//    }
//    protected virtual Vector2 GetMinimumFrameSize() { 
//        float width= Margins.horizontal+myTitleSize.x;
//        float height= Margins.vertical+myTitleSize.y;
//        if(TitleSeperator) {
//            height+= 3f;
//        }
//        return new Vector2(width, height);
//    }
//
//    // ======================================================================
//    // View area management.
//    // ----------------------------------------------------------------------
//    void UpdateTitleAndBodyArea() {
//        // Update title & subview coordinates.
//        myTitleArea= DisplayArea;
//        myBodyArea= DisplayArea;
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
//        if(myTitleArea.height > DisplayArea.height) myTitleArea.height= DisplayArea.height;
//        if(myBodyArea.width < 0f) myBodyArea.width= 0f;
//        if(myBodyArea.x > DisplayArea.xMax) myBodyArea.x= DisplayArea.xMax;
//        if(myBodyArea.y > DisplayArea.yMax) myBodyArea.y= DisplayArea.yMax;
//        if(myBodyArea.xMax > DisplayArea.xMax) myBodyArea.xMax= DisplayArea.xMax;
//        if(myBodyArea.yMax > DisplayArea.yMax) myBodyArea.yMax= DisplayArea.yMax;
//    }
//}
