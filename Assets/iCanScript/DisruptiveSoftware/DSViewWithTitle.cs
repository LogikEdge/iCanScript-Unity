using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSViewWithTitle : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIContent      myTitle               = null;
    TextAlignment   myTitleAlignment      = TextAlignment.Center;
    Vector2         myTitleSize           = Vector2.zero;
    bool            myTitleSeperator      = false;
    Rect            myTitleArea;
    Rect            myBodyArea;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public GUIContent Title {
        get { return myTitle; }
        set {
            myTitle= value;
            if(myTitle != null) {
                myTitleSize= EditorStyles.boldLabel.CalcSize(myTitle);                
            } else {
                myTitleSize= Vector2.zero;
            }
            UpdateTitleAndBodyArea();
        }
    }
    public bool TitleSeperator {
        get { return myTitleSeperator; }
        set { myTitleSeperator= value; UpdateTitleAndBodyArea(); }
    }
    public TextAlignment TitleAlignment {
        get { return myTitleAlignment; }
        set { myTitleAlignment= value; }
    }
    public Rect TitleArea   { get { return myTitleArea; }}
    public Rect BodyArea    { get { return myBodyArea; }}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSViewWithTitle(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                           RectOffset margins, Rect frameArea)
    : base(margins, frameArea) {
        Title= title;
        TitleSeperator= titleSeperator;
        TitleAlignment= titleAlignment;
    }
    public DSViewWithTitle(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                           RectOffset margins)
    : this(title, titleAlignment, titleSeperator, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public virtual void Display(Rect frameArea) { FrameArea= frameArea; Display(); }
    public virtual void Display() {
        GUI.Box(FrameArea,"");
        if(myTitle != null && myTitleSize.x != 0) {
            // Display title.
            Rect titleRect;
            switch(TitleAlignment) {
                case TextAlignment.Left: {
                    titleRect= new Rect(TitleArea.x, TitleArea.y, myTitleSize.x, myTitleSize.y);
                    break;
                }
                case TextAlignment.Right: {
                    titleRect= new Rect(TitleArea.xMax-myTitleSize.x, TitleArea.y, myTitleSize.x, myTitleSize.y);
                    break;
                }
                case TextAlignment.Center:
                default: {
                    titleRect= new Rect(TitleArea.x+0.5f*(TitleArea.width-myTitleSize.x), TitleArea.y, myTitleSize.x, myTitleSize.y);
                    break;
                }
            }
            GUI.Label(titleRect, Title, EditorStyles.boldLabel);        
            // Display title seperator.
            if(myTitleSeperator) {
                float x= FrameArea.x;
                float y= FrameArea.y+myTitleSize.y;
                Rect seperatorRect= new Rect(x, y, FrameArea.width, 3f);
                GUI.Box(seperatorRect, "");
            }
        }
    }

    // ======================================================================
    // View area management.
    // ----------------------------------------------------------------------
    protected override void ViewAreaDidChange() {
        base.ViewAreaDidChange();
        UpdateTitleAndBodyArea();
    }
    void UpdateTitleAndBodyArea() {
        // Update title & subview coordinates.
        myTitleArea= ContentArea;
        myTitleArea.height= 0f;
        if(myTitle != null && myTitleSize.y != 0) {
            myTitleArea.height+= myTitleSize.y;
            myBodyArea.y+= myTitleSize.y;
            myBodyArea.height-= myTitleSize.y;
        }
        if(myTitleSeperator) {
            myTitleArea.height+= 3f;
            myBodyArea.y+= 3f;
            myBodyArea.height-= 3f;
        }
        // Validate adjusted cordinates.
        if(myTitleArea.width < 0f) myTitleArea.width= 0f;
        if(myTitleArea.height > ContentArea.height) myTitleArea.height= ContentArea.height;
        if(myBodyArea.width < 0f) myBodyArea.width= 0f;
        if(myBodyArea.x > ContentArea.xMax) myBodyArea.x= ContentArea.xMax;
        if(myBodyArea.y > ContentArea.yMax) myBodyArea.y= ContentArea.yMax;
        if(myBodyArea.xMax > ContentArea.xMax) myBodyArea.xMax= ContentArea.xMax;
        if(myBodyArea.yMax > ContentArea.yMax) myBodyArea.yMax= ContentArea.yMax;
    }
}
