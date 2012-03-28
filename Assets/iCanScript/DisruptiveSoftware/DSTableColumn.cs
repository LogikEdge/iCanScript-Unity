using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myIdentifier;
    Vector2 myDataSize;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
//    public float FrameWidth {
//        get { return myContentSize.x+Margins.horizontal; }
//        set { myContentSize.x= value-Margins.horizontal; }
//    }
    public Vector2 DataSize {
        get { return myDataSize; }
        set { myDataSize= value; }
    }
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float dataWidth, RectOffset margins, Rect viewArea)
    : base(title, titleAlignment, titleSeperator, margins, viewArea) {
        Identifier= identifier;
        myDataSize= new Vector2(dataWidth, 0);
    }
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         float dataWidth, RectOffset margins)
    : this(identifier, title, titleAlignment, titleSeperator, dataWidth, margins, new Rect(0,0,0,0)) {}
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    public override void Display() {
        base.Display();
    }
}
