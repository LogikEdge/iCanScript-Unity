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
        	return iStorage.BaseType;
        }
        
        // ----------------------------------------------------------------------
		/// Get default base type for the engine code.
		///
		/// @return A type name string representing the engine base type.
		///
		public static string DefaultEngineBaseTypeString {
			get { return "UnityEngine.MonoBehaviour"; }
		}
		
        // ----------------------------------------------------------------------
		/// Get default base type for the editor code.
		///
		/// @return A type name string representing the engine base type.
		///
		public static string DefaultEditorBaseTypeString {
			get { return "UnityEditor.EditorWindow"; }
		}
		
        // ======================================================================
        // NAMESPACE UTILITIES
        // ----------------------------------------------------------------------
        /// Get namespace used for code generation.
        ///
        /// @param iStorage The visual script storage.
        /// @return The namespace.
        ///
        public static string GetNamespace(iCS_IStorage iStorage) {
			var project= PackageController.GetProjectFor(iStorage);
			if(project == null) return null;
			if(iStorage.IsEditorScript) {
				return project.EditorNamespace;
			}
        	return project.EngineNamespace;
        }

        // ======================================================================
        // NAMESPACE UTILITIES
        // ----------------------------------------------------------------------
        public static string GetCodeGenerationFolder(iCS_IStorage iStorage) {
			var project= PackageController.GetProjectFor(iStorage);
			if(project == null) return null;
            if(iStorage.IsEditorScript) {
                return project.GetEditorGeneratedCodeFolder(true);
            }
            return project.GetEngineGeneratedCodeFolder(true);
        }
    }    
}
