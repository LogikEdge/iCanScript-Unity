﻿using UnityEngine;
using UnityEditor;
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
                    GetProjects();
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
        /// Creates a project file.
    	[MenuItem("iCanScript/Create Project", false, 100)]
    	public static void CreateProject() {
            /*var editor=*/ CreateProjectDialog.Init();
    	}

        // =================================================================================
        /// Opens an existing project file.
    	[MenuItem("iCanScript/Open Project", false, 101)]
    	public static void OpenProject() {
            CreateProjectsMenu(OpenProject);
    	}
    	[MenuItem("iCanScript/Open Project", true, 101)]
    	public static bool ValidateOpenProject() {
            return GetProjects().Length > 0;
    	}
        static void OpenProject(object projectPath) {
            Debug.Log("Opening: "+projectPath);
        }
        
        // =================================================================================
        /// Removes an existing project file.
    	[MenuItem("iCanScript/Remove Project", false, 102)]
    	public static void RemoveProject() {
            CreateProjectsMenu(RemoveProject);
    	}
    	[MenuItem("iCanScript/Remove Project", true, 102)]
    	public static bool ValidateRemoveProject() {
            return GetProjects().Length > 0;
    	}
        static void RemoveProject(object projectPath) {
            Debug.Log("Removing: "+projectPath);
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
        /// Get all existing projects.
        public static string[] GetProjects() {
            return FileUtils.GetFilesWithExtension("icsproject");
        }

        // =================================================================================
        /// Extracts the project name from the given project path.
        ///
        /// @param projectPath The project path.
        /// @return The associated project names.
        ///
        public static string GetProjectName(string projectPath) {
            return Path.GetFileNameWithoutExtension(projectPath);
        }

        // =================================================================================
        /// Ask the user to create or select an exist project.
        public static void CreateProjectsMenu(GenericMenu.MenuFunction2 callback) {
            var projects= GetProjects();
            var menu= new GenericMenu();
            foreach(var p in projects) {
                var projectName= GetProjectName(p);
                menu.AddItem(new GUIContent(projectName), false, callback, p);
            }
            menu.ShowAsContext();
        }

    }

}