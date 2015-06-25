using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

namespace iCanScript.Internal.Editor {
    
    public static class TextFileUtils {
        // =================================================================================
        /// Determines if the given file exists.
        ///
        /// @param fileName The path of the file to verify.
        /// @return _true_ if file exists. _false_ otherwise.
        ///
    	public static bool Exists(string fileName) {
    		string systemPath= FileUtils.ToSystemPath(fileName);
    		return File.Exists(systemPath);
    	}

        // =================================================================================
    	/// Reads and return the text content of the file at the given path.
        ///
        /// @param fileName The file path inside the Asset folder.
        /// @return The content of the file.
        ///
        public static string ReadAssetFile(string fileName) {
    		string systemPath= FileUtils.ToSystemPath(fileName);
            return ReadFile(systemPath);
        }

        // =================================================================================
    	/// Reads and return the text content of the file at the given path.
        ///
        /// @param systemPath The absolute path of the file to read.
        /// @return The content of the file.
        ///
        public static string ReadFile(string systemPath) {
            if(!File.Exists(systemPath)) {
                return null;
            }
            StreamReader sr= null;
            try {
                sr= File.OpenText(systemPath);
                string fileData= sr.ReadToEnd();
                sr.Close();
                return fileData;
            }
            catch(System.Exception) {
                if(sr != null) {
                    sr.Close();
                }
                return null;
            }
        }

        // =================================================================================
    	/// Writes the given text into the file at the given path.
        ///
        /// @param fileName The file path inside the Asset folder.
        /// @param fileData A string representing the file data.
        ///
        public static bool WriteAssetFile(string fileName, string fileData) {
    		var systemPath= FileUtils.ToSystemPath(fileName);
            return WriteFile(systemPath, fileData);
        }

        // =================================================================================
    	/// Writes the given text into the file at the given path.
        ///
        /// @param systemPath The absolute path of the file to write.
        /// @param fileData A string representing the file data.
        ///
        public static bool WriteFile(string systemPath, string fileData) {
            StreamWriter sw= null;
            try {
                sw= File.CreateText(systemPath);
                sw.Write(fileData.ToCharArray());
                sw.Close();
                // Create an asset that Unity will recognize...
                AssetDatabase.Refresh();
                return true;
            }
            catch(System.Exception) {
                if(sw != null) {
                    sw.Close();
                }
                return false;
            }
        }

        // =================================================================================
    	/// Deletes the given text file.
        ///
        /// @param fileName The file path to verify.  Can be Absolute or Asset relative.
        ///
        public static bool DeleteFile(string fileName) {
    		var systemPath= FileUtils.ToSystemPath(fileName);    
            try {
                File.Delete(systemPath);
                // Ask Unity to refresh its database.
                AssetDatabase.Refresh();
                return true;
            }
            catch(System.Exception) {
                return false;
            }
        }
        // ---------------------------------------------------------------------------------
    	/// Opens the given text file with the configured text editor.
    	public static bool EditFile(string fileName, int lineNb= 1) {
    		var systemPath= FileUtils.ToSystemPath(fileName);
    		var unityPath = FileUtils.ToUnityAssetPath(fileName);
    
    		if(!File.Exists(systemPath)) {
    			return false;
    		}
    		var fileData= AssetDatabase.LoadAssetAtPath(unityPath, typeof(TextAsset));
    		AssetDatabase.OpenAsset(fileData, lineNb);
    		return true;
    	}
    }

}