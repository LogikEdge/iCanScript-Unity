using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public static class iCS_InstallerMgr {
    // =================================================================================
    // Installs all needed components
    // ---------------------------------------------------------------------------------
	static iCS_InstallerMgr() {
        InstallGizmo();
        CreateCodeGenerationFolder();		
	}
    public static void Start() {}
    
    // =================================================================================
    // Installs the iCanScript Gizmo (if not already done).
    // ---------------------------------------------------------------------------------
	static public void InstallGizmo() {
        // Copy the iCanScript gizmo file into the "Gizmos" project folder.
        string assetPath= Application.dataPath;
        if(!Directory.Exists(assetPath+"/"+iCS_Config.GizmosFolder)) {
            Debug.Log("iCanScript: Creating Gizmos folder");
    	    AssetDatabase.CreateFolder("Assets",iCS_Config.GizmosFolder);            
        }
        string gizmoSrc= iCS_Config.ResourcePath+"/"+iCS_EditorStrings.GizmoIcon;
        string gizmoDest= "Assets/Gizmos/iCanScriptGizmo.png";
        if(iCS_Strings.IsEmpty(AssetDatabase.ValidateMoveAsset(gizmoSrc, gizmoDest))) {
    	    AssetDatabase.CopyAsset(gizmoSrc,gizmoDest);
    	    Debug.Log("iCanScript: Copying iCanScriptGizmo.png into the Gizmos folder");            
        }
        // Warning customer of trial period.
        if(iCS_LicenseController.HasDemoLicense) {
            var lastWarningDate= iCS_PreferencesController.TrialLastWarningDate;
            var today= DateTime.Today;
            if(today != lastWarningDate) {
                iCS_PreferencesController.TrialLastWarningDate= today;
                if(iCS_EditionController.IsDemoEdition) {
                    iCS_DemoDialogs.PurchaseDialog();
                }
                if(iCS_EditionController.IsStoreEdition) {
					var userLicense= iCS_PreferencesController.UserLicense;
					if(String.IsNullOrEmpty(userLicense)) {
						var fingerPrint= iCS_LicenseController.FingerPrint;
						var licenseType= iCS_LicenseType.Pro;
						var version= iCS_Config.MajorVersion;
						var license= iCS_LicenseController.BuildSignature(fingerPrint, (int)licenseType, (int)version);
						iCS_PreferencesController.UserLicense= iCS_LicenseController.ToString(license);						
					}
//                    iCS_DemoDialogs.ActivationDialog();
                }
				if(iCS_EditionController.IsDevEdition) {
					var fingerPrint= iCS_LicenseController.FingerPrint;
					var licenseType= iCS_LicenseType.Pro;
					var version= iCS_Config.MajorVersion;
					var license= iCS_LicenseController.BuildSignature(fingerPrint, (int)licenseType, (int)version);
					iCS_PreferencesController.UserLicense= iCS_LicenseController.ToString(license);
				}                
            }
        }	    
	}

    // ================================================================================
    // Create code generation folder.
    // ---------------------------------------------------------------------------------
    static public void CreateCodeGenerationFolder() {
//        string assetsPath= Application.dataPath;
//        var codeGenerationFolder= iCS_PreferencesEditor.CodeGenerationFolder;
//        string codeGenerationFolderPath= assetsPath+"/"+codeGenerationFolder;
//        if(!Directory.Exists(codeGenerationFolderPath)) {
//            Debug.Log(iCS_Config.ProductName+": Creating Code Generation folder");
//            AssetDatabase.CreateFolder("Assets", codeGenerationFolder);
//        }
//        // Generated behaviour folder.
//        var behavioursSubfolder= iCS_PreferencesEditor.BehaviourGenerationSubfolder;
//        var behavioursPath= codeGenerationFolderPath+"/"+behavioursSubfolder;
//        if(!Directory.Exists(behavioursPath)) {
//            AssetDatabase.CreateFolder("Assets/"+codeGenerationFolder, behavioursSubfolder);            
//        }
    }
    
}
