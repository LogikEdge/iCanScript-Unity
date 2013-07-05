using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;

public static class iCS_TextFileUtility {
    // =================================================================================
    // ---------------------------------------------------------------------------------
	public static bool Exists(string fileName) {
		string systemPath= iCS_FileUtility.ToSystemPath(fileName);
		return File.Exists(systemPath);
	}
    // ---------------------------------------------------------------------------------
	// Reads and return the text content of the file at the given path.
    public static string ReadFile(string fileName) {
		string systemPath= iCS_FileUtility.ToSystemPath(fileName);

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
	// Write the given text into the file at the given path.
    public static bool WriteFile(string fileName, string fileData) {
		var systemPath= iCS_FileUtility.ToSystemPath(fileName);
		var unityPath= iCS_FileUtility.ToUnityAssetPath(fileName);

        StreamWriter sw= null;
        try {
            sw= File.CreateText(systemPath);
            sw.Write(fileData.ToCharArray());
            sw.Close();
            // Create an asset that Unity will recognize...
            AssetDatabase.ImportAsset(unityPath);
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
	// Opens the given text file with the configured text editor.
	public static bool EditFile(string fileName, int lineNb= 1) {
		var systemPath= iCS_FileUtility.ToSystemPath(fileName);
		var unityPath= iCS_FileUtility.ToUnityAssetPath(fileName);

		if(!File.Exists(systemPath)) {
			return false;
		}
		var fileData= AssetDatabase.LoadAssetAtPath(unityPath, typeof(TextAsset));
		AssetDatabase.OpenAsset(fileData, lineNb);
		return true;
	}
}
