using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myIdentifier;
    float   myClientMinimumWidth;
    float   myClientMaximumWidth;
    float   myClientMinSize;
    float   myClientMaxSize;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
    public float MinimumFrameWidth {
        get { return myClientMinimumWidth+Margins.horizontal; }
        set { myClientMinimumWidth= value-Margins.horizontal; }
    }
    public float MaximumFrameWidth {
        get { return myClientMaximumWidth+Margins.vertical; }
        set { myClientMaximumWidth= value-Margins.vertical; }
    }

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float minSize, float maxSize, RectOffset margins, Rect viewArea)
    : base(title, titleAlignment, titleSeperator, margins, viewArea) {
        Identifier= identifier;
        myClientMinSize= minSize;
        myClientMaxSize= maxSize;
    }
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float minSize, float maxSize, RectOffset margins)
    : this(identifier, title, titleAlignment, titleSeperator, minSize, maxSize, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {}
}
