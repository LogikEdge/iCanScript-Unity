using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public static class CSharpFileUtils {
        const string    kFileExtension= ".cs";

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
        /// Writes or overwrites the CSharp code file.
		///
		/// @param folderPath The absolute path of the code generation folder.
		/// @param className The name of the type/file.
		/// @param code The code to be put into the file.
		///
        public static void WriteCSharpFile(string folderPath, string className, string code) {
            var filePath= folderPath+"/"+className+kFileExtension;
            TextFileUtils.WriteFile(filePath, code);
        }

        // ----------------------------------------------------------------------
        /// Deletes CSharp code file.
		///
		/// @param folderPath The absolute path of the code generation folder.
		/// @param className The name of the type/file.
		///
        public static void DeleteCSharpFile(string folderPath, string className) {
            var filePath= "Assets/"+Folder.AbsoluteToAssetPath(folderPath)+"/"+className+kFileExtension;
            AssetDatabase.DeleteAsset(filePath);
        }
    }

}