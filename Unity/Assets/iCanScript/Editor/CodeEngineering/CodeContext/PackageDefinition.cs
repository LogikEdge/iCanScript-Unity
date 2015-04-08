using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class PackageDefinition : CodeBase {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a function call code context.
        ///
        /// @param package VS node associated with the function call.
        /// @return The newly created function call definition.
        ///
        public PackageDefinition(iCS_EditorObject package, CodeBase parent)
        : base(package, parent) {
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