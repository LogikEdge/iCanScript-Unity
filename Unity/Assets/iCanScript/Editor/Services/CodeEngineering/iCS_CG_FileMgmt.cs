using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace CodeEngineering {

    public static class iCS_CG_FileMgmt {
        // ----------------------------------------------------------------------
        // Create code generation folder (if it does not exist)
        public static void CreateCodeGenerationFolder(string folderPath) {
            string systemAssetPath= Application.dataPath;
            string systemCodeGenerationFolder= systemAssetPath+"/"+folderPath;
            if(!Directory.Exists(systemCodeGenerationFolder)) {
                AssetDatabase.CreateFolder("Assets",folderPath);
            }        
        }
    
        // ----------------------------------------------------------------------
        // Make Unique Class Name
        public static string MakeUniqueClassName(string desiredClassName, int id= 0) {
            // Build name
            string className= desiredClassName;
            if(id != 0) {
                className+= "_"+id;
            }
            // Scan UnityEngine assembly for a name collision.
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                // Install all public type of the Unity Engine assembly
    //            if(assembly.FullName.StartsWith("UnityEngine")) {
                    foreach(var classType in assembly.GetTypes()) {
                        if(classType.Name == className) {
                            Debug.Log("Class name=> "+classType.FullName+" Assembly=> "+assembly.FullName);
                            return MakeUniqueClassName(desiredClassName, id+1);
                        }
                    }                
    //            }
            }
            return className;
        }
    
        // ----------------------------------------------------------------------
        // Write generated code to given file.
        public static void WriteGeneratedCode(string folderPath, string className, string code) {
            iCS_TextFileUtility.WriteFile("Assets/"+folderPath+"/"+className, code);
        }
    }

} // CodeEnginering