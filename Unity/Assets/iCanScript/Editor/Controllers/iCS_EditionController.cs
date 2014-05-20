using UnityEngine;
using System;
using System.Collections;

public static class iCS_EditionController {

    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
    public static bool IsStoreEdition {
        get { return Type.GetType("iCS_DemoDialogs", false) == null; }
    }
    public static bool IsDemoEdition {
        get { return Type.GetType("iCS_DemoDialogs", false) != null; }
    }
    public static bool IsDevEdition {
        get { return Type.GetType("iCS_DevMenus", false) != null; }
    }
    public new static string ToString() {
        if(IsDevEdition) return "Development";
        if(IsDemoEdition) return "Demo";
        if(IsStoreEdition) return "Unity Store";
        return "Unknown";
    }
}
