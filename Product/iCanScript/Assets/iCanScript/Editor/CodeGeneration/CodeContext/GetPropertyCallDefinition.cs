using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class GetPropertyCallDefinition : PropertyCallDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the property/field _GET_ code definition.
        ///
        /// @param vsObj The visual script object associated with the _GET_
        ///              property or field accessor.
        /// @param codeBlock The code block in which the accessor reside.
        /// @return The newly created GET definition.
        ///
        public GetPropertyCallDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {
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
            var result= new StringBuilder(128);
            result.Append(FunctionCallPrefix(VSObject));
            result.Append(ToFieldOrPropertyName());
            result.Append(GenerateReturnTypeCastFragment(VSObject));
            return result.ToString();
        }

    }

}
