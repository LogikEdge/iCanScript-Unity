using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_MenuContext {
    public string               Command;
    public iCS_EditorObject     SelectedObject;
    public iCS_IStorage         Storage;
    public iCS_ReflectionInfo   Descriptor;

    // ======================================================================
    // Menu context constructors.
    // ----------------------------------------------------------------------
    public iCS_MenuContext(string command, iCS_ReflectionInfo descriptor= null)
	: this(command, descriptor, null, null) {}
    public iCS_MenuContext(string command, iCS_ReflectionInfo descriptor, iCS_EditorObject selected, iCS_IStorage storage) {
        Command= command;
        SelectedObject= selected;
        Storage= storage;
        Descriptor= descriptor;
    }
}
