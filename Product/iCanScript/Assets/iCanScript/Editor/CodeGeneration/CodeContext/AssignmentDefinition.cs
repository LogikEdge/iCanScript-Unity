using UnityEngine;
using System.Text;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

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
        /// @param codeBlock The code block this assignment belongs to.
        /// @param leftHandCode The target of the assignment.
        /// @param rightHandCode The source of the assignment.
        /// @return The newly created code definition.
        ///
        public AssignmentDefinition(CodeBase parent,
                                    CodeBase leftHandSize, CodeBase rightHandCode)
        : base(null, parent) {
            myLeftHandCode = leftHandSize;
            myRightHandCode= rightHandCode;
            myLeftHandCode.Parent= myRightHandCode.Parent= parent;
        }        
    
        // -------------------------------------------------------------------
        /// Set the new code block for the assignment code
        ///
        /// @param newParent The new code block to be assigned.
        ///
        public override void OnParentChange(CodeBase newParent) {
            myLeftHandCode.Parent = newParent;
            myRightHandCode.Parent= newParent;
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