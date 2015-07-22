using UnityEngine;
using System.Collections;
using iCanScript.Internal.Editor;

namespace iCanScript.Internal.JSON {
    
    public static class JSONFile {
        // =================================================================================
    	/// Writes the given object into the file at the given path.
        ///
        /// @param filePath The file path in which to write.
        /// @param fileData An object to be encoded in JSON format.
        /// @return _true_ if the file was written. _false_ otherwise.
        ///
        public static bool Write(string filePath, System.Object fileData) {
            var jsonRoot= JObject.Build(fileData);
            var jsonData= jsonRoot.Encode();
            return TextFileUtils.WriteFile(filePath, jsonData);
        }

        // =================================================================================
    	/// Writes using JSON pretty print the given object into the file at the given path.
        ///
        /// @param filePath The file path in which to write.
        /// @param fileData An object to be encoded in JSON format.
        /// @return _true_ if the file was written. _false_ otherwise.
        ///
        public static bool PrettyWrite(string filePath, System.Object fileData) {
            var jsonRoot= JObject.Build(fileData);
            var jsonData= JSONPrettyPrint.Print(jsonRoot.Encode(), 80);
            return TextFileUtils.WriteFile(filePath, jsonData);
        }

        // =================================================================================
    	/// Reads the JSON root object from the given file path.
        ///
        /// @param filePath The file path from which to read.
        /// @return The JSON root object. _Null_ is return on error.
        ///
        public static JObject Read(string filePath, bool declareError= false) {
            var jsonData= TextFileUtils.ReadFile(filePath);
            if(string.IsNullOrEmpty(jsonData)) {
                if(declareError) {
                    Debug.LogWarning("iCanScript: Unable to read file at=> "+filePath);                    
                }
                return null;
            }
            // -- Decode JSON string. --
            JObject root= JSON.GetRootObject(jsonData);
            if(root.isNull) {
                if(declareError) {
                    Debug.LogWarning("iCanScript: JSON file corruption at=> "+filePath);                    
                }
                return null;
            }
            return root;
        }
    }

}
