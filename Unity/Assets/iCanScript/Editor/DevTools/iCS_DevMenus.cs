using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

using DisruptiveSoftware;

public static class iCS_DevMenu {
    // ======================================================================
    // Snapshot definitions
	const string ScreenShotsFolder= "/../../../ScreenShots";
    
    // ======================================================================
    // Visual Editor Snapshot
	[MenuItem("DevTools/Visual Editor Snapshot",false,1000)]
	public static void MenuVisualEditorSnapshot() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		if(edWindow == null) return;
		iCS_DevToolsConfig.SnapshotWithoutBackground= false;
		iCS_DevToolsConfig.TakeVisualEditorSnapshot= true;
	}
	[MenuItem("DevTools/Visual Editor Snapshot",true,1000)]
	public static bool ValidateMenuVisualEditorSnapshot() {
        return IsVisualEditor;
	}
	[MenuItem("DevTools/Visual Editor Snapshot - No Background",false,1001)]
	public static void MenuVisualEditorSnapshotNoBackground() {
		EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		if(edWindow == null) return;
		iCS_DevToolsConfig.SnapshotWithoutBackground= true;
		iCS_DevToolsConfig.TakeVisualEditorSnapshot= true;
	}
	[MenuItem("DevTools/Visual Editor Snapshot - No Background",true,1001)]
	public static bool ValidateMenuVisualEditorSnapshotNoBackground() {
        return IsVisualEditor;
	}
    [MenuItem("DevTools/Toggle Asset Store Big Image Frame",false,1006)]
    public static void ToggleBigImageFrame() {
        iCS_DevToolsConfig.ShowAssetStoreBigImageFrame = !iCS_DevToolsConfig.ShowAssetStoreBigImageFrame;
        if(iCS_DevToolsConfig.ShowAssetStoreBigImageFrame) {
            iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame= false;
            iCS_DevToolsConfig.ShowBoldImage= true;
        }
        else {
            iCS_DevToolsConfig.ShowBoldImage= false;
        }
    }
    [MenuItem("DevTools/Toggle Asset Store Small Image Frame",false,1007)]
    public static void ToggleSmallImageFrame() {
        iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame= !iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame;
        if(iCS_DevToolsConfig.ShowAssetStoreSmallImageFrame) {
            iCS_DevToolsConfig.ShowAssetStoreBigImageFrame= false;
            iCS_DevToolsConfig.ShowBoldImage= true;
        }
        else {
            iCS_DevToolsConfig.ShowBoldImage= false;
        }
    }
    [MenuItem("DevTools/Toggle Bold Image",false,1008)]
    public static void ToggleBoldImage() {
        iCS_DevToolsConfig.ShowBoldImage= !iCS_DevToolsConfig.ShowBoldImage;
    }
    [MenuItem("DevTools/Toggle Background Image", false, 1009)]
    public static void ToggleBackgroundImage() {
        iCS_DevToolsConfig.UseBackgroundImage= !iCS_DevToolsConfig.UseBackgroundImage;
    }
    public static bool IsVisualEditor {
		get {
            EditorWindow edWindow= iCS_EditorMgr.FindVisualEditorWindow();
		    return edWindow != null;
        }
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
