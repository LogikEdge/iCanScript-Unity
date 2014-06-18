using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public static class iCS_MenuUtility {

    // ----------------------------------------------------------------------
    public static iCS_VisualScriptImp InstallVisualScriptOn(GameObject gameObject) {
        // Build new visual script with behviour node
		var visualScript= gameObject.AddComponent("iCS_VisualScript") as iCS_VisualScriptImp;
        iCS_IStorage iStorage= new iCS_IStorage(visualScript);
        iStorage.CreateBehaviour(gameObject.name);
        iStorage= null;

        // Install behaviour if not already present.
        UpdateBehaviourComponent(gameObject);
        return visualScript;
    }
    // ----------------------------------------------------------------------
    public static void RemoveVisualScriptFrom(iCS_VisualScriptImp visualScript) {
        // Destroy the given component.
        var gameObject= visualScript.gameObject;
        Object.DestroyImmediate(visualScript);

        // Also remove the Behaviour class if no visual script remain on this object.
        UpdateBehaviourComponent(gameObject);
    }
    // ----------------------------------------------------------------------
    public static void UpdateBehaviourComponent(GameObject gameObject) {
        if(gameObject == null) return;
        var behaviourClassName= iCS_EditorStrings.DefaultBehaviourClassName;
        bool hasVisualScript= gameObject.GetComponent("iCS_VisualScript") != null;
        if(hasVisualScript) {
			// Remove duplicate Behaviours.
			MonoBehaviour iCSBehaviour= null;
            var monoBehaviours= gameObject.GetComponents<MonoBehaviour>();
			foreach(var mb in monoBehaviours) {
				if(mb.GetType().Name == behaviourClassName) {
					if(iCSBehaviour == null) {
						iCSBehaviour= mb;
					} else {
		                Object.DestroyImmediate(mb);						
					}
				}
			}
            // Add behaviour if not already present.
            if(iCSBehaviour == null) {
                gameObject.AddComponent(behaviourClassName);
            }
        } else {
            // Remove behaviour.
            var behaviourComponent= gameObject.GetComponent(behaviourClassName);
            if(behaviourComponent != null) {
                Object.DestroyImmediate(behaviourComponent);
            }
        }
    }
}
