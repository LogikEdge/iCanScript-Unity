using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_TextFile {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------

    // =================================================================================
    // ---------------------------------------------------------------------------------
	public static bool Exists(string fileName) {
		string systemPath= ToSystemPath(fileName);
		return File.Exists(systemPath);
	}
    // ---------------------------------------------------------------------------------
	// Reads and return the text content of the file at the given path.
    public static string ReadFile(string fileName) {
		string systemPath= ToSystemPath(fileName);

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
		var systemPath= ToSystemPath(fileName);
		var unityPath= ToUnityAssetPath(fileName);

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
		var systemPath= ToSystemPath(fileName);
		var unityPath= ToUnityAssetPath(fileName);

		if(!File.Exists(systemPath)) {
			return false;
		}
		var fileData= AssetDatabase.LoadAssetAtPath(unityPath, typeof(TextAsset));
		AssetDatabase.OpenAsset(fileData, lineNb);
		return true;
	}

    // =================================================================================
	// File path utilities.
    // ---------------------------------------------------------------------------------
    public static string ToClassName(string proposedName) {
        string fileName= "";
        foreach(var c in proposedName) {
            if(!Char.IsWhiteSpace(c)) {
                fileName+= (c == '_' || Char.IsLetterOrDigit(c)) ? c : '_';                
            }
        }
        return fileName;
    }
    // ---------------------------------------------------------------------------------
	public static string ToUnityAssetPath(string path) {
		// Nothing to change if already in the correct format
		if(path.StartsWith("Assets/")) {
			return path;
		}
		// Convert system path to unity path
		var systemAssetPath= Application.dataPath;
		if(path.StartsWith(systemAssetPath)) {
			path.Remove(0, systemAssetPath.Length);
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
			path.Remove(0, "Assets/".Length);
		}
		// Assume we need to add the asset path
		return systemAssetPath+"/"+path;
	}
}
