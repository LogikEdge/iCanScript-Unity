using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace iCanScriptEditor { namespace CodeEngineering {

    public static class CSharpFileUtils {

        // ----------------------------------------------------------------------
        // Verify the consistancy of the source file
        public static void VerifyAndCorrectSourceFile(iCS_IStorage iStorage) {
            if(String.IsNullOrEmpty(iStorage.Storage.SourceFile)) {
                iStorage.Storage.SourceFile= MakeUniqueClassName(iStorage[0].name);
                // TODO: Queue creation of source file
            }
            // TODO: verify the existance of the source file and create it
        }
        
        // ----------------------------------------------------------------------
        // Generate a default source file name
        public static void GenerateDefaultSourceFile(iCS_VisualScriptData vsd) {
            if(vsd.SourceFile != null) return;
//            vsd.SourceFile= MakeUniqueClassName(vsd.name);
        }
        
        // ----------------------------------------------------------------------
        // Make Unique Class Name
        public static string MakeUniqueClassName(string desiredClassName, int id= 0) {
            // Build name
            string className= desiredClassName;
            if(id != 0) {
                className+= "_"+id;
            }
            // Search all assemblies...
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            // Search for an existing class with the same name.
                foreach(var classType in assembly.GetTypes()) {
                    if(classType.Name == className) {
                        return MakeUniqueClassName(desiredClassName, id+1);
                    }
                }                
            }
            return className;
        }
    
        // ----------------------------------------------------------------------
        // Write generated code to given file.
        public static void WriteCSharpFile(string folderPath, string className, string code) {
            TextFileUtils.WriteFile("Assets/"+folderPath+"/"+className+".cs", code);
        }
    }

}}