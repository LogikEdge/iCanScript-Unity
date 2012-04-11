using UnityEngine;
using UnityEditor;
using System.Collections;

//public class DSTableColumn : DSViewWithTitle {
//    // ======================================================================
//    // Fields
//    // ----------------------------------------------------------------------
//    string  myIdentifier= null;
//    Vector2 myDataSize  = Vector2.zero;
//	float   myWidth     = 0;
//    
//    // ======================================================================
//    // Properties
//    // ----------------------------------------------------------------------
//    public string Identifier {
//        get { return myIdentifier; }
//        set { myIdentifier= value; }
//    }
//    public Vector2 DataSize {
//        get { return myDataSize; }
//        set { myDataSize= value; }
//    }
//	public float Width {
//		get { return myWidth; }
//		set {
//			if(value == 0) { myWidth= 0; return; }
//			if(value < 2f*Margins.horizontal) value= 2f*Margins.horizontal;
//			myWidth= value;
//		}
//	}
//    
//    // ======================================================================
//    // Initialization
//    // ----------------------------------------------------------------------
//    public DSTableColumn(string identifier, GUIContent title, TextAlignment titleAlignment, bool titleSeperator,
//                         RectOffset margins, bool shouldDisplayFrame= true)
//    : base(title, titleAlignment, titleSeperator, margins, shouldDisplayFrame) {
//        Identifier= identifier;
//    }
//    
//    // ======================================================================
//    // Display
//    // ----------------------------------------------------------------------
//
//	// ======================================================================
//    // View method overrides
//    // ----------------------------------------------------------------------
//    protected Vector2 GetFullFrameSize() {
//		return new Vector2(DataSize.x+Margins.horizontal, DataSize.y+Margins.vertical+TitleArea.height);
//    }
//}
