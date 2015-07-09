using UnityEngine;
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
        static ProjectInfo[]	myProjects= null;
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
		public static ProjectInfo[] Projects {
			get { return myProjects; }
		}
		
        // =================================================================================
        // INIT / SHUTDOWN
        // ---------------------------------------------------------------------------------
        static ProjectController()    { UpdateProjectDatabase(); }
        public static void Start()    {}
        public static void Shutdown() {}
        
        // =================================================================================
        /// Creates a project file.
    	[MenuItem("iCanScript/Projects...", false, 80)]
    	public static void OpenProjectWindow() {
            ProjectSettingsEditor.Init();
    	}

        // =================================================================================
        /// Updates the project database from the disk.
		///
		/// @info The project database is made current from the disk.
		///
        public static void UpdateProjectDatabase() {
            var projectPaths= FileUtils.GetFilesWithExtension("icsproject");
			var nbOfProjects= projectPaths.Length;
			myProjects= new ProjectInfo[nbOfProjects];
			for(int i= 0; i < nbOfProjects; ++i) {
				var path= projectPaths[i];
				myProjects[i]= ProjectInfo.Load(path);
			}
			// -- Assure that the longest path is first to simplify serach. --
			Array.Sort(myProjects, (x,y)=> y.GetProjectFolder().Length - x.GetProjectFolder().Length);
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
            var menu= new GenericMenu();
            foreach(var p in myProjects) {
                var projectName= p.ProjectName;
                menu.AddItem(new GUIContent(projectName), false, callback, p);
            }
            menu.ShowAsContext();
        }

        // =================================================================================
		/// Returns the project associated with a Unity Object.
		///
		/// @param iStorage The visual script storage.
		/// @return The project info assicated with the Unity Object.
		///
		public static ProjectInfo GetProjectFor(iCS_IStorage iStorage) {
			var go= iStorage.HostGameObject;
			if(go == null) {
				Debug.LogWarning("iCanScript: Internal Error: Unable to find Game Object of visual script");
				return null;
			}
			string path= null;
			// -- Search for scene path if GO is in the a scene. --
			if(iCS_UnityUtility.IsSceneGameObject(go)) {
				path= EditorApplication.currentScene;
			}
			// -- Search for asset path if GO is a prefab. --
			else {
				path= AssetDatabase.GetAssetPath(go);
			}
			path= Folder.AssetToAbsolutePath(path);
			return GetProjectFor(path);
		}

        // =================================================================================
		/// Returns the project associated with a Unity Object.
		///
		/// @param absolutePath The absolute path of the asset.
		/// @return The project info assicated with the Unity Object.
		///
		public static ProjectInfo GetProjectFor(string absolutePath) {
			foreach(var p in myProjects) {
				if(absolutePath.StartsWith(p.GetProjectFolder())) {
					return p;
				}
			}
			return null;
		}
    }

}