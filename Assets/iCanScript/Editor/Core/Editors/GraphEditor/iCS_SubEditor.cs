using UnityEngine;
using UnityEditor;
using System.Collections;

public class iCS_SubEditor : EditorWindow {
    // ======================================================================
    // Fields.
	// ----------------------------------------------------------------------
    protected iCS_EditorObject    myTarget = null;
    protected iCS_IStorage        myStorage= null;

    // ======================================================================
    // Properties.
	// ----------------------------------------------------------------------
	public iCS_EditorObject Target { get { return myTarget; }}
	
    // ======================================================================
    // Initialization
	// ----------------------------------------------------------------------
    public virtual void Init(iCS_EditorObject target, iCS_IStorage storage) {
        myTarget= target;
        myStorage= storage;
    }
}
