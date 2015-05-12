using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {
    public class iCS_MenuContext {
        public string               Command;
        public string               HiddenCommand;
        public iCS_EditorObject     SelectedObject;
        public iCS_IStorage         Storage;
    	public Vector2				GraphPosition;
        public LibraryObject        myLibraryObject;

        // ======================================================================
        // Menu context constructors.
        // ----------------------------------------------------------------------
        public iCS_MenuContext(string command, LibraryObject libraryObject= null) {
    		// All other fields are filled-in on a need bases.
    		Command        = command;
            HiddenCommand  = command;
    		myLibraryObject= libraryObject;
    	}
    }
    
}

