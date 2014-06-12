using UnityEngine;
using System;
using System.Collections;

public static class iCS_EditionController {
	static Type ourDevMenus= null;
	static Type ourTrialMenus= null;
	
	static iCS_EditionController() {
		ourDevMenus= Type.GetType("iCS_DevMenus", false);
		ourTrialMenus= Type.GetType("iCS_TrialDialogs", false);
	}
	
    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
    public static bool IsStoreEdition {
        get { return ourDevMenus == null && ourTrialMenus == null; }
    }
    public static bool IsDemoEdition {
        get { return ourDevMenus == null && ourTrialMenus != null; }
    }
    public static bool IsDevEdition {
        get { return ourDevMenus != null; }
    }
    public new static string ToString() {
        if(IsDevEdition) return "Development";
        if(IsDemoEdition) return "User Trial";
        if(IsStoreEdition) return "Unity Store";
        return "Unknown";
    }
}
