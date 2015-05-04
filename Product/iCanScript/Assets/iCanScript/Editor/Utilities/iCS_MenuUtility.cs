using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using iCanScript.Engine;
using P=Prelude;

public static class iCS_MenuUtility {
    // ----------------------------------------------------------------------
    public static void RemoveVisualScriptFrom(iCS_VisualScriptImp visualScript) {
        // Destroy the given component.
        UnityEngine.Object.DestroyImmediate(visualScript);
    }
}
