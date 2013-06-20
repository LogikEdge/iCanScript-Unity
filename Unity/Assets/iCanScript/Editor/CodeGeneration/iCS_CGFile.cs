using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public static class iCS_CGFile {
    // =================================================================================
    // Fields
    // ---------------------------------------------------------------------------------

    // =================================================================================
    // Builder
    // ---------------------------------------------------------------------------------
    public static string ReadFile(string fileName) {
        string filePath= iCS_Config.CodeGenerationFolder+"/"+fileName;
        string assetPath= Application.dataPath;
        string fullFilePath= assetPath+"/"+filePath;

        if(!File.Exists(fullFilePath)) {
            return null;
        }
        StreamReader sr= null;
        try {
            sr= File.OpenText(fullFilePath);
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
    public static bool WriteFile(string fileName, string fileData) {
        string filePath= iCS_Config.CodeGenerationFolder+"/"+fileName;
        string assetPath= Application.dataPath;
        string fullFilePath= assetPath+"/"+filePath;

        StreamWriter sw= null;
        try {
            sw= File.CreateText(fullFilePath);
            sw.Write(fileData.ToCharArray());
            sw.Close();
            // Create an asset that Unity will recognize...
            AssetDatabase.ImportAsset("Assets/"+filePath);
            return true;
        }
        catch(System.Exception) {
            if(sw != null) {
                sw.Close();
            }
            return false;
        }
    }
}
