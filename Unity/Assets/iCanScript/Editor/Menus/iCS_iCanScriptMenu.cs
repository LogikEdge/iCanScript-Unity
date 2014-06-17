using UnityEngine;
using UnityEditor;

public static class iCS_iCanScriptMenu {
    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("Edit/", false, 100)]
	[MenuItem("Edit/iCanScript/Create Visual Script", false, 101)]
	public static void CreateVisualScript() {
		var visualScript = Selection.activeGameObject.GetComponent<iCS_VisualScriptImp>();
		if(visualScript == null) {
		    iCS_MenuUtility.InstallVisualScriptOn(Selection.activeGameObject);
		}
	}
	[MenuItem("Edit/iCanScript/Create Visual Script", true, 101)]
	public static bool ValidateCreateVisualScript() {
		if(Selection.activeGameObject == null) return false;
		var visualScript = Selection.activeGameObject.GetComponent<iCS_VisualScriptImp>();
		return visualScript == null;
	}

    // ======================================================================
    // Navigation
    [MenuItem("Edit/iCanScript/Center Visual Script #f",false,121)]
    public static void FocusOnVisualScript() {
        iCS_VisualEditor visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null) visualEditor.CenterAndScaleOnRoot();
    }
    [MenuItem("Edit/iCanScript/Center Selected _f",false,122)]
    public static void FocusOnSelected() {
        iCS_VisualEditor graphEditor= iCS_EditorController.FindVisualEditor();
        if(graphEditor != null) graphEditor.CenterAndScaleOnSelected();
    }
    // ======================================================================
    // Export Storage
    [MenuItem("Edit/iCanScript/Export...",false,900)]
    public static void ExportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return;
        var go= transform.gameObject;
        if(go == null) return;
        var monoBehaviour= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
        if(monoBehaviour == null) return;
        var storage= monoBehaviour.Storage;
        if(storage == null) return;
        var initialPath= Application.dataPath;
        var path= EditorUtility.SaveFilePanel("Export Visual Script", initialPath, monoBehaviour.name+".json", "json");
        if(string.IsNullOrEmpty(path)) return;
        iCS_VisualScriptImportExport.Export(storage, path);
        Debug.Log("iCanScript: Export completed => "+path);
    } 
    [MenuItem("Edit/iCanScript/Export...",true,900)]
    public static bool ValidateExportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return false;
        var go= transform.gameObject;
        if(go == null) return false;
        var visualEditor= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
        return visualEditor != null;
    }
    // ======================================================================
    // Import Storage
    [MenuItem("Edit/iCanScript/Import...",false,901)]
    public static void ImportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return;
        var go= transform.gameObject;
        if(go == null) return;
        var monoBehaviour= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
        if(monoBehaviour == null) return;
        iCS_StorageImp storage= monoBehaviour.Storage;
        if(storage == null) return;
        var initialPath= Application.dataPath;
        var path= EditorUtility.OpenFilePanel("Import Visual Script", initialPath, "json");
        if(string.IsNullOrEmpty(path)) return;
        iCS_VisualScriptImportExport.Import(storage, path);
        Debug.Log("iCanScript: Import completed => "+path);
        // Attempt to reload and relayout if the selection is visible in the visual editor.
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor == null) return;
        if(visualEditor.IStorage.PersistentStorage != storage) return;
        visualEditor.SendEvent(EditorGUIUtility.CommandEvent("ReloadStorage"));
    }
    [MenuItem("Edit/iCanScript/Import...",true,901)]
    public static bool ValidateImportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return false;
        var go= transform.gameObject;
        if(go == null) return false;
        var visualEditor= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
        return visualEditor != null;
    }
    // ======================================================================
    // Support Access
    [MenuItem("Help/iCanScript/About...",false,10)]
    public static void About() {
        var aboutDialog= iCS_AboutDialog.CreateInstance<iCS_AboutDialog>();
        aboutDialog.ShowUtility();
    }
    [MenuItem("Help/iCanScript/Home Page",false,51)]
    public static void HomePage() {
        Application.OpenURL("http://www.icanscript.com");
    }
    [MenuItem("Help/iCanScript/Helpdesk and Community",false,52)]
    public static void Helpdesk() {
        Application.OpenURL("http://helpdesk.icanscript.com");
    }
    [MenuItem("Help/iCanScript/Submit a ticket",false,53)]
    public static void ReleaseNotes() {
        Application.OpenURL("http://helpdesk.icanscript.com/support/tickets/new");
    }
    // ======================================================================
    // Purchase
    [MenuItem("Help/iCanScript/Check for Updates...",false,72)]
    public static void CheckForUpdate() {
		iCS_SoftwareUpdateController.ManualUpdateVerification();
    }
}
