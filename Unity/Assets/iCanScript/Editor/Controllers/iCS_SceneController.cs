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
        iCS_SystemEvents.OnSceneChanged    = OnSceneChanged;
        iCS_SystemEvents.OnHierarchyChanged= RefreshCache;
        iCS_SystemEvents.OnProjectChanged  = RefreshCache;
        // Events to refresh visual script information.
        iCS_SystemEvents.OnVisualScriptUndo                = OnVisualScriptUndo;                
        iCS_SystemEvents.OnVisualScriptElementAdded        = OnVisualScriptElementAdded;        
        iCS_SystemEvents.OnVisualScriptElementWillBeRemoved= OnVisualScriptElementWillBeRemoved;
        iCS_SystemEvents.OnVisualScriptElementNameChanged  = OnVisualScriptElementNameChanged;  
	}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {}

    // ======================================================================
    // Global Scene Queries
    // ----------------------------------------------------------------------
    /// Returns all active VisualScript included in the current scene.
    static iCS_VisualScriptImp[] GetVisualScriptsInScene() {
        var allVisualScripts= UnityEngine.Object.FindObjectsOfType(typeof(iCS_VisualScriptImp));
        return Array.ConvertAll( allVisualScripts, e=> e as iCS_VisualScriptImp );
    }

    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
    static iCS_VisualScriptImp[] GetVisualScriptsReferencedByScene() {
        var sceneVisualScripts= GetVisualScriptsInScene();
        var result= new iCS_VisualScriptImp[0];
        P.forEach(
            vs=> {
                result= P.append(result, GetVisualScriptsReferencedBy(vs));
            },
            sceneVisualScripts
        );
        result= P.removeDuplicates(result);
        return result;
    }
    
    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
    static iCS_VisualScriptImp[] GetVisualScriptsInOrReferencedByScene() {
        var sceneVisualScripts= GetVisualScriptsInScene();
        var result= sceneVisualScripts;
        P.forEach(
            vs=> {
                result= P.append(result, GetVisualScriptsReferencedBy(vs));
            },
            sceneVisualScripts
        );
        result= P.removeDuplicates(result);
        return result;
    }
    
    // ----------------------------------------------------------------------
    /// Returns Visual Scripts referenced by the given Visual Script.
    static iCS_VisualScriptImp[] GetVisualScriptsReferencedBy(iCS_VisualScriptImp vs) {
        var visualScripts= P.map(o=> o as iCS_VisualScriptImp, P.filter(o=> o is iCS_VisualScriptImp, vs.UnityObjects));
        var gameObjects  = P.map(o=> o as GameObject         , P.filter(o=> o is GameObject         , vs.UnityObjects));
        P.forEach(
            go=> {
                var vsFromGameObject= go.GetComponent(typeof(iCS_VisualScriptImp)) as iCS_VisualScriptImp;
                if(vsFromGameObject != null) {
                    visualScripts.Add(vsFromGameObject);
                }
            },
            gameObjects
        );
        visualScripts= P.removeDuplicates(visualScripts);
        return visualScripts.ToArray();
    }
    
    // ======================================================================
    // Update scene content changed
    // ----------------------------------------------------------------------
    static void OnSceneChanged() {
        RefreshCache();
    }
    static void RefreshCache() {
        ourVisualScriptsInScene              = GetVisualScriptsInScene();
        ourVisualScriptsReferencesByScene    = GetVisualScriptsReferencedByScene();
        ourVisualScriptsInOrReferencesByScene= GetVisualScriptsInOrReferencedByScene();
        
#if DEBUG
        Debug.Log("Scene as changed =>"+EditorApplication.currentScene);
#endif
    }

    // ======================================================================
    // Update visual script content changed
    // ----------------------------------------------------------------------
    static void OnVisualScriptUndo(iCS_IStorage iStorage) {
#if DEBUG
        Debug.Log("Visual Script undo=> "+iStorage.VisualScript.name);
#endif
    }
    static void OnVisualScriptElementAdded(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element added=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }
    static void OnVisualScriptElementWillBeRemoved(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element will be removed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }
    static void OnVisualScriptElementNameChanged(iCS_IStorage iStorage, iCS_EditorObject element) {
#if DEBUG
        Debug.Log("Visual Script element name changed=> "+iStorage.VisualScript.name+"."+element.Name);
#endif
    }
}
