using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSView {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string      myIdentifier;
    GUIContent  myTitle;
    Vector2     myTitleSize;
    float       myMinimumWidth;
    float       myMaximumWidth;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
    public GUIContent Title {
        get { return myTitle; }
        set { myTitle= value; myTitleSize= EditorStyles.boldLabel.CalcSize(myTitle); }
    }
    public float MinimumWidth {
        get { return myMinimumWidth; }
        set { myMinimumWidth= value; }
    }
    public float MaximumWidth {
        get { return myMaximumWidth; }
        set { myMaximumWidth= value; }
    }

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title) {
        Identifier= identifier;
        Title= title;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public void Display(Rect viewArea) { ViewArea= viewArea; Display(); }
    public void Display() {
        
    }
}
