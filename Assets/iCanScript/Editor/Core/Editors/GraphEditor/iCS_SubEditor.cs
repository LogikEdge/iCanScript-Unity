using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_SubEditor : EditorWindow {
    // ======================================================================
    // Field.
	// ----------------------------------------------------------------------
    protected iCS_EditorObject    myTarget = null;
    protected iCS_IStorage        myStorage= null;

    // ======================================================================
    // Initialization
	// ----------------------------------------------------------------------
    public virtual void Init(iCS_EditorObject target, iCS_IStorage storage) {
        myTarget= target;
        myStorage= storage;
    }
}
