using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public static class CodeGenerationConfig {

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
            return Prefs.GlobalBaseType;
        }
    }    
}
