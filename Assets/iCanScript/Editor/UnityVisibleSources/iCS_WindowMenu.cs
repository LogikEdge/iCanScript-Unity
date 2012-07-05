using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript/Graph")]
	public static void MeuGraphEditor() {
        iCS_GraphEditorProxy editor= EditorWindow.GetWindow(typeof(iCS_GraphEditorProxy), false, "iCS Graph") as iCS_GraphEditorProxy;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("Window/iCanScript/Hierarchy")]
 	public static void MenuHierarchyEditor() {
        iCS_HierarchyEditorProxy editor= EditorWindow.GetWindow(typeof(iCS_HierarchyEditorProxy), false, "iCS Hierarchy") as iCS_HierarchyEditorProxy;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("Window/iCanScript/Library")]
 	public static void MenuLibraryEditor() {
        iCS_LibraryEditorProxy editor= EditorWindow.GetWindow(typeof(iCS_LibraryEditorProxy), false, "iCS Library") as iCS_LibraryEditorProxy;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;

 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("Window/iCanScript/Instance Wizard")]
 	public static void MenuInstanceEditor() {
        iCS_InstanceEditorProxy editor= EditorWindow.GetWindow(typeof(iCS_InstanceEditorProxy), false, "iCS Instance") as iCS_InstanceEditorProxy;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}
}
