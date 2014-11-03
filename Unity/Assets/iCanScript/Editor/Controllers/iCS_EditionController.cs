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
    public static bool IsProEdition   { get { return false; }}
    public static bool IsTrialEdition { get { return true;  }}
    public static bool IsDevEdition   { get { return false; }}
#else
#if UNITY_STORE_EDITION
    public static bool IsProEdition   { get { return true;  }}
    public static bool IsTrialEdition { get { return false; }}
    public static bool IsDevEdition   { get { return false; }}
#else
    public static bool IsProEdition   { get { return false; }}
    public static bool IsTrialEdition { get { return false; }}
    public static bool IsDevEdition   { get { return true;  }}
#endif
#endif
    public new static string ToString() {
        if(IsDevEdition)   return "Development";
        if(IsTrialEdition) return "Trial";
        if(IsProEdition)   return "Pro";
        return "Unknown";
    }
    public const int MaxTrialVisualScriptPerScene= 5;
    public const int MaxTrialNodesPerVisualScript= 50;
    public static int TrialVisualScriptsRemaining {
        get {
            return MaxTrialVisualScriptPerScene-iCS_SceneController.NumberOfVisualScriptsInOrReferencedByScene;
        }
    }
    public static float TrialPercentVisualScriptsRemaining {
        get { return (float)(TrialVisualScriptsRemaining) / MaxTrialVisualScriptPerScene; }
    }
    public static int TrialNodesRemaining {
        get {
            var iStorage= iCS_VisualScriptDataController.IStorage;
            if(iStorage == null) return MaxTrialNodesPerVisualScript;
            return MaxTrialNodesPerVisualScript-iStorage.NumberOfNodes;
        }
    }
    public static float TrialPercentNodesRemaining {
        get { return (float)(TrialNodesRemaining) / MaxTrialNodesPerVisualScript;}
    }
}
