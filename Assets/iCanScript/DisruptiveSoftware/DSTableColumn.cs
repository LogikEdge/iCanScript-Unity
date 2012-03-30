using UnityEngine;
using UnityEditor;
using System.Collections;

public class DSTableColumn : DSViewWithTitle {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myIdentifier= null;
    Vector2 myDataSize  = Vector2.zero;
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public string Identifier {
        get { return myIdentifier; }
        set { myIdentifier= value; }
    }
    public Vector2 DataSize {
        get { return myDataSize; }
        set { myDataSize= value; }
    }
	public Vector2 FullFrameSize {
		get { return new Vector2(DataSize.x+Margins.horizontal, DataSize.y+Margins.vertical+TitleArea.height); }
	}
    
    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
                         RectOffset margins)
    : base(title, titleAlignment, titleSeperator, margins) {
        Identifier= identifier;
    }
    
    // ======================================================================
    // Display
    // ----------------------------------------------------------------------
    protected override void Display() {
        base.Display();
    }
}
