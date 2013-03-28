using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript/Preferences")]
	public static void MenuPreferences() {
        iCS_PreferencesEditorWindow editor= EditorWindow.CreateInstance<iCS_PreferencesEditorWindow>();
        editor.ShowUtility();
	}
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript/Visual Editor")]
	public static void MenuGraphEditor() {
        iCS_VisualEditorWindow editor= EditorWindow.GetWindow(typeof(iCS_VisualEditorWindow), false, "Visual Editor") as iCS_VisualEditorWindow;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("Window/iCanScript/Hierarchy")]
 	public static void MenuHierarchyEditor() {
        iCS_HierarchyEditorWindow editor= EditorWindow.GetWindow(typeof(iCS_HierarchyEditorWindow), false, "iCS Hierarchy") as iCS_HierarchyEditorWindow;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("Window/iCanScript/Library")]
 	public static void MenuLibraryEditor() {
        iCS_LibraryEditorWindow editor= EditorWindow.GetWindow(typeof(iCS_LibraryEditorWindow), false, "iCS Library") as iCS_LibraryEditorWindow;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;

 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("Window/iCanScript/Instance Wizard")]
 	public static void MenuInstanceEditor() {
        iCS_InstanceEditorWindow editor= EditorWindow.GetWindow(typeof(iCS_InstanceEditorWindow), false, "iCS Instance") as iCS_InstanceEditorWindow;
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}

}
