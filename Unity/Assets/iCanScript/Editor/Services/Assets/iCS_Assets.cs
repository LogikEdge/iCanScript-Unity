using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public static class iCS_Assets {
    // ======================================================================
    // Fields
	// ----------------------------------------------------------------------
    static iCS_VisualScript[]   allVisualScripts= new iCS_VisualScript[0];
    
    // ======================================================================
    // Iteration
	// ----------------------------------------------------------------------
    public static void ForEach(Action<iCS_VisualScript> f) {
        foreach(var vs in allVisualScripts) {
            f(vs);
        }
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
        allVisualScripts= Resources.FindObjectsOfTypeAll(typeof(iCS_VisualScript)) as iCS_VisualScript[];
        PerformSanityCheckOnAssets();
    }
    static void ReloadAssets() {
        var newVisualScripts= Resources.FindObjectsOfTypeAll(typeof(iCS_VisualScript));
        allVisualScripts= newVisualScripts as iCS_VisualScript[];
        PerformSanityCheckOnAssets();
    }

    // ======================================================================
    // Member code generation
	// ----------------------------------------------------------------------
    static void PerformSanityCheckOnAssets() {
        // Verify that all Visual Script have their Behaviour.
        foreach(var vs in allVisualScripts) {
        }
    }
}
