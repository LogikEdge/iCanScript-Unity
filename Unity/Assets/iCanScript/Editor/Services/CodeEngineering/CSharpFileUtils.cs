using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace iCanScriptEditor { namespace CodeEngineering {

    public static class CSharpFileUtils {

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