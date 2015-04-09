using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class AssignmentDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeBase    myLeftHandCode = null;
        CodeBase    myRightHandCode= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the property/field _GET_ code definition.
        ///
        /// @param outParam The output parameter port.
        /// @param parent The parent code block.
        /// @param rightHandCode The code that generates the parameter value.
        /// @return The newly created GET definition.
        ///
        public AssignmentDefinition(iCS_EditorObject vsObj, CodeBase parent,
                                    CodeBase leftHandSize, CodeBase rightHandCode)
        : base(null, parent) {
            myLeftHandCode = leftHandSize;
            myRightHandCode= rightHandCode;
        }        
    
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the header portion of the output parameter assignment code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the header.
        ///
        public override string GenerateHeader(int indentSize) {
            return ToIndent(indentSize);
        }
        
        // -------------------------------------------------------------------
        /// Generate the output parameter assignment body code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            var result= new StringBuilder(128);
            result.Append(myLeftHandCode.GenerateBody(0));
            result.Append("= ");
            result.Append(myRightHandCode.GenerateBody(0));
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the trailer code for the output parameter assignment.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the output parameter.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ";\n";
        }

    }

}