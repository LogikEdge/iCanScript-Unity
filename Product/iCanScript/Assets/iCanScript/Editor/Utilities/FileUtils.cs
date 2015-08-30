using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class FileUtils {

        // ======================================================================
        // PROPERTIES
        // ----------------------------------------------------------------------
        public static string SystemAssetPath {
            get { return Application.dataPath; }
        }
        
        // ======================================================================
        /// Finds all files with a given extension.
        ///
        /// @param extension The extension to search for.
        /// @param relativePath The folder path relative to the Asset folder.
        /// @return The list of files matching the request.
        ///
        public static string[] GetFilesWithExtension(string extension, string relativePath= null) {
            var path= SystemAssetPath;
            if(!String.IsNullOrEmpty(relativePath)) {
                path+= "/"+relativePath;
            }
            var paths= Directory.GetFiles(path, "*."+extension, SearchOption.AllDirectories);
            for(int i= 0; i < paths.Length; ++i) {
                paths[i]= paths[i].Replace('\\', '/');
            }
            return paths;
        }
        
        // ======================================================================
		/// Make path relative to asset folder.
		///
		/// @param path The path to convert.
		/// @return The path relative to the Unity asset folder.
		///
		public static string MakeRelativeToAssetFolder(string path) {
			int idx= path.LastIndexOf("Assets");
			if(idx < 0) {
				Debug.LogWarning("iCanScript: Unable to locate asset folder!");
				return path;
			}
			idx+= 7;
			return path.Substring(idx, path.Length-idx);
		}

        // ======================================================================
		/// Get the paths of all visual scripts in the project.
		///
		/// @return A list of paths to the all visual scripts of this project.
		///
		public static string[] GetPathsOfAllVisualScripts() {
			var visualScripts= GetFilesWithExtension("ics2");
			var len= visualScripts.Length;
			var paths= new string[len];
			for(int i= 0; i < len; ++i) {
				paths[i]= MakeRelativeToAssetFolder(visualScripts[i]);
			}
			return paths;
		}
		
        // ----------------------------------------------------------------------
        /// Create a subfolder under the project Assets folder
		/// (if it does not exist).
		///
		/// @param folderPath A path relative to the 'Assets' folder.
		///
        public static void CreateAssetFolder(string folderPath) {
            // -- Split up folder into its parts --
            var folders= folderPath.Split(new Char[1]{'/'});
            var len= folders.Length;
            if(len == 0) return;

            // -- Create the folder recursively --
            string systemAssetPath= Application.dataPath;
            string assetPath= "Assets";
            for(int i= 0; i < len; ++i) {
                var leafFolder= folders[i];
                systemAssetPath+= "/"+leafFolder;
                if(!Directory.Exists(systemAssetPath)) {
                    AssetDatabase.CreateFolder(assetPath, leafFolder);
                }        
                assetPath+= "/"+leafFolder;
            }
        }
    

        // ---------------------------------------------------------------------------------
        public static void ChangeRecursivelyAssetsPrefixInDirectory(string dir, string oldPrefix, string newPrefix) {
            ChangeAssetsPrefixInDirectory(dir, oldPrefix, newPrefix);
            dir= ToSystemPath(dir);
            var directories= Directory.GetDirectories(dir);
            foreach(string directory in directories) {
                ChangeRecursivelyAssetsPrefixInDirectory(directory, oldPrefix, newPrefix);
            }        
        }
        
        // ---------------------------------------------------------------------------------
        public static void ChangeAssetsPrefixInDirectory(string dir, string oldPrefix, string newPrefix) {
            dir= ToSystemPath(dir);
            var files= Directory.GetFiles(dir);
            foreach(string file in files) {
                if(!file.EndsWith(".meta")) {
                    ChangeAssetPrefix(file, oldPrefix, newPrefix);                
                }
            }
        }
        
        // ---------------------------------------------------------------------------------
        public static bool ChangeAssetPrefix(string assetPath, string oldPrefix, string newPrefix) {
            if(oldPrefix == newPrefix) return false;
            assetPath= ToUnityAssetPath(assetPath);
            var newFileName= Path.GetFileNameWithoutExtension(assetPath);
            if(newFileName.StartsWith(oldPrefix)) {
                var oldPrefixLen= oldPrefix.Length;
                newFileName= newFileName.Substring(oldPrefixLen, newFileName.Length-oldPrefixLen);
            }
            newFileName= newPrefix+newFileName;
            if(!string.IsNullOrEmpty(AssetDatabase.RenameAsset(assetPath, newFileName))) {
                EditorUtility.DisplayDialog("Failed to rename asset !!!", "Could not rename the asset: "+assetPath+" to "+newFileName+".", "Continue");
                return false;
            }
            return true;
        }
    
        // =================================================================================
    	// File path utilities.    
        // ---------------------------------------------------------------------------------
    	public static string ToUnityAssetPath(string path) {
    		// Nothing to change if already in the correct format
    		if(path.StartsWith("Assets/")) {
    			return path;
    		}
    		// Convert system path to unity path
    		var systemAssetPath= Application.dataPath;
    		if(path.StartsWith(systemAssetPath)) {
    			path= path.Remove(0, systemAssetPath.Length+1);
    		}
    		// Assume that the Assets folder is missing.
    		return "Assets/"+path;
    	}
        // ---------------------------------------------------------------------------------
    	public static string ToSystemPath(string path) {
    		// Nothing to change if already in the correct format
    		var systemAssetPath= Application.dataPath;
    		if(path.StartsWith(systemAssetPath)) {
    			return path;
    		}
    		// Convert Unity path to system path.
    		if(path.StartsWith("Assets/")) {
    			path= path.Remove(0, "Assets/".Length);
    		}
    		// Assume we need to add the asset path
    		return systemAssetPath+"/"+path;
    	}
    }
    
}