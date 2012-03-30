using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSViewWithTitle : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIContent      myTitle         = null;
    TextAlignment   myTitleAlignment= TextAlignment.Center;
    bool            myTitleSeperator= false;
    GUIStyle        myTitleGUIStyle = EditorStyles.boldLabel;
    Vector2         myTitleSize     = Vector2.zero;
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
                myTitleSize= TitleGUIStyle.CalcSize(myTitle);                
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
    public GUIStyle TitleGUIStyle {
        get { return myTitleGUIStyle; }
        set { myTitleGUIStyle= value ?? EditorStyles.boldLabel; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSViewWithTitle(GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                           RectOffset margins, bool shouldDisplayFrame= true)
     : base(margins, shouldDisplayFrame) {
        Title= title;
        TitleSeperator= titleSeperator;
        TitleAlignment= titleAlignment;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    protected override void Display() {
        base.Display();
        DisplayTitle();
        DisplayTitleSeperator();
    }
    public void DisplayTitle() {
        if(FrameArea.width <= 0 || FrameArea.height <= 0) return;
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
            titleRect= Math3D.Intersection(FrameArea, titleRect);
            if(titleRect.width > 0 && titleRect.height > 0) GUI.Label(titleRect, Title, EditorStyles.boldLabel);        
        }
    }
    public void DisplayTitleSeperator() {
        if(FrameArea.width <= 0 || FrameArea.height <= 0) return;
        if(!myTitleSeperator) return;
        float x= FrameArea.x;
        float y= FrameArea.y+myTitleSize.y;
        Rect seperatorRect= new Rect(x, y, FrameArea.width, 3f);
        GUI.Box(seperatorRect, "");
    }
    
    // ======================================================================
    // View area management.
    // ----------------------------------------------------------------------
    protected override void ViewAreaDidChange() {
        base.ViewAreaDidChange();
        UpdateTitleAndBodyArea();
    }
    protected override Vector2 GetMinimumFrameSize() { 
        var baseMinFrameSize= base.GetMinimumFrameSize();
        float width= Mathf.Max(myTitleSize.x+Margins.horizontal, baseMinFrameSize.x);
        float height= myTitleSize.y+baseMinFrameSize.y;
        if(TitleSeperator) {
            height+= 3f;
        }
        return new Vector2(width, height);
    }

    void UpdateTitleAndBodyArea() {
        // Update title & subview coordinates.
        myTitleArea= DisplayArea;
        myBodyArea= DisplayArea;
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
        if(myTitleArea.height > DisplayArea.height) myTitleArea.height= DisplayArea.height;
        if(myBodyArea.width < 0f) myBodyArea.width= 0f;
        if(myBodyArea.x > DisplayArea.xMax) myBodyArea.x= DisplayArea.xMax;
        if(myBodyArea.y > DisplayArea.yMax) myBodyArea.y= DisplayArea.yMax;
        if(myBodyArea.xMax > DisplayArea.xMax) myBodyArea.xMax= DisplayArea.xMax;
        if(myBodyArea.yMax > DisplayArea.yMax) myBodyArea.yMax= DisplayArea.yMax;
    }
}
