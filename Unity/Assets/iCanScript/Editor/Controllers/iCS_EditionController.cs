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
#if COMMUNITY_EDITION
    public static bool IsProEdition         { get { return false; }}
    public static bool IsCommunityEdition   { get { return true;  }}
    public static bool IsDevEdition         { get { return false; }}
#else
#if PRO_EDITION
    public static bool IsProEdition         { get { return true;  }}
    public static bool IsCommunityEdition   { get { return false; }}
    public static bool IsDevEdition         { get { return false; }}
#else
    public static bool IsProEdition         { get { return false; }}
    public static bool IsCommunityEdition   { get { return false; }}
    public static bool IsDevEdition         { get { return true;  }}
#endif
#endif
    public new static string ToString() {
        if(IsDevEdition)       return "Development";
        if(IsCommunityEdition) return "Community";
        if(IsProEdition)       return "Pro";
        return "Unknown";
    }

    // ======================================================================
    // Community Edition Queries
    // ----------------------------------------------------------------------
    public const int MaxComnunityVisualScriptPerScene= 5;
    public const int MaxCommunityNodesPerVisualScript= 50;
    public static int CommunityVisualScriptsRemaining {
        get {
            return MaxComnunityVisualScriptPerScene-iCS_SceneController.NumberOfVisualScriptsInOrReferencedByScene;
        }
    }
    public static float CommunityPercentVisualScriptsRemaining {
        get { return (float)(CommunityVisualScriptsRemaining) / MaxComnunityVisualScriptPerScene; }
    }
    public static int CommunityNodesRemaining {
        get {
            var iStorage= iCS_VisualScriptDataController.IStorage;
            if(iStorage == null) return MaxCommunityNodesPerVisualScript;
            return MaxCommunityNodesPerVisualScript-iStorage.NumberOfNodes;
        }
    }
    public static float CommunityPercentNodesRemaining {
        get { return (float)(CommunityNodesRemaining) / MaxCommunityNodesPerVisualScript;}
    }
    public static bool IsCommunityLimitReached {
        get { return CommunityVisualScriptsRemaining < 0 || CommunityNodesRemaining < 0; }
    }
}
