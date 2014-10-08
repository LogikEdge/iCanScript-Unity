#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_SceneController {
    // ======================================================================
    // Scene Cache
    // ----------------------------------------------------------------------
    // Visual Scripts
    static iCS_VisualScriptImp[]    ourVisualScriptsInScene              = null;
    static iCS_VisualScriptImp[]    ourVisualScriptsReferencesByScene    = null;
    static iCS_VisualScriptImp[]    ourVisualScriptsInOrReferencesByScene= null;

    // ======================================================================
    // Scene properties
    // ----------------------------------------------------------------------
    public static iCS_VisualScriptImp[] VisualScriptsInScene {
        get { return ourVisualScriptsInScene; }
    }
    public static iCS_VisualScriptImp[] VisualScriptsReferencedByScene {
        get { return ourVisualScriptsReferencesByScene; }
    }
    public static iCS_VisualScriptImp[] VisualScriptsInOrReferencedByScene {
        get { return ourVisualScriptsInOrReferencesByScene; }
    }

    // ======================================================================
    // Common Controller activation/deactivation
    // ----------------------------------------------------------------------
	static iCS_SceneController() {
        // Events to refresh scene content information.
		iCS_SystemEvents.OnEditorStarted   = RefreshSceneInfo;
        iCS_SystemEvents.OnSceneChanged    = RefreshSceneInfo;
        iCS_SystemEvents.OnHierarchyChanged= RefreshSceneInfo;
        iCS_SystemEvents.OnProjectChanged  = RefreshSceneInfo;
		iCS_SystemEvents.OnCompileStarted  = RefreshSceneInfo;
        // Force an initial refresh of the scene info.
        RefreshSceneInfo();  
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {}

    // ======================================================================
    // Update scene content changed
    // ----------------------------------------------------------------------
    static void RefreshSceneInfo() {
        ourVisualScriptsInScene              = ScanForVisualScriptsInScene();
        ourVisualScriptsReferencesByScene    = ScanForVisualScriptsReferencedByScene();
        ourVisualScriptsInOrReferencesByScene= CombineVisualScriptsInOrReferencedByScene();
		// Vaidate proper visual script setup.
		SceneSanityCheck();
    }

    // ======================================================================
    // VISUAL SCRIPTS
    // ----------------------------------------------------------------------
    /// Returns all active VisualScript included in the current scene.
    static iCS_VisualScriptImp[] ScanForVisualScriptsInScene() {
        var allVisualScripts= UnityEngine.Object.FindObjectsOfType(typeof(iCS_VisualScriptImp));
        return Array.ConvertAll( allVisualScripts, e=> e as iCS_VisualScriptImp );
    }

    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
	///
	/// Note:	This function assumes that the visual script in the scene
	///			has already been fetched.
	///
    static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedByScene() {
		return P.removeDuplicates(
			P.fold(
				(acc,vs)=> P.append(acc, ScanForVisualScriptsReferencedBy(vs)),
				new iCS_VisualScriptImp[0],
				VisualScriptsInScene
			)
		);
    }
    
    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
	///
	/// Note:	This function assumes that the visual script in and referenced
	///			by this scene have already been fetched.
	///
    static iCS_VisualScriptImp[] CombineVisualScriptsInOrReferencedByScene() {
        return P.removeDuplicates(P.append(VisualScriptsInScene, VisualScriptsReferencedByScene));
    }
    
    // ----------------------------------------------------------------------
    /// Returns Visual Scripts referenced by the given Visual Script.
    static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedBy(iCS_VisualScriptImp vs) {
        var visualScripts= P.map(o=> o as iCS_VisualScriptImp, P.filter(o=> o is iCS_VisualScriptImp, vs.UnityObjects));
        var gameObjects  = P.map(o=> o as GameObject         , P.filter(o=> o is GameObject         , vs.UnityObjects));
		return P.removeDuplicates(
			P.fold(
				(acc,go)=> {
	                var goVs= go.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
	                if(goVs != null) {
	                    acc.Add(goVs);
	                }
					return acc;
                },
				visualScripts,
				gameObjects
			)
		).ToArray();
    }
	
    // ======================================================================
    // VALIDATE SCENE VISUAL SCRIPTS
    // ----------------------------------------------------------------------
	const string kiCS_Behaviour= iCS_EditorStrings.DefaultBehaviourClassName;
	static void SceneSanityCheck() {
		Debug.Log("SceneSanityCheck is running");
		foreach(var vs in VisualScriptsInOrReferencedByScene) {
			var go= vs.gameObject;
			CleanupEmptyComponents(go);
			if(go == null) continue;
			var behaviourScript= vs.gameObject.GetComponent(kiCS_Behaviour);
			if(behaviourScript == null) {
//				Debug.LogWarning("iCanScript: iCS_Behaviour script has been disconnected from=> "+go.name+".  Attempting to reconnect...");
				go.AddComponent(kiCS_Behaviour);
			}
		}
	}
	static void CleanupEmptyComponents(GameObject go) {
		var allComponents= go.GetComponents<Component>();
		foreach(var c in allComponents) {
			if(c == null) {
				Debug.LogWarning("iCanScript: PLEASE REMOVE empty component found on=> "+go.name+".");
			}
		}
	}
}
