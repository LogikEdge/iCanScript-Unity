using UnityEngine;
using System;
using System.IO;
using System.Collections;
using iCanScript.Internal.JSON;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;
	
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class creates an active project singleton.
    ///
    /// On stratup, it will attempt to recover the last active project.  If no
    /// active project can be found, the user will be asked to create or select
    /// an existing project.
    ///
    public static class ProjectController {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
		static string		myProjectPath= null;
        static ProjectInfo  myProject    = null;
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
        public static ProjectInfo Project {
            get {
                if(myProject == null) {
                    GetProject();
                }
                return myProject;
            }
            set {
                if(myProject != value) {
                    SaveProject();
                }
            }
        }
        // =================================================================================
        // INIT / SHUTDOWN
        // ---------------------------------------------------------------------------------
        static ProjectController() {
            LoadProjectFromPreferences();
        }
        public static void Start() {}
        public static void Shutdown() {
            Prefs.ActiveProjectPath= myProjectPath;
        }
        
        // =================================================================================
        /// Loads the active project from the user preferences.
        public static void LoadProjectFromPreferences() {
			myProjectPath= Prefs.ActiveProjectPath;
			if(String.IsNullOrEmpty(myProjectPath)) {
//				CreateProject();
				return;
			}
            LoadProject(myProjectPath);
        }

        // =================================================================================
        /// Save to active project reference in the user preferences.
        public static void SaveProjectToPreferences() {
            if(myProject == null) return;
            SaveProject();
            //TODO:
        }

        // =================================================================================
        /// Save the project file to disk.
        public static void SaveProject() {
            myProject.Save();
        }

        // =================================================================================
        /// Load the project file from disk.
        ///
        /// @param projectPath The absolute path of the project file.
        /// @info The active project set to the newly loaded project.
        /// 
        public static void LoadProject(string projectPath) {
            var jsonRoot= JSONFile.Read(projectPath);
            if(jsonRoot == null || jsonRoot.isNull) {
                Debug.LogError("iCanScript: Unable to load project at=> "+projectPath);
                return;
            }
            // TODO:
        }

        // =================================================================================
        /// Ask the user to create or select an exist project.
        public static void GetProject() {
            // TODO:
            myProject= CreateProject("iCanScript-Examples.ProjectTest");
            var projects= FileUtils.GetFilesWithExtension("icsproject");
            foreach(var p in projects) {
            }
        }

        // =================================================================================
        /// Creates a project file.
        ///
        /// @param projectName The name of the project.
        /// @return The newly created project info.
        ///
        public static ProjectInfo CreateProject(string projectName) {
            var project= new ProjectInfo(projectName);
			var projectPath= projectName.Replace('.', '/');
            project.Save();
            return project;
        }
    }

}