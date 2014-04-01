using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public static class iCS_DevMenu {
    // ======================================================================
    // Snapshot definitions
	const string ScreenShotsFolder= "/../../../ScreenShots";

    // ======================================================================
    [MenuItem("DevTools/Export Storage",false,900)]
    public static void ExportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return;
        var go= transform.gameObject;
        if(go == null) return;
        var monoBehaviour= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
        if(monoBehaviour == null) return;
        var storage= monoBehaviour.Storage;
        if(storage == null) return;
        var archive= storage.BuildArchive();
        Debug.Log(archive);    
    }
    [MenuItem("DevTools/Export Storage",true,900)]
    public static bool ValidateExportStorage() {
        var transform= Selection.activeTransform;
        if(transform == null) return false;
        var go= transform.gameObject;
        if(go == null) return false;
        var visualEditor= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_VisualScriptImp;
        return visualEditor != null;
    }
    
    // ======================================================================
    // Visual Editor Snapshot
	[MenuItem("DevTools/Visual Editor Snapshot",false,1000)]
	public static void MenuVisualEditorSnapshot() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		if(edWindow == null) return;
		iCS_DevToolsConfig.takeVisualEditorSnapshot= true;
	}
	[MenuItem("DevTools/Visual Editor Snapshot",true,1000)]
	public static bool ValidateMenuVisualEditorSnapshot() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		return edWindow != null;
	}
	[MenuItem("DevTools/Visual Editor Snapshot - No Background",false,1000)]
	public static void MenuVisualEditorSnapshotNoBackground() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		if(edWindow == null) return;
		iCS_DevToolsConfig.framesWithoutBackground= 2;
		iCS_DevToolsConfig.takeVisualEditorSnapshot= true;
	}
	[MenuItem("DevTools/Visual Editor Snapshot - No Background",true,1000)]
	public static bool ValidateMenuVisualEditorSnapshotNoBackground() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		return edWindow != null;
	}
    // ======================================================================
    // Sanity Check
	[MenuItem("DevTools/Sanity Check Selection",false,1020)]
	public static void MenuSanityCheck() {
		iCS_IStorage storage= iCS_StorageMgr.IStorage;
		if(storage == null) return;
		Debug.Log("iCanScript: Start Sanity Check on: "+storage.Storage.name);
		storage.SanityCheck();
		Debug.Log("iCanScript: Completed Sanity Check on: "+storage.Storage.name);
	}
    // ======================================================================
    // Trigger Periodic Software Update Verification
	[MenuItem("DevTools/Invoke Periodic Software Update Verification",false,1021)]
	public static void MenuPeriodicSoftwareUpdateVerification() {
		iCS_SoftwareUpdateController.PeriodicUpdateVerification();
	}	
    // ======================================================================
    // Extract some info.
	[MenuItem("DevTools/Get Layout Info",false,1022)]
	public static void MenuGetLayoutInfo() {
		iCS_IStorage iStorage= iCS_StorageMgr.IStorage;
		if(iStorage == null) return;
        var selectedObj= iStorage.SelectedObject;
        if(selectedObj == null) return;
        Debug.Log("Layout Info for => "+selectedObj.Name+"\n"+
            "LayoutRect => "+selectedObj.LayoutRect
        );
    }
}
