using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSViewWithTitle : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    GUIContent  myTitle         = null;
    Vector2     myTitleSize     = Vector2.zero;
    bool        myTitleSeperator= false;
    Rect        myTitleArea;
    Rect        mySubviewArea;
    
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
            UpdateSubviewArea();
        }
    }
    public bool TitleSeperator {
        get { return myTitleSeperator; }
        set { myTitleSeperator= value; UpdateSubviewArea(); }
    }
    public Rect SubviewArea {
        get { return mySubviewArea; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSViewWithTitle(Rect viewArea, GUIContent title, bool titleSeperator= false) {
        ViewArea= viewArea;
        Title= title;
        TitleSeperator= titleSeperator;
    }
    public DSViewWithTitle(GUIContent title, bool titleSeperator= false) : this(new Rect(0,0,0,0), title, titleSeperator) {}
    public DSViewWithTitle() : this(new GUIContent("title")) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public void Display(Rect viewArea) { ViewArea= viewArea; Display(); }
    public void Display() {
        GUI.Box(ViewArea,"");
        if(myTitle != null && myTitleSize.x != 0) {
            CenterTitle(ViewArea, myTitle, myTitleSize);
            if(myTitleSeperator) {
                float x= ViewArea.x;
                float y= ViewArea.y+myTitleSize.y;
                Rect seperatorRect= new Rect(x, y, ViewArea.width, 3f);
                GUI.Box(seperatorRect, "");
            }
        }
    }

    // ======================================================================
    // Display Utilities
    // ----------------------------------------------------------------------
    void CenterTitle(Rect rect, GUIContent content, Vector2 contentSize) {
        CenterLabel(rect, content, contentSize, EditorStyles.boldLabel);
    }
    void CenterLabel(Rect rect, GUIContent content, Vector2 contentSize, GUIStyle style) {
        GUI.Label(new Rect(CenterHorizontaly(contentSize.x), rect.y, contentSize.x, contentSize.y), content, style);        
    }
    void UpdateSubviewArea() {
        // Update subview area.
        mySubviewArea= ViewArea;
        myTitleArea= ViewArea;
        myTitleArea.height= 0f;
        if(myTitle != null && myTitleSize.y != 0) {
            myTitleArea.height+= myTitleSize.y;
            mySubviewArea.y+= myTitleSize.y;
            mySubviewArea.height-= myTitleSize.y;
        }
        if(myTitleSeperator) {
            myTitleArea.height+= 3f;
            mySubviewArea.y+= 3f;
            mySubviewArea.height-= 3f;
        }
    }
}
