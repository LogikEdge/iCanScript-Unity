using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Engine;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
	void PerformEngineDataUpgrade() {
        bool isUpgraded= false;
        
        // PRE-PROCESSING ====================================================
        // Use this are to perform pre-processing data conversion.
		iCS_Version softwareVersion= iCS_Version.Current;

		// Extract the version of the storage.
		iCS_Version storageVersion= new iCS_Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
        // POST-PROCESING ====================================================
        // v1.2.0 Needs to convert "this" port name to "type instance"
		if(storageVersion.IsOlderThen(1,2,0)) {
            foreach(var obj in EngineStorage.EngineObjects) {
                if(obj.IsDataPort && obj.RawName == "this" && obj.RuntimeType != null) {
                    obj.RawName= GetInstancePortName(obj.RuntimeType);
                    isUpgraded= true;
                }         
            }
        }
        if(storageVersion.IsOlderOrEqualTo(1,2,3)) {
        }
        // Warn the user that an upgrade toke place.
        if(isUpgraded) {
			ShowUpgradeDialog(softwareVersion);
        }
		// Update storage version identifiers
		EngineStorage.MajorVersion = iCS_Config.MajorVersion;
		EngineStorage.MinorVersion = iCS_Config.MinorVersion;
		EngineStorage.BugFixVersion= iCS_Config.BugFixVersion;
	}

    // ----------------------------------------------------------------------
	void ShowUpgradeDialog(iCS_Version softwareVersion) {
		EditorUtility.DisplayDialog("iCanScript Data Upgrade Required", "Your visual scripts were created with an earlier version of iCanScript.\n\nAn upgrade to v"+softwareVersion.ToString()+" will be performed in memory.\nPlease save your scenes to complete the upgrade.", "Ok");
	}
    // ----------------------------------------------------------------------
	void SaveCurrentScene() {
		EditorApplication.SaveCurrentSceneIfUserWantsTo();				
	}
	
}
