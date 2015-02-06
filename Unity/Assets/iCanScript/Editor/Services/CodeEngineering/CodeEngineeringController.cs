using UnityEngine;
using System.Collections;

namespace iCanScriptEditor { namespace CodeEngineering {
    
    public static class CodeEngineeringController {
        // ======================================================================
    	// Start Code generator service.
        // ----------------------------------------------------------------------
    	static CodeEngineeringController() {
            // Create default code generation path.
            iCanScriptEditor.FileUtils.CreateAssetFolder("VisualScripts");
            
            // Install events.
            SystemEvents.OnHierarchyChanged+= OnHierarchyChanged;
            SystemEvents.OnEditorStarted   += OnEditorStarted;				
    	}
    	public static void Start() {}
        public static void Shutdown() {}
    
        // ----------------------------------------------------------------------
        static void OnEditorStarted() {
            CSharpGenerateBehaviour.UpdateBehaviourCode();        
        }
        
        // ----------------------------------------------------------------------
        // Update Visual Script to Behaviour relationship.
        static void OnHierarchyChanged() {
            var allGameObjects= GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
            if(allGameObjects != null) {
                foreach(var go in allGameObjects) {
                    iCS_MenuUtility.UpdateBehaviourComponent(go);
                }
            }
        }
    }

}}
