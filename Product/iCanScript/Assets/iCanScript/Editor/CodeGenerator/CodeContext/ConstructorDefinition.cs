using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ConstructorDefinition : FunctionCallDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a function call code context.
        ///
        /// @param vsObj VS node associated with the function call.
        /// @param codeBlock The code block this assignment belongs to.        
        /// @return The newly created function call definition.
        ///
        public ConstructorDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {}
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            var result= new StringBuilder(128);
            result.Append("new ");
            result.Append(base.GenerateBody(0));
            return result.ToString();
        }
    }

}