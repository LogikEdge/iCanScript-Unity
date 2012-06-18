using UnityEngine;
using UnityEditor;

public static class iCS_WindowMenu {
    // ======================================================================
	// iCanScript Graph editor.
	[MenuItem("Window/iCanScript Graph Editor")]
	public static void MeuGraphEditor() {
        iCS_EditorMgr.GetGraphEditor();
	}
    // ======================================================================
 	// iCanScript Hierarchy editor Menu.
 	[MenuItem("Window/iCanScript Hierarchy")]
 	public static void MenuHierarchyEditor() {
        iCS_EditorMgr.GetHierarchyEditor();
 	}
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("Window/iCanScript Library")]
 	public static void MenuLibraryEditor() {
        iCS_EditorMgr.GetLibraryEditor();
 	}
    // ======================================================================
 	// iCanScript ClassWizard editor Menu.
 	[MenuItem("Window/iCanScript Wizard")]
 	public static void MenuClassWizardEditor() {
        iCS_EditorMgr.GetClassWizardEditor();
 	}
}
