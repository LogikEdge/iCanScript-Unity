using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript/Preferences")]
	public static void MenuPreferences() {
        var editor= EditorWindow.CreateInstance<iCS_PreferencesEditorWindow>();
        editor.ShowUtility();
	}
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript/Visual Editor")]
	public static void MenuGraphEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_VisualEditorWindow), false, "Visual Editor");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("Window/iCanScript/Hierarchy")]
 	public static void MenuHierarchyEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_HierarchyEditorWindow), false, "iCS Hierarchy");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("Window/iCanScript/Library")]
 	public static void MenuLibraryEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_LibraryEditorWindow), false, "iCS Library");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;

 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("Window/iCanScript/Instance Wizard")]
 	public static void MenuInstanceEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_InstanceEditorWindow), false, "iCS Instance");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}

}
