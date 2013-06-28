using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_CETextFile {
    // =================================================================================
    // ---------------------------------------------------------------------------------
	public static bool Exists(string fileName) {
		return iCS_TextFile.Exists(iCS_PreferencesEditor.CodeGenerationFolder+"/"+fileName);
	}
    // ---------------------------------------------------------------------------------
	// Reads and return the text content of the file at the given path.
    public static string ReadFile(string fileName) {
		return iCS_TextFile.ReadFile(iCS_PreferencesEditor.CodeGenerationFolder+"/"+fileName);
    }
    // ---------------------------------------------------------------------------------
	// Write the given text into the file at the given path.
    public static bool WriteFile(string fileName, string fileData) {
		return iCS_TextFile.WriteFile(iCS_PreferencesEditor.CodeGenerationFolder+"/"+fileName, fileData);
    }
    // ---------------------------------------------------------------------------------
	// Opens the given text file with the configured text editor.
	public static bool EditFile(string fileName, int lineNb= 1) {
		return iCS_TextFile.EditFile(iCS_PreferencesEditor.CodeGenerationFolder+"/"+fileName, lineNb);
	}
}
