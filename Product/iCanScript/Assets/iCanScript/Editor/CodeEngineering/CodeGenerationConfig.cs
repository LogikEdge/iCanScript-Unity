using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Editor {
    using Prefs= PreferencesController;
    
    public static class CodeGenerationConfig {

        // ----------------------------------------------------------------------
        /// Gets and Sets the base type to be used for code generation.
        ///
        public static Type BaseType {
            get {
                Type baseType= null;
                // -- Get the default base type from the user preferences. --
                var baseTypeString= Prefs.CodeGenerationBaseTypeName;
                if(!string.IsNullOrEmpty(baseTypeString)) {
                    string typeName;
                    string namespaceName;
                    baseType= iCS_Types.GetTypeInfoFromTypeString(baseTypeString, out namespaceName, out typeName);
                    if(baseType == null || baseType == typeof(void)) {
                        Debug.LogWarning("iCanScript: Error: Unable to find default base type: "+baseTypeString+".  Please verify base type configured in the Preferences->Code Generation.");
                        Debug.Log("NS=> "+namespaceName+" TN=> "+typeName);
                    }
                }
                return baseType;
            }
        }

    }    
}
