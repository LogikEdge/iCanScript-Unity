using UnityEngine;
using System;
using System.Collections;

public static class iCS_EditionController {
    // =================================================================================
    // Installation
    // ---------------------------------------------------------------------------------
	static iCS_EditionController() {}
	
    public static void Start() {}
    public static void Shutdown() {}
    
    // ======================================================================
    // Edition Query
    // ----------------------------------------------------------------------
#if TRIAL_EDITION
    public static bool IsStoreEdition { get { return false; }}
    public static bool IsTrialEdition { get { return true;  }}
    public static bool IsDevEdition   { get { return false; }}
#else
#if UNITY_STORE_EDITION
    public static bool IsStoreEdition { get { return true; }}
    public static bool IsTrialEdition { get { return false;  }}
    public static bool IsDevEdition   { get { return false; }}
#else
    public static bool IsStoreEdition { get { return false; }}
    public static bool IsTrialEdition { get { return false;  }}
    public static bool IsDevEdition   { get { return true; }}
#endif
#endif
    public new static string ToString() {
        if(IsDevEdition) return "Development";
        if(IsTrialEdition) return "Trial";
        if(IsStoreEdition) return "Unity Store";
        return "Unknown";
    }
}
