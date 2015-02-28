#define DEBUG
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript { namespace Editor {
    
    public static class SceneController {
        // ======================================================================
        // Scene Cache
        // ----------------------------------------------------------------------
        // Visual Scripts
        static Texture2D                ourLogo                              = null;
        static iCS_VisualScriptImp[]    ourVisualScriptsInScene              = null;
        static iCS_VisualScriptImp[]    ourVisualScriptsReferencesByScene    = null;
        static iCS_VisualScriptImp[]    ourVisualScriptsInOrReferencesByScene= null;
    
        // ======================================================================
        // Scene properties
        // ----------------------------------------------------------------------
        public static iCS_VisualScriptImp[] VisualScriptsInScene {
            get { return ourVisualScriptsInScene; }
        }
        public static iCS_VisualScriptImp[] VisualScriptsReferencedByScene {
            get { return ourVisualScriptsReferencesByScene; }
        }
        public static iCS_VisualScriptImp[] VisualScriptsInOrReferencedByScene {
            get { return ourVisualScriptsInOrReferencesByScene; }
        }
        static Texture2D iCanScriptLogo {
            get {
                if(ourLogo == null) {
                    iCS_TextureCache.GetIcon(iCS_EditorStrings.TitleLogoIcon, out ourLogo);
                }
                return ourLogo;  
            }
        }
        public static int NumberOfVisualScriptsInOrReferencedByScene {
            get { return VisualScriptsInOrReferencedByScene == null ? 0: VisualScriptsInOrReferencedByScene.Length; }
        }
        
        // ======================================================================
        // Common Controller activation/deactivation
        // ----------------------------------------------------------------------
    	static SceneController() {
            // -- Delegate to draw iCanScript icon in hierarchy --
            EditorApplication.hierarchyWindowItemOnGUI+= UnityHierarchyItemOnGui;
            
            // -- Events to refresh scene content information --
    		SystemEvents.OnEditorStarted   += RefreshSceneInfo;
            SystemEvents.OnSceneChanged    += RefreshSceneInfo;
            SystemEvents.OnHierarchyChanged+= RefreshSceneInfo;
            SystemEvents.OnProjectChanged  += RefreshSceneInfo;
    		SystemEvents.OnCompileStarted  += RefreshSceneInfo;
            // -- Force an initial refresh of the scene info --
            RefreshSceneInfo();  
    	}
        
        /// Start the application controller.
    	public static void Start() {}
        /// Shutdowns the application controller.
        public static void Shutdown() {
            // -- Remove events to refresh scene content information --
    		SystemEvents.OnEditorStarted   -= RefreshSceneInfo;
            SystemEvents.OnSceneChanged    -= RefreshSceneInfo;
            SystemEvents.OnHierarchyChanged-= RefreshSceneInfo;
            SystemEvents.OnProjectChanged  -= RefreshSceneInfo;
    		SystemEvents.OnCompileStarted  -= RefreshSceneInfo;
    
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
        
        // ======================================================================
        // Update scene content changed
        // ----------------------------------------------------------------------
        static void RefreshSceneInfo() {
            // -- Scan scene for visual scripts --
            ourVisualScriptsInScene              = ScanForVisualScriptsInScene();
            ourVisualScriptsReferencesByScene    = ScanForVisualScriptsReferencedByScene();
            ourVisualScriptsInOrReferencesByScene= CombineVisualScriptsInOrReferencedByScene();
    
    		// -- Vaidate proper visual script setup --
    		SceneSanityCheck();
        }
    
        // ======================================================================
        // VISUAL SCRIPTS
        // ----------------------------------------------------------------------
        /// Returns all active VisualScript included in the current scene.
        static iCS_VisualScriptImp[] ScanForVisualScriptsInScene() {
            var allVisualScripts= UnityEngine.Object.FindObjectsOfType(typeof(iCS_VisualScriptImp));
            return Array.ConvertAll( allVisualScripts, e=> e as iCS_VisualScriptImp );
        }
    
        // ----------------------------------------------------------------------
        /// Returns all Visual Scripts referenced in the current scene.
    	///
    	/// Note:	This function assumes that the visual script in the scene
    	///			has already been fetched.
    	///
        static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedByScene() {
    		return P.removeDuplicates(
    			P.fold(
    				(acc,vs)=> P.append(acc, ScanForVisualScriptsReferencedBy(vs)),
    				new iCS_VisualScriptImp[0],
    				VisualScriptsInScene
    			)
    		);
        }
        
        // ----------------------------------------------------------------------
        /// Returns all Visual Scripts referenced in the current scene.
    	///
    	/// Note:	This function assumes that the visual script in and referenced
    	///			by this scene have already been fetched.
    	///
        static iCS_VisualScriptImp[] CombineVisualScriptsInOrReferencedByScene() {
            return P.removeDuplicates(P.append(VisualScriptsInScene, VisualScriptsReferencedByScene));
        }
        
        // ----------------------------------------------------------------------
        /// Returns Visual Scripts referenced by the given Visual Script.
        static iCS_VisualScriptImp[] ScanForVisualScriptsReferencedBy(iCS_VisualScriptImp vs) {
            var visualScripts= P.map(o=> o as iCS_VisualScriptImp, P.filter(o=> o is iCS_VisualScriptImp, vs.UnityObjects));
            var gameObjects  = P.map(o=> o as GameObject         , P.filter(o=> o is GameObject         , vs.UnityObjects));
    		return P.removeDuplicates(
    			P.fold(
    				(acc,go)=> {
    	                var goVs= go.GetComponent<iCS_VisualScriptImp>() as iCS_VisualScriptImp;
    	                if(goVs != null) {
    	                    acc.Add(goVs);
    	                }
    					return acc;
                    },
    				visualScripts,
    				gameObjects
    			)
    		).ToArray();
        }
    	
        // ======================================================================
        // VALIDATE SCENE VISUAL SCRIPTS
        // ----------------------------------------------------------------------
    	static void SceneSanityCheck() {
    //		Debug.Log("SceneSanityCheck is running");
    		foreach(var vs in VisualScriptsInOrReferencedByScene) {
    			var go= vs.gameObject;
    			CleanupEmptyComponents(go);
    			if(go == null) continue;
    			var behaviourScript= vs.gameObject.GetComponent<iCS_BehaviourImp>();
    			if(behaviourScript == null) {
    //				Debug.LogWarning("iCanScript: iCS_Behaviour script has been disconnected from=> "+go.name+".  Attempting to reconnect...");
                    iCS_DynamicCall.AddBehaviour(go);
    			}
    		}
    	}
    	static void CleanupEmptyComponents(GameObject go) {
    		var allComponents= go.GetComponents<Component>();
    		foreach(var c in allComponents) {
    			if(c == null) {
    				Debug.LogWarning("iCanScript: PLEASE REMOVE empty component found on=> "+go.name+".");
    			}
    		}
    	}
    }
    
}}