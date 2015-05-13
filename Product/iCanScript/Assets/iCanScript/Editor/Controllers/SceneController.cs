#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {
    
    public static class SceneController {
        // ======================================================================
        // Scene Cache
        // ----------------------------------------------------------------------
        static Texture2D    ourLogo= null;
    
        // ======================================================================
        // Scene properties
        // ----------------------------------------------------------------------
        static Texture2D iCanScriptLogo {
            get {
                if(ourLogo == null) {
                    TextureCache.GetIcon(iCS_EditorStrings.TitleLogoIcon, out ourLogo);
                }
                return ourLogo;  
            }
        }
        
        // ======================================================================
        // Common Controller activation/deactivation
        // ----------------------------------------------------------------------
    	static SceneController() {
            // -- Delegate to draw iCanScript icon in hierarchy --
            EditorApplication.hierarchyWindowItemOnGUI+= UnityHierarchyItemOnGui;
    	}
        
        /// Start the application controller.
    	public static void Start() {}
        /// Shutdowns the application controller.
        public static void Shutdown() {
            // -- Delegate to draw iCanScript icon in hierarchy --
            EditorApplication.hierarchyWindowItemOnGUI-= UnityHierarchyItemOnGui;
        }
    
        // ======================================================================
        // Show iCanScript logo in Unity hierarchy
        // ----------------------------------------------------------------------
        static void UnityHierarchyItemOnGui(int instanceId, Rect r) {
            // -- Need to have a logo to show --
            var logo= iCanScriptLogo;
            if(logo == null) return;
            var heightDiff= r.height-logo.height;
            if(heightDiff <= 0f) heightDiff= 0f;
            var iconRect= new Rect(r.xMax-logo.width, r.y+0.5f*heightDiff, logo.width, logo.height);
            
            // -- Get the associated the game object --
            var unityObject= EditorUtility.InstanceIDToObject(instanceId);
            var go= unityObject as GameObject;
            if(go != null) {
                if(go.GetComponent<iCS_VisualScriptImp>() != null) {
                    // -- Assure that we have a visual editor opened --
                    iCS_EditorController.OpenVisualEditor();
                    // -- Draw iCanScript logo next to hierarchy item --
                    GUI.DrawTexture(iconRect, logo);
                }
            }
        }
        static bool IsVisualScriptInChild(GameObject go) {
            var t= go.transform;
            var childCount= t.childCount;
            for(int i= 0; i < childCount; ++i) {
                var child= t.GetChild(i).gameObject;
                if(child != null) {
                    if(child.GetComponent<iCS_VisualScriptImp>() != null) {
                        return true;
                    }
                    var result= IsVisualScriptInChild(child);
                    if(result == true) {
                        return true;
                    }
                }
            }
            return false;
        }
            	
    }
    
}