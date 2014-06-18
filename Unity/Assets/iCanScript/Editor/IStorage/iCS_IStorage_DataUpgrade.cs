using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
	void PerformEngineDataUpgrade() {
        Debug.Log("iCanScript: Upgrade verification done.");
        // Special case for v1.1.2 that used a seperate ScriptableObject as
        // visual script data storage
		iCS_Version softwareVersion= new iCS_Version(iCS_Config.MajorVersion, iCS_Config.MinorVersion, iCS_Config.BugFixVersion);
        if(iCSMonoBehaviour.myStorage != null) {
			ShowUpgradeDialog(softwareVersion);
			v1_1_2_Upgrade();
			SaveCurrentScene();            
        }
		iCS_Version storageVersion= new iCS_Version(PersistentStorage.MajorVersion, PersistentStorage.MinorVersion, PersistentStorage.BugFixVersion);
		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
		// v1.1.2: Need to convert behaviour module to message 
		if(!storageVersion.IsOlderThen(1,1,2)) {
            // Already done...
		}
		// Update storage version identifiers
		PersistentStorage.MajorVersion = iCS_Config.MajorVersion;
		PersistentStorage.MinorVersion = iCS_Config.MinorVersion;
		PersistentStorage.BugFixVersion= iCS_Config.BugFixVersion;
	}

    // ----------------------------------------------------------------------
	// Convert module under behaviour to message.
    public void v1_1_2_Upgrade() {
        iCSMonoBehaviour.v1_1_2_Upgrade();
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
