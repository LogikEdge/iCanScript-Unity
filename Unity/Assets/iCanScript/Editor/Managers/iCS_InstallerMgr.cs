using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_InstallerMgr {
    // =================================================================================
    // Installs the iCanScript Gizmo (if not already done).
    // ---------------------------------------------------------------------------------
	static public void InstallGizmo() {
        // Copy the iCanScript gizmo file into the "Gizmos" project folder.
        string assetPath= Application.dataPath;
        if(!Directory.Exists(assetPath+"/Gizmos")) {
            Debug.Log("iCanScript: Creating Gizmos folder");
    	    AssetDatabase.CreateFolder("Assets","Gizmos");            
        }
        string gizmoSrc= iCS_Config.ResourcePath+"/"+iCS_EditorStrings.GizmoIcon;
        string gizmoDest= "Assets/Gizmos/iCanScriptGizmo.png";
        if(iCS_Strings.IsEmpty(AssetDatabase.ValidateMoveAsset(gizmoSrc, gizmoDest))) {
    	    AssetDatabase.CopyAsset(gizmoSrc,gizmoDest);
    	    Debug.Log("iCanScript: Copying iCanScriptGizmo.png into the Gizmos folder");            
        }	    
	}

    // =================================================================================
    // Install the iCanScript Gizmo (if not already done).
    // ---------------------------------------------------------------------------------
    public static bool CheckForUpdates() {
        if(iCS_WebUtils.IsLatestVersion()) {
            return true;
        }
        var latestVersion= iCS_WebUtils.GetLatestReleaseId();
        var download= EditorUtility.DisplayDialog("A new version of iCanScript is available!",
                                                  "Version "+latestVersion+" is available for download.\n"+
                                                  "Would you like to download it?",
                                                  "Download", "Skip This Version");
        if(download) {
            Application.OpenURL("http://www.icanscript.com/support/release_notes");            
        }
        return false;
    }
}
