using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class InjectedCodeDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        string myCode= null;
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds textual code definition.
        ///
        /// @param vsObj The visual script object associated with this code.
        /// @param parent The parent code block.
        /// @param code The textual code to be injected.
        /// @return The newly created code definition.
        ///
        public InjectedCodeDefinition(iCS_EditorObject vsObj, CodeBase parent, string code)
        : base(null, parent) {
            myCode= code;
        }        

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the first indentation for the injected code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The indentation text.
        ///
        public override string GenerateHeader(int indentSize) {
            return ToIndent(indentSize);
        }
        
        // -------------------------------------------------------------------
        /// Generate the injected code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The injected code.
        ///
        public override string GenerateBody(int indentSize) {
            return myCode;
        }
    }
}
