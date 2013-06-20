using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_CETextFile {
    // =================================================================================
    // ---------------------------------------------------------------------------------
	// Reads and return the text content of the file at the given path.
    public static string ReadFile(string fileName) {
		return iCS_TextFile.ReadFile(iCS_Config.CodeGenerationFolder+"/"+fileName);
    }
    // ---------------------------------------------------------------------------------
	// Write the given text into the file at the given path.
    public static bool WriteFile(string fileName, string fileData) {
		return iCS_TextFile.WriteFile(iCS_Config.CodeGenerationFolder+"/"+fileName, fileData);
    }
    // ---------------------------------------------------------------------------------
	// Opens the given text file with the configured text editor.
	public static bool EditFile(string fileName, int lineNb= 1) {
		return iCS_TextFile.EditFile(iCS_Config.CodeGenerationFolder+"/"+fileName, lineNb);
	}
}
