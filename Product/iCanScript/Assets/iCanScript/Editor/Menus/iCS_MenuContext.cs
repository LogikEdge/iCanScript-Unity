using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Editor;

public class iCS_MenuContext {
    public string               Command;
    public string               HiddenCommand;
    public iCS_EditorObject     SelectedObject;
    public iCS_IStorage         Storage;
    public iCS_FunctionPrototype   Descriptor;
	public Vector2				GraphPosition;

    // ======================================================================
    // Menu context constructors.
    // ----------------------------------------------------------------------
    public iCS_MenuContext(string command, iCS_FunctionPrototype descriptor= null) {
		// All other fields are filled-in on a need bases.
		Command= command;
        HiddenCommand= command;
		Descriptor= descriptor;
	}
}
