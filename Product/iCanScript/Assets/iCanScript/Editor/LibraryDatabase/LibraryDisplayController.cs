using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {

    public class LibraryDisplayController : DSTreeViewDataSource {
        // =================================================================================
        // FIELDS
        // ---------------------------------------------------------------------------------
    	DSTreeView      myTreeView  = null;
        LibraryObject   myCursor    = null;
    	float           myFoldOffset= 16f;
        
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
    	public DSView   View  { get { return myTreeView; }}
	
        // =================================================================================
        // Constants
        // ---------------------------------------------------------------------------------
        const int   kIconWidth  = 16;
        const int   kIconHeight = 16;
        const float kLabelSpacer= 4f;
    
        // =================================================================================
        // Initialization
        // ---------------------------------------------------------------------------------
        /// Builds and iniotializes the library display controller.
    	public LibraryDisplayController() {
            // -- Initialize panel. --
    		myTreeView = new DSTreeView(new RectOffset(0,0,0,0), false, this, 16, 2);
            // -- Initialize the cursor --
            Reset();
    	}

        // ===================================================================
        // Tree View Data Source Extensions
        // -------------------------------------------------------------------
        /// Initializes the cursor position.
    	public void Reset() {
    	    myCursor= Reflection.LibraryDatabase;
    	}
    	public void BeginDisplay() {
            // -- Initialize some display constants. --
            if(myFoldOffset == 0) {
                myFoldOffset= EditorStyles.foldout.CalcSize(new GUIContent("")).x;                
            }
            // Enable RTF
            GUI.skin.label.richText= true;
            
    	}
    	public void EndDisplay() {}
        // -------------------------------------------------------------------
        /// Moves the cursor to the next element in the tree.
        ///
        /// @return _true_ if a next element exists. _false_ otherwise.
        ///
    	public bool MoveToNext() {
    		if(MoveToFirstChild()) return true;
    		if(MoveToNextSibling()) return true;
    		do {
    			if(!MoveToParent()) return false;			
    		} while(!MoveToNextSibling());
    		return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the next sibling under the same parent.
        ///
        /// @return _true_ if a next sibling exists.  _false_ otherwise.
        ///
    	public bool MoveToNextSibling() {
    	    var parent= myCursor.parent;
            if(parent == null) return false;
            var siblings= parent.children;
            if(siblings == null) return false;
            int idx= siblings.IndexOf(myCursor);
            if(idx < 0 || idx >= siblings.Count-1) return false;
            myCursor= siblings[idx+1] as LibraryObject;
            return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the first child.
        ///
        /// @return _true_ if the cursor was moved. _false_ otherwise.
        ///
    	public bool MoveToFirstChild() {
            var siblings= myCursor.children;
            if(siblings == null || siblings.Count == 0) return false;
    	    myCursor= siblings[0] as LibraryObject;
            return true;
    	}
        // -------------------------------------------------------------------
        /// Moves the cursor to the parent object.
        ///
        /// @return _true_ if the cursor was moved. _false_ otherwise.
        ///
    	public bool MoveToParent() {
    	    var parent= myCursor.parent;
            if(parent == null) return false;
            myCursor= parent as LibraryObject;
            return true;
    	}
        // -------------------------------------------------------------------
        /// Returns the area needed to display the library content.
        ///
        /// @return The area needed to display the libary content.
        ///
    	public Vector2 CurrentObjectLayoutSize() {
    	    return new Vector2(300, 16);
    	}
        // -------------------------------------------------------------------
        /// Displays the object pointed by the cursor.
        ///
        /// @param displayArea The area in which to display the object.
        /// @param foldout The current foldout state.
        /// @param frameArea The total area in which the display takes place.
        /// @return The new foldout state.
    	public bool DisplayCurrentObject(Rect displayArea, bool foldout, Rect frameArea) {
            // -- Show foldout (if needed) --
    		bool foldoutState= ShouldUseFoldout ? EditorGUI.Foldout(new Rect(displayArea.x, displayArea.y, myFoldOffset, displayArea.height), foldout, "") : false;
            // -- Get string to display --
            var displayString= myCursor.GetDisplayString();
            if(string.IsNullOrEmpty(displayString)) {
                displayString= "<empty>";
            }
            // -- Display string to user --
            displayArea.x= displayArea.x+myFoldOffset;
            GUI.Label(displayArea, displayString);
    	    return foldoutState;
    	}
        // -------------------------------------------------------------------
        /// Returns a uniqu key for the item under cursor.
        ///
        /// @return The cursor is returned.
        ///
    	public object CurrentObjectKey() {
    	    return myCursor;
    	}
        // -------------------------------------------------------------------
        /// Advises that a mouse down was performed on the given key.
        ///
        /// @param key The library object on which a mouse down occured.
        /// @param mouseInScreenPoint The position of the mouse.
        /// @param screenArea The screen area used to display the library object.
        ///
    	public void MouseDownOn(object key, Vector2 mouseInScreenPoint, Rect screenArea) {}

        // ===================================================================
        // -------------------------------------------------------------------
        bool ShouldUseFoldout {
            get {
                if(myCursor == null) return false;
                if(myCursor is LibraryRootNamespace) return true;
                if(myCursor is LibraryChildNamespace) return true;
                if(myCursor is LibraryType) return true;
                return false;
            }
        }

    }
    
}
