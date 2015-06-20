using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class FunctionCallOutParameterDefinition : FunctionCallParameterDefinition {
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an output parameter definition.
        ///
        /// @param port The VS port producing the data.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public FunctionCallOutParameterDefinition(iCS_EditorObject port, CodeBase parent)
            : base(port, parent) {}

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the output parameter code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the output parameter.
        ///
        public override string GenerateBody(int indentSize) {
            return "out "+GetNameFor(VSObject);
        }

    }

}