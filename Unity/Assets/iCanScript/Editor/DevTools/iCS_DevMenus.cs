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
    // ======================================================================
    // Licensing.
    [MenuItem("DevTools/Machine Finger Print",false,1040)]
    public static void MenuMachineFingerPrint() {
        Debug.Log("Machine Finger Print=> "+iCS_LicenseController.FingerPrint.ToString());
    }
    [MenuItem("DevTools/Remaining Trial Days",false,1041)]
    public static void MenuRemainingTrialDays() {
        Debug.Log("Remaining Trial Days=> "+iCS_LicenseType.RemainingTrialDays);
    }
    [MenuItem("DevTools/Generate User Licenses",false,1042)]
    public static void MenuGenerateUserLicenses() {
        var fingerPrint= iCS_LicenseController.FingerPrint;
        var proLicense     = iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseTypeEnum.Pro, 1234);
        var standardLicense= iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseTypeEnum.Standard, 1234);
        Debug.Log("Finger print=> "+iCS_LicenseController.ToString(fingerPrint));
        Debug.Log("Pro license=> "+iCS_LicenseController.ToString(proLicense));
        Debug.Log("Standard license=> "+iCS_LicenseController.ToString(standardLicense));            
    }
    [MenuItem("DevTools/Set Activation Keys",false,1043)]
    public static void MenuSetActivationKeys() {
        var fingerPrint= iCS_LicenseController.FingerPrint;
        var proActivationKey     = iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseTypeEnum.Pro, 1234);
        var standardActivationKey= iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseTypeEnum.Standard, 5678);
        
        var proDecode= iCS_LicenseController.Xor(fingerPrint, proActivationKey);
        var standardDecode= iCS_LicenseController.Xor(fingerPrint, standardActivationKey);
        int license, date;
        if(iCS_LicenseController.GetSignature(proDecode, out license, out date)) {
            Debug.Log("Pro License key=> "+license+" date=> "+date);
        }
        else {
            Debug.Log("Unable to decode Pro Activation Key=> "+iCS_LicenseController.ToString(proDecode));
        }
        if(iCS_LicenseController.GetSignature(standardDecode, out license, out date)) {
            Debug.Log("Standard License key=> "+license+" date=> "+date);
        }
        else {
            Debug.Log("Unable to decode standard Activation Key=> "+iCS_LicenseController.ToString(standardDecode));
        }
    }
}
