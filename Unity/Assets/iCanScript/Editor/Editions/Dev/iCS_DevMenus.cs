using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.IO;

using DisruptiveSoftware;

public static class iCS_DevMenus {
    // ======================================================================
    // Snapshot definitions
	const string ScreenShotsFolder= "/../../../ScreenShots";
    
    // ======================================================================
    // Visual Editor Snapshot
	[MenuItem("DevTools/Visual Editor Snapshot",false,1000)]
	public static void MenuVisualEditorSnapshot() {
		EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
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
		EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
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
            EditorWindow edWindow= iCS_EditorController.FindVisualEditorWindow();
		    return edWindow != null;
        }
    }
    // ======================================================================
    // Sanity Check
	[MenuItem("DevTools/Sanity Check Selection",false,1020)]
	public static void MenuSanityCheck() {
		iCS_IStorage storage= iCS_VisualScriptDataController.IStorage;
		if(storage == null) return;
		Debug.Log("iCanScript: Start Sanity Check on: "+storage.iCSMonoBehaviour.name);
		storage.SanityCheck();
		Debug.Log("iCanScript: Completed Sanity Check on: "+storage.iCSMonoBehaviour.name);
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
		iCS_IStorage iStorage= iCS_VisualScriptDataController.IStorage;
		if(iStorage == null) return;
        var selectedObj= iStorage.SelectedObject;
        if(selectedObj == null) return;
        Debug.Log("Layout Info for => "+selectedObj.Name+"\n"+
            "LayoutRect => "+selectedObj.LayoutRect
        );
    }
    // ======================================================================
    // Licensing.
    [MenuItem("DevTools/Generate User Licenses",false,1043)]
    public static void MenuGenerateUserLicenses() {
        var fingerPrint= iCS_LicenseController.FingerPrint;
        var proLicense     = iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseType.Pro, (int)iCS_Config.MajorVersion);
        var standardLicense= iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseType.Standard, (int)iCS_Config.MajorVersion);
        Debug.Log("Finger Print=> "+iCS_LicenseController.ToString(iCS_LicenseController.FingerPrint));
        Debug.Log("Pro license=> "+iCS_LicenseController.ToString(proLicense));
        Debug.Log("Standard license=> "+iCS_LicenseController.ToString(standardLicense));            
    }
    [MenuItem("DevTools/Set Activation Keys",false,1044)]
    public static void MenuSetActivationKeys() {
        var fingerPrint= iCS_LicenseController.FingerPrint;
        var proActivationKey     = iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseType.Pro, (int)iCS_Config.MajorVersion);
        var standardActivationKey= iCS_LicenseController.BuildSignature(fingerPrint, (int)iCS_LicenseType.Standard, (int)iCS_Config.MajorVersion);
        
        var proDecode= iCS_LicenseController.Xor(fingerPrint, proActivationKey);
        var standardDecode= iCS_LicenseController.Xor(fingerPrint, standardActivationKey);
        int license, version;
        if(iCS_LicenseController.GetSignature(proDecode, out license, out version)) {
            Debug.Log("Pro License for version=> "+version+".x");
        }
        else {
            Debug.Log("Unable to decode Pro Activation Key=> "+iCS_LicenseController.ToString(proDecode));
        }
        if(iCS_LicenseController.GetSignature(standardDecode, out license, out version)) {
            Debug.Log("Standard License for version=> "+version+".x");
        }
        else {
            Debug.Log("Unable to decode standard Activation Key=> "+iCS_LicenseController.ToString(standardDecode));
        }
        
        var activationForm= new iCS_ActivationForm();
        activationForm.OnGUI();
    }
    [MenuItem("DevTools/Clear User License", false, 1045)]
    public static void MenuClearLicense() {
        iCS_LicenseController.ResetUserLicense();
		iCS_PreferencesController.TrialLastWarningDate= DateTime.Today.AddDays(-1);
    }
    [MenuItem("DevTools/Environment", false, 1050)]
    public static void ShowEnvironment() {
        Debug.Log("Machine Name=> "+System.Environment.MachineName);
        Debug.Log("User Name=> "+System.Environment.UserName);
    }
    // ======================================================================
    [MenuItem("DevTools/Switch Collision Algorithm", false, 1060)]
    public static void SwitchCollisionAlgorithm() {
        iCS_EditorObject.UseNewCollisionAlgorithm= !iCS_EditorObject.UseNewCollisionAlgorithm;
        if(iCS_EditorObject.UseNewCollisionAlgorithm) {
            Debug.Log("Using new collision algorithm");
        }
        else {
            Debug.Log("Using original collision algorithm");            
        }
    }
    
}
