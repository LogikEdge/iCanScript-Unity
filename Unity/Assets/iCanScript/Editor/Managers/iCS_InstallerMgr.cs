using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_InstallerMgr {

    // =================================================================================
    // Prompt installation.
    // ---------------------------------------------------------------------------------
	static public void InstallGizmo() {
        // Copy the iCanScript gizmo file into the "Gizmos" project folder.
        string assetPath= Application.dataPath;
        string gizmoSrc= iCS_Config.ResourcePath+"/iCanScriptGizmo.png";
        string gizmoDest= "Assets/Gizmos/iCanScriptGizmo.png";
        if(!Directory.Exists(assetPath+"/Gizmos")) {
            Debug.Log("iCanScript: Creating Gizmos folder");
    	    AssetDatabase.CreateFolder("Assets","Gizmos");            
        }
        if(iCS_Strings.IsEmpty(AssetDatabase.ValidateMoveAsset(gizmoSrc, gizmoDest))) {
    	    AssetDatabase.CopyAsset(gizmoSrc,gizmoDest);
    	    Debug.Log("iCanScript: Copying iCanScriptGizmo.png into the Gizmos folder");            
        }	    
	}
}
