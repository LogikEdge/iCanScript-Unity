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
        // Update Behaviour game object...
        var behaviourComponents= GameObject.FindObjectsOfType(typeof(iCS_Behaviour)) as Component[];
        if(allVisualScriptsInScene.Length == 0) {
            foreach(var component in behaviourComponents) {
                var go= component.gameObject;
                if(go != null) {
//                    Debug.Log("iCanScript: Removing hidden Behaviour game object");
                    GameObject.DestroyImmediate(go);                    
                }
            }
        } else {
            if(behaviourComponents.Length == 0) {
//                Debug.Log("iCanScript: Creating hidden Behaviour game object");
                var behaviourObject= new GameObject("iCS_HiddenBehaviour");
                behaviourObject.AddComponent("iCS_Behaviour");
                behaviourObject.hideFlags= HideFlags.HideInHierarchy | HideFlags.HideInInspector | HideFlags.NotEditable;
            }
            for(int i= 1; i < behaviourComponents.Length; ++i) {
                var component= behaviourComponents[i];
                var go= component.gameObject;
                if(go != null) {
//                    Debug.Log("iCanScript: Removing hidden Behaviour game object");
                    GameObject.DestroyImmediate(go);                    
                }
            }
        }
    }
}
