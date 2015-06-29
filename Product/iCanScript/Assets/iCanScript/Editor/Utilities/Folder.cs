﻿using UnityEngine;
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
