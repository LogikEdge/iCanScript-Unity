using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor {

	public class PackageSettingsEditor : ConfigEditorBase {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
		PackageInfo	myProject      = new PackageInfo();
    	string[]    myOptionStrings= new string[]{
			"Create",
            "Update"
    	};
    	
        // =================================================================================
        // INITIALIZATION
        // ---------------------------------------------------------------------------------
        public static PackageSettingsEditor Init() {
            var editor= EditorWindow.CreateInstance<PackageSettingsEditor>();
            editor.ShowUtility();
            return editor;
        }

        // =================================================================================
        // SPECIALIZATION
        // ---------------------------------------------------------------------------------
        /// Allows the child class to define an horizontal offset for the value column.
        ///
        /// @return The horizontal offset.
        ///
        protected override float GetValueHorizontalOffset() {
            return -30f;
        }
        

        // =================================================================================
        // INTERFACES TO BE PROVIDED
        // ---------------------------------------------------------------------------------
        protected override string   GetTitle() {
            return "iCanScript Package Settings";
        }
        protected override string[] GetMainSelectionGridStrings() {
            return myOptionStrings;
        }
        protected override void     ProcessSelection(int selection) {
            // Execute option specific panel.
            switch(selection) {
				case 0: Create(); break;
                case 1: Update(); break;
                default: break;
            }
        }
        
        // =================================================================================
        /// Ask the user to provide the needed information to create a project.
		void Create() {
            // -- Label column --
            var pos= GetLabelColumnPositions(7);
            GUI.Label(pos[0], "Package Name");
            GUI.Label(pos[1], "Parent Namespace");
            GUI.Label(pos[2], "Create Project Folder");
            GUI.Label(pos[4], "Project Folder");
            GUI.Label(pos[5], "Namespace");
            GUI.Label(pos[6], "Editor Namespace");
            
            // -- Value column --
            pos= GetValueColumnPositions(7);
            myProject.PackageName= EditorGUI.TextField(pos[0], myProject.PackageName);
			var allProjects= BuildNamespaceSelection();
            var isFolderSelection= EditorGUI.Popup(pos[1], 0, allProjects);
            myProject.CreateProjectFolder= EditorGUI.Toggle(pos[2], myProject.CreateProjectFolder);
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(pos[4], myProject.GetRelativePackageFolder());
			EditorGUI.TextField(pos[5], myProject.GetEngineNamespace());
			EditorGUI.TextField(pos[6], myProject.GetEditorNamespace());
            EditorGUI.EndDisabledGroup();

            // -- Process buttons. --
            if(isFolderSelection != 0) {
                myProject.RootFolder= EditorUtility.OpenFolderPanel("iCanScript Project Folder Selection", Application.dataPath, "");                
            }
        
    		// -- Compute button area. --
            var totalWidth= kColumn2Width + kColumn3Width;
            var width= totalWidth / 3f;
            var buttonWidth= width - 2f*kMargin;
            var buttonX= kColumn2X + 2*kMargin;
            var buttonY= position.height-kMargin-20.0f;

            // -- Show project already exists error message. --
            bool projectAlreadyExists= myProject.AlreadyExists;
            if(projectAlreadyExists) {
                var x= buttonX-kMargin;
                var y= buttonY - 3f*pos[0].height;
                var w= totalWidth;
                var h= 2f*pos[0].height;
                EditorGUI.HelpBox(new Rect(x,y,w,h), "PROJECT ALREADY EXISTS:\n--> "+myProject.GetRelativeFileNamePath(), MessageType.Error);                
            }
                        
            // -- Process "Save" button. --
            EditorGUI.BeginDisabledGroup(projectAlreadyExists);
            if(GUI.Button(new Rect(buttonX+width, buttonY, buttonWidth, 20.0f),"Save")) {
                if(!projectAlreadyExists) {
                    myProject.Save();
					PackageController.UpdateProjectDatabase();
                    Close();
                }
            }
            EditorGUI.EndDisabledGroup();            
            // -- Process "Cancel" button. --
            if(GUI.Button(new Rect(buttonX+2f*width, buttonY, buttonWidth, 20.0f),"Cancel")) {
                Close();
            }
		}
        
        // =================================================================================
        /// Updates the active project information.
        void Update() {
            
        }
        // =================================================================================
        /// Build namespace menu.
		string[] BuildNamespaceSelection() {
			var allNamespaces= P.map(p=> p.EngineNamespace, PackageController.Projects);
			Array.Sort(allNamespaces, (x,y)=> string.Compare(x,y));
			var len= allNamespaces.Length;
			Array.Resize(ref allNamespaces, len+1);
			Array.Copy(allNamespaces, 0, allNamespaces, 1, len);
			allNamespaces[0]= "-- None --";
			return allNamespaces;
		}

		 
	}

}