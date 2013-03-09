using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_MenuContext {
    public string               Command;
    public iCS_EditorObject     SelectedObject;
    public iCS_IStorage         Storage;
    public iCS_ReflectionInfo   Descriptor;
	public Vector2				GraphPosition;

    // ======================================================================
    // Menu context constructors.
    // ----------------------------------------------------------------------
    public iCS_MenuContext(string command, iCS_ReflectionInfo descriptor= null) {
		// All other fields are filled-in on a need bases.
		Command= command;
		Descriptor= descriptor;
	}
}
