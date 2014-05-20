using UnityEngine;
using System;
using System.Collections;

public static class iCS_EditionController {
	static Type ourDevMenus= null;
	static Type ourDemoMenus= null;
	
	static iCS_EditionController() {
		ourDevMenus= Type.GetType("iCS_DevMenus", false);
		ourDemoMenus= Type.GetType("iCS_DemoDialogs", false);
	}
	
    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
    public static bool IsStoreEdition {
        get { return ourDevMenus == null && ourDemoMenus == null; }
    }
    public static bool IsDemoEdition {
        get { return ourDevMenus == null && ourDemoMenus != null; }
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
