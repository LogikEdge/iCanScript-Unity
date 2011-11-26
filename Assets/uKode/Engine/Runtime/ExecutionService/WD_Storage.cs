using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of uCode.  All object are derived
// from this storage class.
public class WD_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
                      public WD_UserPreferences       Preferences  = new WD_UserPreferences();
    [HideInInspector] public List<WD_EditorObject>    EditorObjects= new List<WD_EditorObject>();
    [HideInInspector] public List<Object>             UnityObjects = new List<Object>();
    [HideInInspector] public int                      UndoRedoId   = 0;

    // ----------------------------------------------------------------------
	// We do not want this behaviour to run.
	void Start () { /*enabled= false;*/ }
	
}
