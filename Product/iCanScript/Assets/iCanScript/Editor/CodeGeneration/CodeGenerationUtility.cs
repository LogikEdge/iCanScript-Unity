using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor {
    using Prefs= PreferencesController;
    
    public static class CodeGenerationUtility {

        // ======================================================================
        // BASE TYPE UTILITIES
        // ----------------------------------------------------------------------
        /// Gets and Sets the base type to be used for code generation.
        ///
        /// @param iStorage The visual script storage.
        /// @return The base type or _null_ if base type is not valid.
        ///
        public static Type GetBaseType(iCS_IStorage iStorage) {
            Type baseType= null;
            // -- Get the default base type from the user preferences. --
            var baseTypeString= GetBaseTypeString(iStorage);
            if(!string.IsNullOrEmpty(baseTypeString)) {
                baseType= iCS_Types.GetTypeFromTypeString(baseTypeString);
            }
            else {
                baseType= typeof(void);
            }
            return baseType;
        }

        // ----------------------------------------------------------------------
        /// Get base type string
        ///
        /// @param iStorage The visual script storage.
        /// @return The base type string or _null_ if not found.
        ///
        public static string GetBaseTypeString(iCS_IStorage iStorage) {
            if(iStorage.BaseTypeOverride) {
                return iStorage.BaseType;
            }
            return Prefs.EngineBaseType;
        }
        
        // ======================================================================
        // NAMESPACE UTILITIES
        // ----------------------------------------------------------------------
        /// Returns the default namespace according to the type of viusal script.
        ///
        /// @param iStorage The visual script storage.
        /// @return The default namespace.
        ///
        public static string GetNamespace(iCS_IStorage iStorage) {
            var project= ProjectController.ActiveProject;
            return iStorage.IsEditorScript ? project.EditorNamespace : project.Namespace;
        }

        // ======================================================================
        // NAMESPACE UTILITIES
        // ----------------------------------------------------------------------
        public static string GetCodeGenerationFolder(iCS_IStorage iStorage) {
            var project= ProjectController.ActiveProject;
            if(iStorage.IsEditorScript) {
                return project.GetRelativeEditorCodeGenerationFolder();
            }
            return project.GetRelativeEngineCodeGenerationFolder();
        }
    }    
}
