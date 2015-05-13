using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_MenuUtility {
        // ----------------------------------------------------------------------
        public static void RemoveVisualScriptFrom(iCS_VisualScriptImp visualScript) {
            // Destroy the given component.
            UnityEngine.Object.DestroyImmediate(visualScript);
        }
    }
}
