using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myIdentifier;
    float   myContentWidth;
    bool    myIsStrechable= false;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
    public float FrameWidth {
        get { return myContentWidth+Margins.horizontal; }
        set { myContentWidth= value-Margins.horizontal; }
    }
    public float ContentWidth {
        get { return myContentWidth; }
        set { myContentWidth= value; }
    }
    public bool IsStrechable {
        get { return myIsStrechable; }
        set { myIsStrechable= value; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float contentWidth, RectOffset margins, Rect viewArea)
    : base(title, titleAlignment, titleSeperator, margins, viewArea) {
        Identifier= identifier;
        myContentWidth= contentWidth;
    }
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float contentWidth, RectOffset margins)
    : this(identifier, title, titleAlignment, titleSeperator, contentWidth, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        base.Display();
    }
}
