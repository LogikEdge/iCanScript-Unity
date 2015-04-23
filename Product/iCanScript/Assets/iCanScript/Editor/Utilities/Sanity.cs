using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public class Sanity {
        // =========================================================================
        // BASE TYPE VALIDATIONS
        // -------------------------------------------------------------------------
        /// Validates the default type defined in the uSer Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateDefaultBaseType() {
            // -- Return previous message if nothing changed --
            var baseTypeName= Prefs.DefaultBaseType;
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


        // =========================================================================
        // NAMESPACE VALIDATIONS
        // -------------------------------------------------------------------------
        /// Validates that the default namespace follows programmatic conventions.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateDefaultNamespace() {
            return ValidateNamespace(Prefs.DefaultNamespace);
        }

        // -------------------------------------------------------------------------
        /// Validates that the visual script namespace follows programmatic conventions.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateVisualScriptNamespace(iCS_IStorage iStorage) {
            if(iStorage.NamespaceOverride == false) return null;
            return ValidateNamespace(iStorage.Namespace);
        }

        // -------------------------------------------------------------------------
        /// Validates that the given namespace follows programmatic conventions.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateNamespace(string ns) {
            if(string.IsNullOrEmpty(ns)) return null;
            var parts= ns.Split(new Char[1]{'.'});
            foreach(var p in parts) {
                var error= ValidateIdentifier(p);
                if(error != null) {
                    return error;
                }
            }
            return null;
        }

        // -------------------------------------------------------------------------
        /// Verifies for a valid programmatic identifier.
        ///
        /// @param identifier The identifier to validate.
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateIdentifier(string identifier) {
            if(string.IsNullOrEmpty(identifier)) return null;
            var firstLetter= identifier[0];
            if(!(Char.IsLetter(firstLetter) || firstLetter == '_')) {
                return "Invalid first character for an identifier. An identifier must start with a letter [a-z][A-Z] or an underscore.";
            }
            foreach(var c in identifier) {
                if(!(Char.IsLetterOrDigit(c) || c == '_')) {
                    return "Invalid identifier. Valid characters include letters [a-z][A-Z], digits [0-9], or an underscore.";
                }
            }
            return null;
        }

    }
    
}
