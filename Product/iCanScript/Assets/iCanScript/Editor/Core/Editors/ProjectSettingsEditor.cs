using UnityEngine;
using UnityEditor;
using System.Collections;

namespace iCanScript.Internal.Editor {

	public class ProjectSettingsEditor : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
		ProjectInfo	myProject      = new ProjectInfo();
    	string[]    myOptionStrings= new string[]{
			"General"
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static void Init(iCS_IStorage iStorage) {
            var editor= EditorWindow.CreateInstance<ProjectSettingsEditor>();
            editor.ShowUtility();
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
				case 0: General(); break;
                default: break;
            }
        }
        
        // =================================================================================
		void General() {
            // -- Label column --
            var pos= GetLabelColumnPositions(4);
            GUI.Label(pos[0], "Project Name");
            GUI.Label(pos[2], "Namespace");
            GUI.Label(pos[3], "Editor Namespace");
            
            // -- Value column --
            pos= GetValueColumnPositions(4);
            myProject.ProjectName    = EditorGUI.TextField(pos[0], myProject.ProjectName);
			myProject.Namespace      = EditorGUI.TextField(pos[2], myProject.Namespace);
			myProject.EditorNamespace= EditorGUI.TextField(pos[3], myProject.EditorNamespace);

            // -- Reset button --
            if(GUI.Button(new Rect(kColumn2X+kMargin, position.height-kMargin-20.0f, 0.75f*kColumn2Width, 20.0f),"Reset Namespaces")) {
				GUI.FocusControl("");			// Remove keyboard focus.
				myProject.ResetNamespaces();
            }

    		// -- Save changes --
            if(GUI.changed) {
                myProject.Save();
            }
		}
	}

}