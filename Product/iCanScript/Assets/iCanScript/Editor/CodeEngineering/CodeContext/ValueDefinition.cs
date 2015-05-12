using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeEngineering {

    public class ValueDefinition : CodeBase {
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a visual script value.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created reference.
        ///
        public ValueDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {}
    
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the if-statement code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the if-statement.
        ///
        public override string GenerateBody(int indentSize) {
            return ToValueString(VSObject.InitialValue);
        }
    
    }
}