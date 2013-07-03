using UnityEngine;
using UnityEditor;
using System.Collections;

public static class iCS_Assets {
    static iCS_VisualEditor[]   allVisualEditors= new iCS_VisualEditor[0];
    
    static iCS_Assets() {
        iCS_SystemEvents.OnSceneChanged  += ReloadAssets;
        iCS_SystemEvents.OnProjectChanged+= ReloadAssets;
        ReloadAssets();
    }
    
    static void ReloadAssets() {
        
    }
}
