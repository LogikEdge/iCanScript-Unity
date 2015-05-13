using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public class DSTableColumn {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        string				myIdentifier= null;
        Vector2				myDataSize  = Vector2.zero;
    	RectOffset			myMargins;
    	DSView.AnchorEnum	myAnchor;
    	GUIContent			myTitle;
    
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
    	public RectOffset Margins {
    		get { return myMargins; }
    		set { myMargins= value; }
    	}
    	public DSView.AnchorEnum Anchor {
    		get { return myAnchor; }
    		set { myAnchor= value; }
    	}
    	public GUIContent Title {
    		get { return myTitle; }
    		set { myTitle= value; }
    	}

        // ======================================================================
        // Initialization
        // ----------------------------------------------------------------------
        public DSTableColumn(string identifier, RectOffset margins, GUIContent title, DSView.AnchorEnum columnAlignment) {
            Identifier= identifier;
    		Title= title;
    		Margins= margins;
    		Anchor= columnAlignment;
        }
        
    }
}    