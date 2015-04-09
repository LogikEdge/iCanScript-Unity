using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class OutParameterAssignmentDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject myOutParameter= null;
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds the property/field _GET_ code definition.
        ///
        /// @param vsObj VS node associated with the property/field _GET_.
        /// @return The newly created GET definition.
        ///
        public OutParameterAssignmentDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(null, parent) {
            myOutParameter= vsObj;
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
            return "";
        }
    }

}