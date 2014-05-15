using UnityEngine;
using System.Collections;

public static class iCS_EditionController {

    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
    public static bool IsStoreEdition {
        get { return true; }
    }
    public static bool IsDemoEdition {
        get { return false; }
    }
}
