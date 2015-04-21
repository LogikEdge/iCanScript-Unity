using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public static class CodeGenerationConfig {

        // ----------------------------------------------------------------------
        /// Gets & Sets the base type to be used for code generation.
        ///
        public static Type BaseType {
            get {
                Type baseType= null;
                // -- Get the default base type from the user preferences. --
                var defaultBaseTypeName= Prefs.CodeGenerationBaseTypeName;
                if(!string.IsNullOrEmpty(defaultBaseTypeName)) {
                    var defaultBaseTypeNamespace= Prefs.CodeGenerationBaseTypeNamespace;
                    baseType= iCS_Types.FindType(defaultBaseTypeName, defaultBaseTypeNamespace);
                    if(baseType == null || baseType == typeof(void)) {
                        Debug.LogWarning("iCanScript: Error: Unable to find default base type.  Please verify base type configured in the Preferences->Code Generation.");
                    }
                }
                return baseType;
            }
        }

    }    
}
