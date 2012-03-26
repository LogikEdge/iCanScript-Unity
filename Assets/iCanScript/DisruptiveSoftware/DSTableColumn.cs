using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myIdentifier;
    float   myMinimumContentWidth;
    float   myMaximumContentWidth;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
    public float MinimumFrameWidth {
        get { return myMinimumContentWidth+Margins.horizontal; }
        set { myMinimumContentWidth= value-Margins.horizontal; }
    }
    public float MaximumFrameWidth {
        get { return myMaximumContentWidth+Margins.vertical; }
        set { myMaximumContentWidth= value-Margins.vertical; }
    }
    public float MinimumContentWidth {
        get { return myMinimumContentWidth; }
        set { myMinimumContentWidth= value; }
    }
    public float MaximumContentWidth {
        get { return myMaximumContentWidth; }
        set { myMaximumContentWidth= value; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float minWidth, float maxWidth, RectOffset margins, Rect viewArea)
    : base(title, titleAlignment, titleSeperator, margins, viewArea) {
        Identifier= identifier;
        myMinimumContentWidth= minWidth;
        myMaximumContentWidth= maxWidth;
    }
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float minWidth, float maxWidth, RectOffset margins)
    : this(identifier, title, titleAlignment, titleSeperator, minWidth, maxWidth, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {}
}
