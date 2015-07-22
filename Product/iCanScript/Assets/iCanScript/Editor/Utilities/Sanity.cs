using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;
    
    public class Sanity {
        // -------------------------------------------------------------------------
        /// Validates the default type defined in the uSer Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateVisualScriptBaseType(iCS_IStorage iStorage) {
            if(!iStorage.IsRootObjectAType) return null;
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
                return "First character must start with a letter [a-z][A-Z] or an underscore.";
            }
            foreach(var c in identifier) {
                if(!(Char.IsLetterOrDigit(c) || c == '_')) {
                    return "Valid characters are [a-z][A-Z][0-9] and underscore.";
                }
            }
            return null;
        }

        // =========================================================================
        // FOLDER NAME VALIDATIONS
        // -------------------------------------------------------------------------
        /// Validates the given folder name.
        ///
        /// @param folderName The name to be validated.
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateFolderName(string folderName, bool shortFormat= false) {
            foreach(var c in folderName) {
                if(!(Char.IsLetterOrDigit(c) || c == ' ' || c == '_' || c == '/')) {
                    if(shortFormat) {
                        return "Valid characters are [a-z][A-Z][0-9], space and underscore.";                        
                    }
                    return "The folder name '<color=red>"+folderName+"</color>' includes invalid characters. Valid characters are [a-z][A-Z][0-9], space and underscore.";
                }
            }
            return null;
        }
        
        // =========================================================================
        // TYPE NAME VALIDATIONS
        // -------------------------------------------------------------------------
        /// Validates the visual script type name.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateVisualScriptTypeName(iCS_IStorage iStorage, bool shortFormat= false) {
            if(!iStorage.IsRootObjectAType) return null;
            var typeName= iStorage.TypeName;
            var error= ValidateIdentifier(typeName);
            if(error == null) return null;
            if(shortFormat) {
                return "<b>Type Name '<color=red>"+typeName+"</color>'</b> is invalid. "+error;
            }
            return "The <b>Type Name '<color=red>"+typeName+"</color>'</b> defined in the <b>Visual Script Configuration</b> is invalid. "+error;
        }
    }
    
}
