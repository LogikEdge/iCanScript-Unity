using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_SceneController {
    // ======================================================================
    // Common Controller activation/deactivation
    // ----------------------------------------------------------------------
	static iCS_SceneController() {}
    
    /// Start the application controller.
	public static void Start() {}
    /// Shutdowns the application controller.
    public static void Shutdown() {}

    // ======================================================================
    // Global Scene Queries
    // ----------------------------------------------------------------------
    /// Returns all active VisualScript included in the current scene.
    public static iCS_VisualScriptImp[] GetVisualScriptsInScene() {
        var allVisualScripts= UnityEngine.Object.FindObjectsOfType(typeof(iCS_VisualScriptImp));
        return Array.ConvertAll( allVisualScripts, e=> e as iCS_VisualScriptImp );
    }

    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
    public static iCS_VisualScriptImp[] GetVisualScriptsReferencedByScene() {
        var sceneVisualScripts= GetVisualScriptsInScene();
        List<iCS_VisualScriptImp> result= new List<iCS_VisualScriptImp>(sceneVisualScripts);
        P.forEach(
            vs=> {
                result.AddRange(GetReferencedVisualsScriptBy(vs));
            },
            sceneVisualScripts
        );
        return result.ToArray();
    }
    
    // ----------------------------------------------------------------------
    /// Returns all Visual Scripts referenced in the current scene.
    public static iCS_VisualScriptImp[] GetVisualScriptsInOrReferencesByScene() {
        return new iCS_VisualScriptImp[0];
    }
    
    // ----------------------------------------------------------------------
    /// Returns Visual Scripts referenced by the given Visual Script.
    public static iCS_VisualScriptImp[] GetReferencedVisualsScriptBy(iCS_VisualScriptImp vs) {
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
        return visualScripts.ToArray();
    }
}
