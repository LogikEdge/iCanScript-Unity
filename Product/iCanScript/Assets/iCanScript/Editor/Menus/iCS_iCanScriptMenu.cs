using UnityEngine;
using UnityEditor;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor {
    
    public static class iCS_iCanScriptMenu {
        // ======================================================================
        // About...
        [MenuItem("iCanScript/About...",false,10)]
        public static void About() {
            var aboutDialog= iCS_AboutDialog.CreateInstance<iCS_AboutDialog>();
            aboutDialog.ShowUtility();
        }

        // ======================================================================
    	// Create a behavior to selected game object.
    	[MenuItem("iCanScript/Create Visual Script", false, 101)]
    	public static void CreateVisualScript() {
            var gameObject= Selection.activeGameObject;
            if(gameObject == null) return;
    		var visualScript= gameObject.GetComponent<iCS_VisualScriptImp>();
    		if(visualScript == null) {
        		iCS_DynamicCall.AddVisualScript(gameObject);
    		}
    	}
    	[MenuItem("iCanScript/Create Visual Script", true, 101)]
    	public static bool ValidateCreateVisualScript() {
    		if(Selection.activeGameObject == null) return false;
    		var visualScript = Selection.activeGameObject.GetComponent<iCS_VisualScriptImp>();
    		return visualScript == null;
    	}

        // ======================================================================
        // Navigation
        [MenuItem("iCanScript/Frame Selected",false,201)]
        public static void FocusOnSelected() {
            iCS_VisualEditor graphEditor= iCS_EditorController.FindVisualEditor();
            if(graphEditor != null) graphEditor.CenterAndScaleOnSelected();
        }
        [MenuItem("iCanScript/Frame Selected",true,201)]
        public static bool ValidateFocusOnSelected() {
            var focusedWindow= EditorWindow.focusedWindow;
            if(focusedWindow == null) return false;
            if((focusedWindow as iCS_VisualEditor) == null) return false;
            return true;
        }
        // ======================================================================
        // Export Storage
        [MenuItem("iCanScript/Export...",false,300)]
        public static void ExportStorage() {
            var transform= Selection.activeTransform;
            if(transform == null) return;
            var go= transform.gameObject;
            if(go == null) return;
            var monoBehaviour= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
            if(monoBehaviour == null) return;
            var storage= new iCS_VisualScriptData(monoBehaviour);
            var initialPath= Application.dataPath;
            var path= EditorUtility.SaveFilePanel("Export Visual Script", initialPath, monoBehaviour.name+".ics2", "ics2");
            if(string.IsNullOrEmpty(path)) return;
            iCS_VisualScriptImportExport.Export(storage, path);
            Debug.Log("iCanScript: Export completed => "+path);
        } 
        [MenuItem("iCanScript/Export...",true,300)]
        public static bool ValidateExportStorage() {
            var transform= Selection.activeTransform;
            if(transform == null) return false;
            var go= transform.gameObject;
            if(go == null) return false;
            var visualEditor= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
            return visualEditor != null;
        }
        // ======================================================================
        // Import Storage
        [MenuItem("iCanScript/Import...",false,301)]
        public static void ImportStorage() {
            var transform= Selection.activeTransform;
            if(transform == null) return;
            var go= transform.gameObject;
            if(go == null) return;
            var monoBehaviour= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
            if(monoBehaviour == null) return;
            var initialPath= Application.dataPath;
            var path= EditorUtility.OpenFilePanel("Import Visual Script", initialPath, "ics2");
            if(string.IsNullOrEmpty(path)) return;
            var tmpVsd= new iCS_VisualScriptData();
            if(!iCS_VisualScriptImportExport.Import(tmpVsd, path)) {
                Debug.LogError("iCanScript: Import failure. The selected visual script was not modified.");
            }
            tmpVsd.CopyTo(monoBehaviour);
            Debug.Log("iCanScript: Import completed => "+path);
            // Attempt to reload and relayout if the selection is visible in the visual editor.
            var visualEditor= iCS_EditorController.FindVisualEditor();
            if(visualEditor == null) return;
            var iStorage= visualEditor.IStorage;
            if(iStorage.iCSMonoBehaviour != monoBehaviour) return;
            iCS_UserCommands.InitEditorData(iStorage);
        }
        [MenuItem("iCanScript/Import...",true,301)]
        public static bool ValidateImportStorage() {
            var transform= Selection.activeTransform;
            if(transform == null) return false;
            var go= transform.gameObject;
            if(go == null) return false;
            var visualEditor= go.GetComponent<iCS_MonoBehaviourImp>() as iCS_MonoBehaviourImp;
            return visualEditor != null;
        }
        // ======================================================================
        // Tutorials Access
        [MenuItem("iCanScript/Tutorials/Space Shooter Tutorial on YouTube",false,999)]
        public static void SpaceShooterTutorialPlaylist() {        
            Application.OpenURL("http://www.youtube.com/playlist?list=PLbRggLFpBWUQA7RgHO_eIZvl56W4A53Vj");
        }
        // ======================================================================
        // Support Access
        [MenuItem("iCanScript/Support/iCanScript Website",false,1000)]
        public static void HomePage() {
            Application.OpenURL("http://www.icanscript.com");
        }
        [MenuItem("iCanScript/Support/HowTo",false,1001)]
        public static void Learn() {
            Application.OpenURL("http://www.icanscript.com/learn2");
        }
        [MenuItem("iCanScript/Support/Submit a ticket",false,1001)]
        public static void ReleaseNotes() {
            Application.OpenURL("http://www.icanscript.com/support");
        }
        [MenuItem("iCanScript/Support/Knowledge Base",false,1002)]
        public static void Helpdesk() {
            Application.OpenURL("http://www.icanscript.com/support");
        }
        [MenuItem("iCanScript/Support/Forum",false,1002)]
        public static void Forum() {
            Application.OpenURL("http://www.icanscript.com/forum");
        }
        // ======================================================================
        // Purchase
        [MenuItem("iCanScript/Check for Updates...",false,1003)]
        public static void CheckForUpdate() {
    		SoftwareUpdateController.ManualUpdateVerification();
        }
    #if COMMUNITY_EDITION
        [MenuItem("iCanScript/Purchase...",false,1004)]
        public static void Purchase() {
    		Application.OpenURL("http://u3d.as/content/disruptive-software/i-can-script-visual-scripting/7ji");
    	}
    #endif
    }
}

