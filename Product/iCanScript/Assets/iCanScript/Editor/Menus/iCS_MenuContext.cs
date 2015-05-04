using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Editor {
    public class iCS_MenuContext {
        public string               Command;
        public string               HiddenCommand;
        public iCS_EditorObject     SelectedObject;
        public iCS_IStorage         Storage;
        public iCS_FunctionPrototype   Descriptor;
    	public Vector2				GraphPosition;
        public LibraryObject        myLibraryObject;

        // ======================================================================
        // Menu context constructors.
        // ----------------------------------------------------------------------
        public iCS_MenuContext(string command, iCS_FunctionPrototype descriptor) {
    		// All other fields are filled-in on a need bases.
    		Command        = command;
            HiddenCommand  = command;
    		Descriptor     = descriptor;
            myLibraryObject= null;
    	}
        // ----------------------------------------------------------------------
        public iCS_MenuContext(string command, LibraryObject libraryObject) {
    		// All other fields are filled-in on a need bases.
    		Command        = command;
            HiddenCommand  = command;
    		myLibraryObject= libraryObject;
            Descriptor     = null;
    	}
        // ----------------------------------------------------------------------
        public iCS_MenuContext(string command) {
    		// All other fields are filled-in on a need bases.
    		Command        = command;
            HiddenCommand  = command;
    		Descriptor     = null;
            myLibraryObject= null;
    	}
    }
    
}

