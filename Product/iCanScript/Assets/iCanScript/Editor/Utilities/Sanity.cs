using UnityEngine;
using System.Collections;

namespace iCanScript.Editor {

    public class Sanity {
        // -------------------------------------------------------------------------
        /// Validates the default type defined in the uSer Preferences.
        ///
        /// @return A user message if a problem is found. _null_ otherwise.
        ///
        public static string ValidateDefaultBaseType() {
            var baseTypeName= PreferencesController.CodeGenerationBaseTypeName;
            if(baseTypeName == c_codeGenerationBaseTypeName) {
                return c_validateDefaultBaseTypeMessage;
            }
            c_codeGenerationBaseTypeName= baseTypeName;
            if(CodeGenerationConfig.BaseType != null) {
                c_validateDefaultBaseTypeMessage= null;
                return null;
            }
            c_validateDefaultBaseTypeMessage=
                "Unable to find the <b>Base Type</b> <color=red><b>"+baseTypeName+
                "</b></color> defined in the <b>User Preferences</b>";
            return c_validateDefaultBaseTypeMessage;
        }
        private static string c_codeGenerationBaseTypeName    = null;
        private static string c_validateDefaultBaseTypeMessage= null;
    }
    
}
