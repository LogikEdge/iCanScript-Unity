using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of uCode.  All object are derived
// from this storage class.
public class UK_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
                      public UK_UserPreferences       Preferences  = new UK_UserPreferences();
    [HideInInspector] public List<UK_EditorObject>    EditorObjects= new List<UK_EditorObject>();
    [HideInInspector] public List<Object>             UnityObjects = new List<Object>();
    [HideInInspector] public int                      UndoRedoId   = 0;

    // ----------------------------------------------------------------------
	// We do not want this behaviour to run.
	void Start () { /*enabled= false;*/ }
	
}
