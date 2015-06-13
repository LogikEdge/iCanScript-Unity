using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {

	public class ProjectSettingsEditor : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        iCS_IStorage	iStorage= null;
    	string[] myOptionStrings= new string[]{
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static void Init(iCS_IStorage iStorage) {
            var editor= EditorWindow.CreateInstance<ProjectSettingsEditor>();
            editor.ShowUtility();
            editor.iStorage= iStorage;
        }

        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "Project Setting";
        }
        protected override string[] GetMainSelectionGridStrings() {
            return myOptionStrings;
        }
        protected override void     ProcessSelection(int selection) {
            // Execute option specific panel.
            switch(selection) {
                default: break;
            }
        }
        
	}

}