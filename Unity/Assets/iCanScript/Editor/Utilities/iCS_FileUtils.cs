using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

namespace iCanScriptEditor {
    
    public static class FileUtils {

        // ----------------------------------------------------------------------
        // Create a subfolder under the project Assets folder (if it does not exist)
        public static void CreateAssetFolder(string folderPath) {
            string systemAssetPath= Application.dataPath;
            string systemCodeGenerationFolder= systemAssetPath+"/"+folderPath;
            if(!Directory.Exists(systemCodeGenerationFolder)) {
                AssetDatabase.CreateFolder("Assets",folderPath);
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