using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using iCanScript.Engine;

namespace iCanScript { namespace Editor {
    
    public static class GizmoController {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        public const string GizmosFolder   = "Gizmos";
    	public const string SourceGizmoIcon= "iCS_Logo_128x128.png";
        public const string GizmoIcon      = "iCanScriptGizmo.png";
              
        // ---------------------------------------------------------------------------------
        static GizmoController() {
    		SystemEvents.OnEditorStarted+= InstallGizmo;
        }
        
        // ---------------------------------------------------------------------------------
        public static void Start() {}
            
        // ---------------------------------------------------------------------------------
        public static void Shutdown() {
    		SystemEvents.OnEditorStarted-= InstallGizmo;
        }
        
        // ---------------------------------------------------------------------------------
        static void InstallGizmo() {
            // Copy the iCanScript gizmo file into the "Gizmos" project folder.
            string systemAssetPath= Application.dataPath;
            string systemGizmosFolder= systemAssetPath+"/"+GizmosFolder;
            string unityGizmosFolder= "Assets/"+GizmosFolder;
            if(!Directory.Exists(systemGizmosFolder)) {
                AssetDatabase.CreateFolder("Assets",GizmosFolder);            
            }
            string gizmoSrc= iCS_Config.ImagePath+"/"+SourceGizmoIcon;
            string gizmoDest= unityGizmosFolder+"/"+GizmoIcon;
            if(iCS_Strings.IsEmpty(AssetDatabase.ValidateMoveAsset(gizmoSrc, gizmoDest))) {
                AssetDatabase.CopyAsset(gizmoSrc,gizmoDest);
            }         			
        }
    
        // ---------------------------------------------------------------------------------
        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
        public static void DrawGizmos(iCS_VisualScriptImp visualScript, GizmoType gizmoType) {
            var go= visualScript.gameObject;
            var p= go.transform.position;
            Gizmos.DrawIcon(p, GizmoIcon);
            if(go.GetComponent<Renderer>() != null) {
                for(int intensity= 5; intensity >= 0; --intensity) {
                    Gizmos.DrawIcon(p, GizmoIcon);                
                }
            }			
        }
    }
    
}}