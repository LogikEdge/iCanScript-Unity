﻿using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace iCanScriptEditor {
    
    public static class EditionController {
        // =================================================================================
        // Installation
        // ---------------------------------------------------------------------------------
    	static EditionController() {}
    	
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
        // Community License Management
        // ----------------------------------------------------------------------
        public const int MaxCommunityVisualScriptPerScene= 7;
        public const int MaxCommunityNodesPerVisualScript= 50;
        public static int CommunityVisualScriptsRemaining {
            get {
                if(EditorApplication.isPlaying) return MaxCommunityVisualScriptPerScene;
                return MaxCommunityVisualScriptPerScene-SceneController.NumberOfVisualScriptsInOrReferencedByScene;
            }
        }
        public static float CommunityPercentVisualScriptsRemaining {
            get { return EditorApplication.isPlaying ? 1.0f : (float)(CommunityVisualScriptsRemaining) / MaxCommunityVisualScriptPerScene; }
        }
        public static int CommunityNodesRemaining {
            get {
                if(EditorApplication.isPlaying) return MaxCommunityNodesPerVisualScript;
                var iStorage= iCS_VisualScriptDataController.IStorage;
                if(iStorage == null) return MaxCommunityNodesPerVisualScript;
                return MaxCommunityNodesPerVisualScript-iStorage.NumberOfNodes;
            }
        }
        public static float CommunityPercentNodesRemaining {
            get { return EditorApplication.isPlaying ? 1.0f : (float)(CommunityNodesRemaining) / MaxCommunityNodesPerVisualScript;}
        }
        public static bool IsCommunityLimitReached {
            get {
                if(!IsCommunityEdition || EditorApplication.isPlaying) return false;
                return CommunityVisualScriptsRemaining < 0 || CommunityNodesRemaining < 0;
            }
        }
    }
    
}