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
    public static iCS_GraphEditor GetGraphEditor() {
        iCS_GraphEditor editor= EditorWindow.GetWindow(typeof(iCS_GraphEditor), false, "iCanScript") as iCS_GraphEditor;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;                    
        return editor;        
    } 
    public static iCS_ClassWizard GetClassWizardEditor() {
        iCS_ClassWizard editor= EditorWindow.GetWindow(typeof(iCS_ClassWizard), false, "iCS Wizard") as iCS_ClassWizard;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
        return editor;
    }
    public static iCS_HierarchyEditor GetHierarchyEditor() {
        iCS_HierarchyEditor editor= EditorWindow.GetWindow(typeof(iCS_HierarchyEditor), false, "iCS Hierarchy") as iCS_HierarchyEditor;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
        return editor;
    }
    
    // ======================================================================
 	// iCanScript Graph editor Menu.
 	[MenuItem("Window/iCanScript Wizard")]
 	public static void MenuGraphEditor() {
        GetClassWizardEditor();
 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("Window/iCanScript Wizard")]
 	public static void MenuClassWizardEditor() {
        GetClassWizardEditor();
 	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("Window/iCanScript Hierarchy")]
 	public static void MenuHierarchyEditor() {
        GetHierarchyEditor();
 	}
}
