using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using P=Prelude;

public static class iCS_MenuUtility {
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
        bool hasVisualScript= gameObject.GetComponent<iCS_VisualScriptImp>() != null;
        if(hasVisualScript) {
			// Remove duplicate Behaviours.
			MonoBehaviour iCSBehaviour= null;
            var monoBehaviours= gameObject.GetComponents<iCS_BehaviourProxy>();
			foreach(var mb in monoBehaviours) {
				if(iCSBehaviour == null) {
					iCSBehaviour= mb;
				} else {
	                Object.DestroyImmediate(mb);						
				}
			}
            // Add behaviour if not already present.
            if(iCSBehaviour == null) {
                gameObject.AddComponent(behaviourClassName);
            }
        } else {
            // Remove behaviour.
            var behaviourComponent= gameObject.GetComponent<iCS_BehaviourProxy>();
            if(behaviourComponent != null) {
                Object.DestroyImmediate(behaviourComponent);
            }
        }
    }
}
