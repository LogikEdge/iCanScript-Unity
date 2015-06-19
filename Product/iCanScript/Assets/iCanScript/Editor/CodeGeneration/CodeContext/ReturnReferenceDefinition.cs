using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ReturnReferenceDefinition : CodeBase {
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a return variable reference.
        ///
        /// @param vsObject Visual Script return port.
        /// @param parent The parent code context.
        /// @return The newly created return variable.
        ///
        public ReturnReferenceDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {}
    
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the return variable reference code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the return variable reference.
        ///
        public override string GenerateBody(int indentSize) {
            return GetNameFor(VSObject);
        }
    }
}
