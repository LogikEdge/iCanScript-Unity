using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// Quick Open Visual Editor.
	[MenuItem("iCanScript/Open iCanScript",false,20)]
	public static void OpeniCanScript() {
        MenuGraphEditor();
        MenuLibraryEditor();
        MenuInstanceEditor();
	}
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
        // -- Adjust default size when first opened --
        var width= 1280;
        var height= 800;
        var r= new Rect(0.2f*width, 0.1f*height, 0.6f*width, 0.6f*height);
        var editor= EditorWindow.GetWindowWithRect(typeof(iCS_VisualEditorWindow), r, false, "Visual Editor");
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
        // -- Adjust default size when first opened --
        var width= 1280;
        var height= 800;
        var r= new Rect(0.2f*width, 0.1f*height, 0.2f*width, 0.6f*height);
        var editor= EditorWindow.GetWindowWithRect(typeof(iCS_LibraryEditorWindow), r, false, "Library");
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
