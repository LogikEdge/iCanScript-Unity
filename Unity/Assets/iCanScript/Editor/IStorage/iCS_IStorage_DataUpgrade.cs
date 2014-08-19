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
        Debug.Log("Storage Version=> "+storageVersion);
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
        // v1.2.1: Convert GameController scale without deltaTime
		if(storageVersion.IsOlderThen(1,2,1)) {
            Debug.Log("Version is older then v1.2.1");
            foreach(var obj in PersistentStorage.EngineObjects) {
                if(obj.IsClassFunction) {
                    var runtimeType= obj.RuntimeType;
                    if(runtimeType != null && runtimeType.Name == "iCS_GameController") {
                        if(obj.MethodName == "GameController") {
                            Debug.Log("Found a GameController function");                            
                        }
                    }
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
