using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

namespace iCanScript.Editor {
    
    public static class TextFileUtils {
        // =================================================================================
        // ---------------------------------------------------------------------------------
    	public static bool Exists(string fileName) {
    		string systemPath= FileUtils.ToSystemPath(fileName);
    		return File.Exists(systemPath);
    	}
        // ---------------------------------------------------------------------------------
    	// Reads and return the text content of the file at the given path.
        public static string ReadFile(string fileName) {
    		string systemPath= FileUtils.ToSystemPath(fileName);
    
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
        // ---------------------------------------------------------------------------------
    	/// Writes the given text into the file at the given path.
        public static bool WriteFile(string fileName, string fileData) {
    		var systemPath= FileUtils.ToSystemPath(fileName);
    
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
        // ---------------------------------------------------------------------------------
    	/// Deletes the given text file.
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