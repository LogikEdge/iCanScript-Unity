using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class iCS_EditorMgr {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
    static List<iCS_EditorWindow>   myWindows       = null;
	static int						myModificationId= 0;
	static bool                     myIsPlaying     = false;
    
    // =================================================================================
    // Initialization
    // ---------------------------------------------------------------------------------
    static iCS_EditorMgr() {
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
	public static void Update() {
		iCS_StorageMgr.Update();
		bool isPlaying= Application.isPlaying;
		Prelude.filterWith(
			w=> w.IStorage != iCS_StorageMgr.IStorage || myIsPlaying != isPlaying,
			w=> { w.IStorage= iCS_StorageMgr.IStorage; w.OnStorageChange(); w.Repaint(); },
			myWindows);
		Prelude.filterWith(
			w=> w.SelectedObject != iCS_StorageMgr.SelectedObject || myIsPlaying != isPlaying,
			w=> { w.SelectedObject= iCS_StorageMgr.SelectedObject; w.OnSelectedObjectChange(); w.Repaint(); },
			myWindows);
		if(iCS_StorageMgr.IStorage != null && myModificationId != iCS_StorageMgr.IStorage.ModificationId) {
			myModificationId= iCS_StorageMgr.IStorage.ModificationId;
			Prelude.forEach(w=> w.Repaint(), myWindows);
		}
		myIsPlaying= isPlaying;
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
    public static iCS_ProjectEditor GetProjectEditor() {
        iCS_ProjectEditor editor= EditorWindow.GetWindow(typeof(iCS_ProjectEditor), false, "iCS Project") as iCS_ProjectEditor;
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
    // ======================================================================
 	// iCanScript Project editor Menu.
 	[MenuItem("Window/iCanScript Project")]
 	public static void MenuProjectEditor() {
        GetProjectEditor();
 	}
}
