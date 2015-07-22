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
    public static class PackageController {
        // =================================================================================
        // Fields
        // ---------------------------------------------------------------------------------
        static PackageInfo      myDefaultProject= null;
        static PackageInfo[]	myProjects      = null;
        
        // =================================================================================
        // Properties
        // ---------------------------------------------------------------------------------
        /// Returns the list of existing iCanScript projects.
		public static PackageInfo[] Projects {
			get { return myProjects; }
		}
        // =================================================================================
        /// Returns the default iCanScript project for this Unity project.
        public static PackageInfo DefaultProject {
            get {
                if(myDefaultProject == null) {
                    myDefaultProject= new PackageInfo();
                    myDefaultProject.Save();            
                }
                return myDefaultProject;                
            }
        }
		
        // =================================================================================
        // INIT / SHUTDOWN
        // ---------------------------------------------------------------------------------
        static PackageController()    { UpdatePackageDatabase(); }
        public static void Start()    {}
        public static void Shutdown() {}
        
        // =================================================================================
        /// Creates a project file.
    	[MenuItem("iCanScript/Packages...", false, 80)]
    	public static void OpenPackageWindow() {
            PackageSelectionWindow.Init();
    	}

        // =================================================================================
        /// Updates the project database from the disk.
		///
		/// @info The project database is made current from the disk.
		///
        public static void UpdatePackageDatabase() {
            var projectPaths= FileUtils.GetFilesWithExtension("icspackage");
			var nbOfProjects= projectPaths.Length;
			myProjects= new PackageInfo[nbOfProjects];
			for(int i= 0; i < nbOfProjects; ++i) {
				var path= projectPaths[i];
				myProjects[i]= PackageInfo.Load(path);
			}
			// -- Add default project if it does not exist. --
			bool rootProjectFound= false;
			foreach(var p in myProjects) {
				if(p.IsRootPackage) {
					rootProjectFound= true;
				}
			}
			if(!rootProjectFound) {
				var rootProject= new PackageInfo();
				rootProject.Save();
				Array.Resize(ref myProjects, myProjects.Length+1); 
				myProjects[myProjects.Length-1]= rootProject;
			}
            // -- Rebuild package hierarchy. --
            for(int i= 0; i < projectPaths.Length; ++i) {
                var pi= Path.GetDirectoryName(projectPaths[i]);
                if(myProjects[i].IsNested) {
                    for(int j= i-1; j >= 0; --j) {
                        var pj= Path.GetDirectoryName(projectPaths[j]);
                        if(pi.StartsWith(pj)) {
                            myProjects[i].ParentPackage= myProjects[j];
                            break;
                        }
                    }
                    myProjects[i].SetPackageFolder(pi);                    
                }
            }
			// -- Assure that the longest path is first to simplify search. --
			Array.Sort(myProjects, (x,y)=> y.GetPackageFolder().Length - x.GetPackageFolder().Length);
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
                var packageName= p.PackageName;
                menu.AddItem(new GUIContent(packageName), false, callback, p);
            }
            menu.ShowAsContext();
        }

        // =================================================================================
        /// Determines if the given package has child package(s).
        ///
        /// @param package The package to verify.
        /// @return _true_ if the given package has one or more children. _false_ otherwise.
        ///
        public static bool HasChildPackage(PackageInfo package) {
            foreach(var p in myProjects) {
                if(p.ParentPackage == package) {
                    return true;
                }
            }
            return false;
        }
        
        // =================================================================================
		/// Returns the project associated with a Unity Object.
		///
		/// @param iStorage The visual script storage.
		/// @return The project info assicated with the Unity Object.
		///
		public static PackageInfo GetProjectFor(iCS_IStorage iStorage) {
			var go= iStorage.HostGameObject;
			if(go == null) {
				Debug.LogWarning("iCanScript: Internal Error: Unable to find Game Object of visual script");
				return null;
			}
			string path= null;
			// -- Search for scene path if GO is in the a scene. --
			if(UnityUtility.IsSceneGameObject(go)) {
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
		public static PackageInfo GetProjectFor(string absolutePath) {
			foreach(var p in myProjects) {
				if(absolutePath.StartsWith(p.GetPackageFolder())) {
					return p;
				}
			}
			return DefaultProject;
		}

    }

}