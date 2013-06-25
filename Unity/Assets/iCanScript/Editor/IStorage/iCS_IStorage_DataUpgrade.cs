using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
	void PerformEngineDataUpgrade() {
		iCS_Version storageVersion= new iCS_Version(Storage.MajorVersion, Storage.MinorVersion, Storage.BugFixVersion);
		iCS_Version softwareVersion= new iCS_Version(iCS_Config.MajorVersion, iCS_Config.MinorVersion, iCS_Config.BugFixVersion);

		// v0.9.3: Need to convert behaviour module to message 
		if(!storageVersion.IsOlderThen(0,9,3)) {
			ShowUpgradeDialog(softwareVersion);
			SaveCurrentScene();
		}
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
