using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using P=Prelude;

public static class iCS_MenuUtility {
    // ----------------------------------------------------------------------
    public static void RemoveVisualScriptFrom(iCS_VisualScriptImp visualScript) {
        // Destroy the given component.
        var gameObject= visualScript.gameObject;
        UnityEngine.Object.DestroyImmediate(visualScript);

        // Also remove the Behaviour class if no visual script remain on this object.
        UpdateBehaviourComponent(gameObject);
    }
    // ----------------------------------------------------------------------
    public static void UpdateBehaviourComponent(GameObject gameObject) {
        if(gameObject == null) return;
        bool hasVisualScript= gameObject.GetComponent<iCS_VisualScriptImp>() != null;
        if(hasVisualScript) {
			// Remove duplicate Behaviours.
			MonoBehaviour iCSBehaviour= null;
            var monoBehaviours= gameObject.GetComponents<iCS_BehaviourProxy>();
			foreach(var mb in monoBehaviours) {
				if(iCSBehaviour == null) {
					iCSBehaviour= mb;
				} else {
	                UnityEngine.Object.DestroyImmediate(mb);						
				}
			}
            // Add behaviour if not already present.
            if(iCSBehaviour == null) {
                iCS_MenuInterface.AddBehaviour(gameObject);
            }
        } else {
            // Remove behaviour.
            var behaviourComponent= gameObject.GetComponent<iCS_BehaviourProxy>();
            if(behaviourComponent != null) {
                UnityEngine.Object.DestroyImmediate(behaviourComponent);
            }
        }
    }
}
