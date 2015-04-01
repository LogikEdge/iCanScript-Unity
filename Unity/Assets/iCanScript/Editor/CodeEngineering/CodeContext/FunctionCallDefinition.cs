using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionCallDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------

        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a function call code context.
        ///
        /// @param vsObj VS node associated with the function call.
        /// @return The newly created function call definition.
        ///
        public FunctionCallDefinition(iCS_EditorObject vsObj)
        : base(CodeType.FUNCTION_CALL) {
            
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        public override string GenerateCode(int indentSize) {
            var result= new StringBuilder(256);
            return result.ToString();
        }
    }

}