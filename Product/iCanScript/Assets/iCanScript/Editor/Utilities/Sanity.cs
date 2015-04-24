using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public class Sanity {
        // =========================================================================
        // BASE TYPE VALIDATIONS
        // -------------------------------------------------------------------------
        /// Validates the engine base type defined in the Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateEngineBaseType() {
            // -- Return previous message if nothing changed --
            var baseTypeName= Prefs.EngineBaseType;
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
                "Unable to locate the <b>Engine Base Type</b> <color=red><b>"+baseTypeName+
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
        /// Validates that the default engine namespace.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateEngineNamespace(bool shortFormat= false) {
            var ns= Prefs.EngineNamespace;
            var error= ValidateNamespace(ns);
            if(error == null) return null;
            if(shortFormat) {
                return "Invalid namespace format: "+error;
            }
            return "Invalid <b>Engine Namespace '<color=red>"+ns+"</color>'</b> defined in <b>Global Preferences</b>: "+error;
        }
        // -------------------------------------------------------------------------
        /// Validates that the default editor namespace.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateEditorNamespace(bool shortFormat= false) {
            var ns= Prefs.EditorNamespace;
            var error= ValidateNamespace(ns);
            if(error == null) return null;
            if(shortFormat) {
                return "Invalid namespace format: "+error;
            }
            return "Invalid <b>Editor Namespace '<color=red>"+ns+"</color>'</b> defined in <b>Global Preferences</b>: "+error;
        }

        // -------------------------------------------------------------------------
        /// Validates that the visual script namespace follows programmatic conventions.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateVisualScriptNamespace(iCS_IStorage iStorage, bool shortFormat= false) {
            if(iStorage.NamespaceOverride == false) return null;
            var ns= iStorage.Namespace;
            var error= ValidateNamespace(ns);
            if(error == null) return null;
            if(shortFormat) {
                return "Invalid namespace format: "+error;
            }
            return "Invalid <b>Namespace '<color=red>"+ns+"</color>'</b> defined in the <b>Visual Script Configuration</b>: "+error;
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
        /// Validates the editor code generation folder from Global Preferences.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateEditorCodeGenerationFolder(bool shortFormat= false) {
            var folder= Prefs.EditorCodeGenerationFolder;
            var pathParts= folder.Split(new Char[1]{'/'});
            bool isEditorFolderFound= false;
            foreach(var p in pathParts) {
                if(p == "Editor") isEditorFolderFound= true;
            }
            if(!isEditorFolderFound) {
                if(shortFormat) {
                    return "<b>'<color=red>"+folder+"</color>'</b> MUST include <b>'Editor'</b> in its path.";                    
                }
                return "The <b>Editor Code Generation Folder '<color=red>"+folder+"</color>'</b> defined in <b>Global Preferences</b> MUST include <b>'Editor'</b> in its path.";
            }
            var error= ValidateFolderName(folder, true);
            if(error == null) return null;
            if(shortFormat) {
                return "<b>'<color=red>"+folder+"</color>'</b> includes invalid characters. "+error;                
            }
            return "The <b>Editor Code Generation Folder '<color=red>"+folder+"</color>'</b> defined in <b>Global Preferences</b> includes invalid characters. "+error;
        }

        // -------------------------------------------------------------------------
        /// Validates the engine code generation folder from Global Preferences.
        ///
        /// @return The error message or _null_ if no error found.
        ///
        public static string ValidateEngineCodeGenerationFolder(bool shortFormat= false) {
            // -- Verify that the 'Editor' path is not included --
            var folder= Prefs.EngineCodeGenerationFolder;
            var pathParts= folder.Split(new Char[1]{'/'});
            bool isEditorFolderFound= false;
            foreach(var p in pathParts) {
                if(p == "Editor") isEditorFolderFound= true;
            }
            if(isEditorFolderFound) {
                if(shortFormat) {
                    return "<b>'<color=red>"+folder+"</color>'</b> CAN NOT include <b>'Editor'</b> in its path.";                    
                }
                return "The <b>Engine Code Generation Folder '<color=red>"+folder+"</color>'</b> defined in <b>Global Preferences</b> CAN NOT include <b>'Editor'</b> in its path.";
            }
            var error= ValidateFolderName(folder, true);
            if(error == null) return null;
            if(shortFormat) {
                return "<b>'<color=red>"+folder+"</color>'</b> includes invalid characters. "+error;                
            }
            return "<b>Engine Code Generation Folder '<color=red>"+folder+"</color>'</b> defined in <b>Global Preferences</b> includes invalid characters. "+error;
        }

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
