using UnityEngine;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public class Sanity {
        // -------------------------------------------------------------------------
        /// Validates the default type defined in the uSer Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateDefaultBaseType() {
            // -- Return previous message if nothing changed --
            var baseTypeName= Prefs.GlobalBaseType;
            if(baseTypeName == c_codeGenerationBaseTypeName) {
                return c_codeGenerationBaseTypeMessage;
            }
            c_codeGenerationBaseTypeName= baseTypeName;
            // -- Accept no base type --
            if(string.IsNullOrEmpty(baseTypeName)) {
                c_codeGenerationBaseTypeMessage= null;
                return null;
            }
            // -- Attempt to find the base type inside the application --
            if(iCS_Types.GetTypeFromTypeString(baseTypeName) != null) {
                c_codeGenerationBaseTypeMessage= null;
                return null;
            }
            // -- Base type not found; generate error message --
            c_codeGenerationBaseTypeMessage=
                "Unable to locate the <b>Default Base Type</b> <color=red><b>"+baseTypeName+
                "</b></color> configured in the <b>Global Preferences</b>";
            return c_codeGenerationBaseTypeMessage;
        }
        private static string c_codeGenerationBaseTypeName    = null;
        private static string c_codeGenerationBaseTypeMessage= null;

        // -------------------------------------------------------------------------
        /// Validates the default type defined in the uSer Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateVisualScriptBaseType(iCS_IStorage iStorage) {
            // -- Don't validate if not enabled --
            if(iStorage.BaseTypeOverride == false) return null;
            // -- Return previous message if nothing changed --
            var baseTypeName= iStorage.BaseType;
            if(baseTypeName == c_visualScriptBaseTypeName) {
                return c_visualScriptBaseTypeMessage;
            }
            c_visualScriptBaseTypeName= baseTypeName;
            // -- Accept no base type --
            if(string.IsNullOrEmpty(baseTypeName)) {
                c_visualScriptBaseTypeMessage= null;
                return null;
            }
            // -- Attempt to find the base type inside the application --
            if(iCS_Types.GetTypeFromTypeString(baseTypeName) != null) {
                c_visualScriptBaseTypeMessage= null;
                return null;
            }
            // -- Base type not found; generate error message --
            c_visualScriptBaseTypeMessage=
                "Unable to locate the <b>Base Type</b> <color=red><b>"+baseTypeName+
                "</b></color> configured in the <b>Visual Script Configuration</b>";
            return c_visualScriptBaseTypeMessage;
        }
        private static string c_visualScriptBaseTypeName   = null;
        private static string c_visualScriptBaseTypeMessage= null;

    }
    
}
