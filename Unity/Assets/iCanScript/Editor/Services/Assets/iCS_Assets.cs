using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public static class iCS_Assets {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    static iCS_VisualScript[]   allVisualScriptsInScene = new iCS_VisualScript[0];
    static iCS_VisualScript[]   allVisualScriptInPrefabs= new iCS_VisualScript[0];
    
    // ======================================================================
    // Property
	// ----------------------------------------------------------------------
    public static string CurrentSceneGUID {
        get { return AssetDatabase.AssetPathToGUID(EditorApplication.currentScene); }
    }
    public static iCS_VisualScript[] VisualScriptsInCurrentScene {
        get { return allVisualScriptsInScene; }
    }
    public static iCS_VisualScript[] VisualScriptsInProject {
        get { return allVisualScriptInPrefabs; }
    }
    public static GameObject HiddenBehaviourObject {
        get { return null; }
    }
    
    // ======================================================================
    // Initialization
	// ----------------------------------------------------------------------
    static iCS_Assets() {
        iCS_SystemEvents.OnSceneChanged    += LoadAssets;
        iCS_SystemEvents.OnHierarchyChanged+= ReloadAssets;
        iCS_SystemEvents.OnProjectChanged  += ReloadAssets;
        LoadAssets();
    }
    public static void Start() {}
    
    // ======================================================================
    // Member code generation
	// ----------------------------------------------------------------------
    static void LoadAssets() {
        // Get all visual scripts.
        var allVisualScripts= Resources.FindObjectsOfTypeAll(typeof(iCS_VisualScript)) as iCS_VisualScript[];
        int visualScriptInPrefabCount= 0;
        int visualScriptInSceneCount= 0;
        foreach(var vs in allVisualScripts) {
			var go= vs.gameObject;
			if(go == null) continue;
			var prefabType= PrefabUtility.GetPrefabType(go);
			if(prefabType == PrefabType.Prefab) {
			    ++visualScriptInPrefabCount;
		    } else {
		        ++visualScriptInSceneCount;
		    }
        }
//        Debug.Log("# Visual Scripts in Scene: "+visualScriptInSceneCount+" in project: "+visualScriptInPrefabCount);
        // Separate scene from project visual scripts.
        allVisualScriptsInScene= new iCS_VisualScript[visualScriptInSceneCount];
        allVisualScriptInPrefabs= new iCS_VisualScript[visualScriptInPrefabCount];
        int prefabIdx= 0;
        int sceneIdx= 0;
        foreach(var vs in allVisualScripts) {
			var go= vs.gameObject;
			if(go == null) continue;
			var prefabType= PrefabUtility.GetPrefabType(go);
			if(prefabType == PrefabType.Prefab) {
			    allVisualScriptInPrefabs[prefabIdx++]= vs;
		    } else {
		        allVisualScriptsInScene[sceneIdx++]= vs;
		    }            
        }
        PerformSanityCheckOnAssets();
    }
    static void ReloadAssets() {
        LoadAssets();
        PerformSanityCheckOnAssets();
    }

    // ======================================================================
    // Member code generation
	// ----------------------------------------------------------------------
    static void PerformSanityCheckOnAssets() {
    }
}
