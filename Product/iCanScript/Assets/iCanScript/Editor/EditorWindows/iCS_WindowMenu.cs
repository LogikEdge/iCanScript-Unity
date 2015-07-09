using UnityEngine;
using UnityEditor;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_WindowMenu {
        // ======================================================================
    	// iCanScript Graph editor.
    	[MenuItem("iCanScript/Preferences...",false,11)]
    	public static void MenuPreferences() {
            var editor= EditorWindow.CreateInstance<iCS_GlobalSettingsEditorWindow>();
            editor.ShowUtility();
    	}
        // ======================================================================
    	// Quick Open Visual Editor.
    	[MenuItem("iCanScript/Open iCanScript",false,12)]
    	public static void OpeniCanScript() {
            MenuGraphEditor();
            MenuLibraryEditor();
    	}
        // ======================================================================
    	// iCanScript Graph editor.
    	[MenuItem("iCanScript/Editors/Visual Editor",false,900)]
    	public static void MenuGraphEditor() {
            // -- Adjust default size when first opened --
            var width= 1280f;
            var height= 800f;
            var r= new Rect(0.2f*width, 0.2f*height, 0.6f*width, 0.6f*height);
            var editor= EditorWindow.GetWindowWithRect(typeof(iCS_VisualEditorWindow), r, false, "Visual Editor");
            editor.maxSize= new Vector2(5120f, 2880f);
            editor.minSize= new Vector2(50f,50f);
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
            // -- Adjust default size when first opened --        // -- Adjust default size when first opened --
            var width= 1280f;
            var height= 800f;
            var r= new Rect(0.2f*width, 0.2f*height, 0.2f*width, 0.6f*height);
            var editor= EditorWindow.GetWindowWithRect(typeof(iCS_LibraryEditorWindow), r, false, "Library");
            editor.maxSize= new Vector2(5120f, 2880f);
            editor.minSize= new Vector2(50f,50f);
            EditorWindow.DontDestroyOnLoad(editor);
            editor.hideFlags= HideFlags.DontSave;
     	}

    	// %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    	// TODO: TO BE OBSOLETED ONCE THE VIDEOS ARE REDONE.
        // ======================================================================
    	// iCanScript Graph editor.
    	[MenuItem("Window/iCanScript/Preferences...")]
    	public static void OldMenuPreferences() {
    		MenuPreferences();
    	}
        // ======================================================================
    	// iCanScript Graph editor.
    	[MenuItem("Window/iCanScript/Visual Editor")]
    	public static void OldMenuGraphEditor() {
    		MenuGraphEditor();
    	}
        // ======================================================================
     	// iCanScript Hierarchy editor Menu.
     	[MenuItem("Window/iCanScript/Tree View")]
     	public static void OldMenuTreeViewEditor() {
    		MenuTreeViewEditor();
     	}
        // ======================================================================
     	// iCanScript Project editor Menu.
     	[MenuItem("Window/iCanScript/Library")]
     	public static void OldMenuLibraryEditor() {
    		MenuLibraryEditor();
     	}

    }

}
