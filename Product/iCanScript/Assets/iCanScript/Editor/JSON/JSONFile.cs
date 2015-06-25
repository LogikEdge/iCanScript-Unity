using UnityEngine;
using System.Collections;
using iCanScript.Internal.Editor;

namespace iCanScript.Internal.JSON {
    
    public static class JSONFile {
        // =================================================================================
    	/// Writes the given text into the file at the given path.
        ///
        /// @param filePath The file path to verify.  Can be Absolute or Asset relative.
        /// @param fileData An object to be encoded in JSON format.
        /// @return _true_ if the file was written. _false_ otherwise.
        ///
        public static bool Write(string filePath, System.Object fileData) {
            var jsonRoot= JObject.Build(fileData);
            var jsonData= jsonRoot.Encode();
            return TextFileUtils.WriteFile(filePath, jsonData);
        }

        // =================================================================================
    	/// Writes using JSON pretty print the given text into the file at the given path.
        ///
        /// @param filePath The file path to verify.  Can be Absolute or Asset relative.
        /// @param fileData An object to be encoded in JSON format.
        /// @return _true_ if the file was written. _false_ otherwise.
        ///
        public static bool PrettyWrite(string filePath, System.Object fileData) {
            var jsonRoot= JObject.Build(fileData);
            var jsonData= JSONPrettyPrint.Print(jsonRoot.Encode(), 80);
            return TextFileUtils.WriteFile(filePath, jsonData);
        }

    }

}
