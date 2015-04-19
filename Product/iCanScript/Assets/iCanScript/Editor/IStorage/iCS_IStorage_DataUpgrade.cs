using UnityEngine;
using UnityEditor;
using System.Collections;
using iCanScript.Engine;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
    /// Performs engine data upgarde before generating the editor data.
	void PerformEngineDataUpgrade() {
        bool isUpgraded= false;
        
        // PRE-PROCESSING ====================================================
        // Use this are to perform pre-processing data conversion.
		iCS_Version softwareVersion= iCS_Version.Current;

		// Extract the version of the storage.
		iCS_Version storageVersion= new iCS_Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
        // -- Warn the user that an upgrade toke place --
        if(isUpgraded) {
			ShowUpgradeDialog(softwareVersion);
        }
	}
    // ----------------------------------------------------------------------
    /// Performs editor data upgarde.
	void PerformEditorDataUpgrade() {
        bool isUpgraded= false;
        
        // PRE-PROCESSING ====================================================
        // Use this are to perform pre-processing data conversion.
		iCS_Version softwareVersion= iCS_Version.Current;

		// Extract the version of the storage.
		iCS_Version storageVersion= new iCS_Version(EngineStorage.MajorVersion, EngineStorage.MinorVersion, EngineStorage.BugFixVersion);
		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
        // -- Before v2.0.6 --
		if(storageVersion.IsOlderThen(2,0,6)) {
            // -- Scan for functions and properties nodes to add a self port --
            ForEach(
                o=> {
                    if(o.IsKindOfFunction || o.IsInstanceNode) {
                        if(GetSelfPort(o) == null) {
                            CreateSelfPort(o);
                            isUpgraded= true;
                        }
                    }
                }
            );
        }
        // -- Warn the user that an upgrade toke place --
        if(isUpgraded) {
			ShowUpgradeDialog(softwareVersion);
        }
		// -- Update storage version identifiers --
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
