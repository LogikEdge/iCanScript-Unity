using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class SetPropertyCallDefinition : PropertyCallDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the property/field _SET_ code definition.
        ///
        /// @param vsObj VS node associated with the property/field _SET_.
        /// @return The newly created SET definition.
        ///
        public SetPropertyCallDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {
        }
        

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the function call.
        ///
        public override string GenerateHeader(int indentSize) {
            return ToIndent(indentSize);
        }
        
        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            // Set must have a single parameter.
            if(Parameters.Length != 1) return "";
            var result= new StringBuilder(128);
            // Determine function prefix.
            result.Append(FunctionCallPrefix(VSObject));
            result.Append(ToFieldOrPropertyName());
            result.Append("= ");
            result.Append(Parameters[0].GenerateBody(0));
            return result.ToString();
        }
    }

}
