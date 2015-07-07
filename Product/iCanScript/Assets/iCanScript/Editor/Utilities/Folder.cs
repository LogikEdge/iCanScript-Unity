using UnityEngine;
using System.IO;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    // %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    /// This class provides common folder conversion utilities.
    public static class Folder {
        // ==================================================================
        /// Converts an asset path to a system path.
        ///
        /// @param assetPath The path relative to the asset folder.
        /// @return The system absolute path of the given assetPath.
        ///
        public static string AssetToAbsolutePath(string assetPath) {
    		// Nothing to change if already in the correct format
    		var systemAssetPath= Application.dataPath;
    		if(assetPath.StartsWith(systemAssetPath)) {
    			return assetPath;
    		}
    		// Convert Unity path to system path.
    		if(assetPath.StartsWith("Assets/")) {
    			assetPath= assetPath.Remove(0, "Assets/".Length);
    		}
    		// Assume we need to add the asset path
    		return systemAssetPath+"/"+assetPath;            
        }

        // ==================================================================
        /// Converts a system path to an Assets relative path.
        ///
        /// @param systemPath The system path to be converted.
        /// @return The _Assets_ relative path.
        ///
		public static string AbsoluteToAssetPath(string systemPath) {
    		var systemAssetPath= Application.dataPath;
			if(!systemPath.StartsWith(systemAssetPath)) {
				Debug.LogWarning("iCanScript: Internal Error: Unable to convert from absolute to Assets path=> "+systemPath);
				return systemPath;
			}
			var assetPath= systemPath.Remove(0, systemAssetPath.Length);
			if(assetPath.StartsWith("/")) {
				assetPath= assetPath.Remove(0, 1);
			}
			return assetPath;
		}
		
        // ==================================================================
        /// Returns _true_ if the given folder is empty.
        ///
        /// @param absolutePath The absolute path of the folder.
        /// @return _true_ if folder is empty.
        ///
        public static bool IsEmpty(string absolutePath) {
            var files= Directory.GetFiles(absolutePath);
            return files == null || files.Length == 0;
        }
    }

}
