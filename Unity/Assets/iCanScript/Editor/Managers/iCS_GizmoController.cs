using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

public static class iCS_GizmoController {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------
	public const string SourceGizmoIcon      = "iCS_Logo128x128.png";
    public const string GizmoIcon            = "iCanScriptGizmo.png";
          
    // ---------------------------------------------------------------------------------
    static iCS_GizmoController() {
        InstallGizmo();
    }
    
    // ---------------------------------------------------------------------------------
    public static void Start() {}
        
    // ---------------------------------------------------------------------------------
    public static void Shutdown() {}
    
    // ---------------------------------------------------------------------------------
    static void InstallGizmo() {
        // Copy the iCanScript gizmo file into the "Gizmos" project folder.
        string assetPath= Application.dataPath;
        if(!Directory.Exists(assetPath+"/"+iCS_Config.GizmosFolder)) {
            AssetDatabase.CreateFolder("Assets",iCS_Config.GizmosFolder);            
        }
        string gizmoSrc= iCS_Config.ResourcePath+"/"+SourceGizmoIcon;
        string gizmoDest= "Assets/Gizmos/iCanScriptGizmo.png";
        if(iCS_Strings.IsEmpty(AssetDatabase.ValidateMoveAsset(gizmoSrc, gizmoDest))) {
            AssetDatabase.CopyAsset(gizmoSrc,gizmoDest);
            Debug.Log("iCanScript: Installing the iCanScript Gizmo graphic file.");            
        }         
    }

    // ---------------------------------------------------------------------------------
    [DrawGizmo(GizmoType.NotSelected | GizmoType.Selected)]
    public static void DrawGizmos(Component component, GizmoType gizmoType) {
        var go= component.gameObject;
        var p= go.transform.position;
        Gizmos.DrawIcon(p, iCS_Strings.GizmoIcon);
        if(go.renderer != null) {
            for(int intensity= 5; intensity >= 0; --intensity) {
                Gizmos.DrawIcon(p, GizmoIcon);                
            }
        }
    }
}
