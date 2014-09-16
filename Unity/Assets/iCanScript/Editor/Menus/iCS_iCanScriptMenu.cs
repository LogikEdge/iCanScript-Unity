using UnityEngine;
using UnityEditor;

public static class iCS_iCanScriptMenu {
    // ======================================================================
	// Create a behavior to selected game object.
	[MenuItem("Edit/", false, 100)]
	[MenuItem("Edit/iCanScript/Create Visual Script", false, 101)]
	public static void CreateVisualScript() {
        var gameObject= Selection.activeGameObject;
        if(gameObject == null) return;
		var visualScript= gameObject.GetComponent<iCS_VisualScriptImp>();
		if(visualScript == null) {
    		gameObject.AddComponent("iCS_VisualScript");
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
    [MenuItem("Edit/iCanScript/Center Visual Script",false,121)]
    public static void FocusOnVisualScript() {
        iCS_VisualEditor visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor != null) visualEditor.CenterAndScaleOnRoot();
    }
    [MenuItem("Edit/iCanScript/Center Visual Script",true,121)]
    public static bool ValidateFocusOnVisualScript() {
        var focusedWindow= EditorWindow.focusedWindow;
        if(focusedWindow == null) return false;
        if((focusedWindow as iCS_VisualEditor) == null) return false;
        return true;
    }
    [MenuItem("Edit/iCanScript/Center Selected",false,122)]
    public static void FocusOnSelected() {
        iCS_VisualEditor graphEditor= iCS_EditorController.FindVisualEditor();
        if(graphEditor != null) graphEditor.CenterAndScaleOnSelected();
    }
    [MenuItem("Edit/iCanScript/Center Selected",true,122)]
    public static bool ValidateFocusOnSelected() {
        var focusedWindow= EditorWindow.focusedWindow;
        if(focusedWindow == null) return false;
        if((focusedWindow as iCS_VisualEditor) == null) return false;
        return true;
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
        var storage= new iCS_VisualScriptData(monoBehaviour);
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
        var initialPath= Application.dataPath;
        var path= EditorUtility.OpenFilePanel("Import Visual Script", initialPath, "json");
        if(string.IsNullOrEmpty(path)) return;
        var tmpVsd= new iCS_VisualScriptData();
        if(!iCS_VisualScriptImportExport.Import(tmpVsd, path)) {
            Debug.LogError("iCanScript: Import failure. The selected visual script was not modified.");
        }
        tmpVsd.CopyTo(monoBehaviour);
        Debug.Log("iCanScript: Import completed => "+path);
        // Attempt to reload and relayout if the selection is visible in the visual editor.
        var visualEditor= iCS_EditorController.FindVisualEditor();
        if(visualEditor == null) return;
        if(visualEditor.IStorage.iCSMonoBehaviour != monoBehaviour) return;
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
    // Tutorials Access
    [MenuItem("Help/iCanScript/Space Shooter Tutorial on YouTube",false,70)]
    public static void SpaceShooterTutorialPlaylist() {        
        Application.OpenURL("http://www.youtube.com/playlist?list=PLbRggLFpBWUQA7RgHO_eIZvl56W4A53Vj");
    }
    // ======================================================================
    // Purchase
    [MenuItem("Help/iCanScript/Check for Updates...",false,91)]
    public static void CheckForUpdate() {
		iCS_SoftwareUpdateController.ManualUpdateVerification();
    }
}
