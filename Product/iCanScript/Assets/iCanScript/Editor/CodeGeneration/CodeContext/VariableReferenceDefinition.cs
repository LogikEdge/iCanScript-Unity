using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class VariableReferenceDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject myVariable= null;
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds variable reference code definition.
        ///
        /// @param variable The port referensenting the variable.
        /// @return The newly created variable reference.
        ///
        public VariableReferenceDefinition(iCS_EditorObject variable, CodeBase parent)
        : base(null, parent) {
            myVariable= variable;
        }        

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            return GetNameFor(myVariable);
        }
    }

}
