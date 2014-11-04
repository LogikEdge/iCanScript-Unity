using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("iCanScript/Preferences...",false,50)]
	public static void MenuPreferences() {
        var editor= EditorWindow.CreateInstance<iCS_PreferencesEditorWindow>();
        editor.ShowUtility();
	}
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("iCanScript/Editors/Visual Editor",false,900)]
	public static void MenuGraphEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_VisualEditorWindow), false, "Visual Editor");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("iCanScript/Editors/Tree View",false,901)]
 	public static void MenuTreeViewEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_TreeViewEditorWindow), false, "Tree View");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("iCanScript/Editors/Library",false,902)]
 	public static void MenuLibraryEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_LibraryEditorWindow), false, "Library");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;

 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("iCanScript/Editors/Instance Wizard",false,903)]
 	public static void MenuInstanceEditor() {
        var editor= EditorWindow.GetWindow(typeof(iCS_InstanceEditorWindow), false, "Instance");
        EditorWindow.DontDestroyOnLoad(editor);
        editor.hideFlags= HideFlags.DontSave;
 	}

}
