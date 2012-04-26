using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class iCS_EditorWindowMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static List<iCS_EditorWindow>   myWindows= null;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    static iCS_EditorWindowMgr() {
        myWindows= new List<iCS_EditorWindow>();
    }
    
    // =================================================================================
    // Window management
    // ---------------------------------------------------------------------------------
    public static void Add(iCS_EditorWindow window) {
        myWindows.Add(window);
    }
    public static bool Remove(iCS_EditorWindow window) {
        return myWindows.Remove(window);
    }

    // =================================================================================
    // Event distribution.
    // ---------------------------------------------------------------------------------
    public static void Activate(iCS_EditorObject target, iCS_IStorage storage) {
        foreach(var window in myWindows) {
            window.OnActivate(target, storage);
        }
    }
    public static void Deactivate() {
        foreach(var window in myWindows) {
            window.OnDeactivate();
        }
     }

    // ======================================================================
 	// iCanScript ClassWizard Window Menu.
 	[MenuItem("Window/iCanScript Wizard")]
 	public static void ShowiCanScriptClassWizard() {
         iCS_ClassWizardProxy window= EditorWindow.GetWindow(typeof(iCS_ClassWizardProxy), false, "iCS Wizard") as iCS_ClassWizardProxy;
         window.hideFlags= HideFlags.DontSave;
 	}
    // ======================================================================
 	// iCanScript Hierarchy Window Menu.
 	[MenuItem("Window/iCanScript Hierarchy")]
 	public static void ShowiCanScriptHierarchy() {
         iCS_HierarchyEditorProxy window= EditorWindow.GetWindow(typeof(iCS_HierarchyEditorProxy), false, "iCS Hierarchy") as iCS_HierarchyEditorProxy;
         window.hideFlags= HideFlags.DontSave;
 	}
}
