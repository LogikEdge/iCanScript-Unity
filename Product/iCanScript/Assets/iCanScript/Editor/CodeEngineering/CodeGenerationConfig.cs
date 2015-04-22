using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public static class CodeGenerationConfig {

        // ----------------------------------------------------------------------
        /// Gets and Sets the base type to be used for code generation.
        ///
        /// @return The base type or _null_ if base type is not valid.
        ///
        public static Type BaseType {
            get {
                Type baseType= null;
                // -- Get the default base type from the user preferences. --
                var baseTypeString= BaseTypeString;
                if(!string.IsNullOrEmpty(baseTypeString)) {
                    baseType= iCS_Types.GetTypeFromTypeString(baseTypeString);
                }
                else {
                    baseType= typeof(void);
                }
                return baseType;
            }
        }

        // ----------------------------------------------------------------------
        /// Get base type string
        public static string BaseTypeString {
            get {
                return Prefs.CodeGenerationBaseTypeName;
            }
        }
    }    
}
