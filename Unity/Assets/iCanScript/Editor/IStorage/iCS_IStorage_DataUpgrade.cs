using UnityEngine;
using UnityEditor;
using System.Collections;

public partial class iCS_IStorage {
    // ----------------------------------------------------------------------
	void PerformEngineDataUpgrade() {
        bool isUpgraded= false;
        
        // PRE-PROCESSING ====================================================
        // v1.1.2: used a seperate ScriptableObject for storage
		iCS_Version softwareVersion= iCS_Version.Current;
        if(iCSMonoBehaviour.myStorage != null) {
			ShowUpgradeDialog(softwareVersion); 
			v1_1_2_Upgrade();
			SaveCurrentScene();            
        }
		iCS_Version storageVersion= new iCS_Version(PersistentStorage.MajorVersion, PersistentStorage.MinorVersion, PersistentStorage.BugFixVersion);
		if(softwareVersion.IsEqual(storageVersion)) { return; }
		
        // POST-PROCESING ====================================================
        // v1.2.0 Needs to convert "this" port name to "type instance"
		if(storageVersion.IsOlderThen(1,2,0)) {
            foreach(var obj in PersistentStorage.EngineObjects) {
                if(obj.IsDataPort && obj.RawName == "this" && obj.RuntimeType != null) {
                    obj.RawName= GetInstancePortName(obj.RuntimeType);
                    isUpgraded= true;
                }         
            }
        }
        // Warn the user that an upgrade toke place.
        if(isUpgraded) {
			ShowUpgradeDialog(softwareVersion);
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
