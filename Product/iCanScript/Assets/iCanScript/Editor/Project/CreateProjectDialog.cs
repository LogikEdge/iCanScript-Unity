using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

	public class CreateProjectDialog : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
		ProjectInfo	myProject      = new ProjectInfo();
    	string[]    myOptionStrings= new string[]{
			"Create",
            "Open",
            "Remove"
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static CreateProjectDialog Init() {
            var editor= EditorWindow.CreateInstance<CreateProjectDialog>();
            editor.ShowUtility();
            return editor;
        }

        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "iCanScript Project Settings";
        }
        protected override string[] GetMainSelectionGridStrings() {
            return myOptionStrings;
        }
        protected override void     ProcessSelection(int selection) {
            // Execute option specific panel.
            switch(selection) {
				case 0: Create(); break;
                case 1: Open(); break;
                case 2: Remove(); break;
                default: break;
            }
        }
        
        // =================================================================================
        /// Ask the user to provide the needed information to create a project.
		void Create() {
            // -- Label column --
            var pos= GetLabelColumnPositions(7);
            GUI.Label(pos[0], "Project Name");
            GUI.Label(pos[1], "Project Parent Folder");
            GUI.Label(pos[2], "Create Project Folder");
            GUI.Label(pos[4], "Project Folder");
            GUI.Label(pos[5], "Namespace");
            GUI.Label(pos[6], "Editor Namespace");
            
            // -- Value column --
            pos= GetValueColumnPositions(7);
            myProject.ProjectName= EditorGUI.TextField(pos[0], myProject.ProjectName);
            var isFolderSelection= GUI.Button(pos[1], "Select Folder...");
            myProject.CreateProjectFolder= EditorGUI.Toggle(pos[2], myProject.CreateProjectFolder);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(pos[4], myProject.GetRelativeProjectFolder());
			EditorGUI.TextField(pos[5], myProject.GetNamespace());
			EditorGUI.TextField(pos[6], myProject.GetEditorNamespace());
            EditorGUI.EndDisabledGroup();

            // -- Process buttons. --
            if(isFolderSelection) {
                myProject.RootFolder= EditorUtility.OpenFolderPanel("iCanScript Project Folder Selection", Application.dataPath, "");                
            }
        
    		// -- Save changes --
            var totalWidth= kColumn2Width + kColumn3Width;
            var width= totalWidth / 3f;
            var buttonWidth= width - 2f*kMargin;
            var buttonX= kColumn2X + 2*kMargin;
            var buttonY= position.height-kMargin-20.0f;
            if(GUI.Button(new Rect(buttonX, buttonY, buttonWidth, 20.0f),"Save")) {
                myProject.Save();
            }
            if(GUI.Button(new Rect(buttonX+width, buttonY, buttonWidth, 20.0f),"Save & Close")) {
                myProject.Save();
                Close();
            }
            if(GUI.Button(new Rect(buttonX+2f*width, buttonY, buttonWidth, 20.0f),"Cancel")) {
                Close();
            }
		}
        
        // =================================================================================
        /// Ask the user to select which project to open.
        void Open() {
            // -- Label column --
            var pos= GetLabelColumnPositions(1);
            GUI.Label(pos[0], "Select Project");
            
            // -- Value column --
            pos= GetValueColumnPositions(1);
            var projectPaths= ProjectController.GetProjects();
            var projectNames= P.map(p=> ProjectController.GetProjectName(p), projectPaths);
            var idx= EditorGUI.Popup(pos[0], -1, projectNames);
            if(idx >= 0 && idx < projectPaths.Length) {
                Debug.Log("Opening: "+projectNames[idx]);
            }
        }

        // =================================================================================
        /// Ask the user to select which project to remove.
        void Remove() {
            
        }
        
	}

}