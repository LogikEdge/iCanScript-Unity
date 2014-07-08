using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_MenuContext {
    public string               Command;
    public string               HiddenCommand;
    public iCS_EditorObject     SelectedObject;
    public iCS_IStorage         Storage;
    public iCS_MethodBaseInfo   Descriptor;
	public Vector2				GraphPosition;

    // ======================================================================
    // Menu context constructors.
    // ----------------------------------------------------------------------
    public iCS_MenuContext(string command, iCS_MethodBaseInfo descriptor= null) {
		// All other fields are filled-in on a need bases.
		Command= command;
        HiddenCommand= command;
		Descriptor= descriptor;
	}
}
