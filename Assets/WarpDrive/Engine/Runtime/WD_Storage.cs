using UnityEngine;
using System.Collections;

// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
// This class is the main storage of WarpDrive.  All object are derived
// from this storage class.
public class WD_Storage : MonoBehaviour {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public WD_UserPreferences           Preferences  = new WD_UserPreferences();
    public WD_EditorObjectMgr           EditorObjects= new WD_EditorObjectMgr();

    // ----------------------------------------------------------------------
	// We do not want this behaviour to run.
	void Start () { enabled= false; }
	
    // ======================================================================
    // Object generation functionality.
    public void GenerateEditorData() {}
    public void GenerateRuntimeData() {}
    public void CompileRuntimeToScript() {}
}
