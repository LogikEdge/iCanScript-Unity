using UnityEngine;
using System;
using System.Collections;

public static class iCS_EditionController {
	static Type ourDevMenus= null;
	static Type ourTrialMenus= null;
	
    // =================================================================================
    // Installation
    // ---------------------------------------------------------------------------------
	static iCS_EditionController() {
		ourDevMenus= Type.GetType("iCS_DevMenus", false);
		ourTrialMenus= Type.GetType("iCS_TrialDialogs", false);
	}
	
    public static void Start() {}
    public static void Shutdown() {}
    
    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
    public static bool IsStoreEdition {
        get { return ourDevMenus == null && ourTrialMenus == null; }
    }
    public static bool IsTrialEdition {
        get { return ourDevMenus == null && ourTrialMenus != null; }
    }
    public static bool IsDevEdition {
        get { return ourDevMenus != null; }
    }
    public new static string ToString() {
        if(IsDevEdition) return "Development";
        if(IsTrialEdition) return "User Trial";
        if(IsStoreEdition) return "Unity Store";
        return "Unknown";
    }
}
