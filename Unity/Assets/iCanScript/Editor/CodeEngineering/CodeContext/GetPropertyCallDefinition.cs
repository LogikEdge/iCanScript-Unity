using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

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
        public GetPropertyCallDefinition(iCS_EditorObject vsObj, CodeBase codeBlock)
        : base(vsObj, codeBlock) {
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
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(VSObject);
            result.Append(FunctionCallPrefix(memberInfo, VSObject));
            result.Append(ToFieldOrPropertyName(memberInfo));
            result.Append(GenerateReturnTypeCastFragment(VSObject));
            return result.ToString();
        }

    }

}
