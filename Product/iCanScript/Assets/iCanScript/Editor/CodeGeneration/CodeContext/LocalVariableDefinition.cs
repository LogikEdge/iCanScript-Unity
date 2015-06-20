using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class LocalVariableDefinition : CodeBase {

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a statement block local variable.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created type.
        ///
        public LocalVariableDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {
                // Generate a local name.
                // FIXME: The local name is not working.
                parent.GetLocalVariableName(VSObject);
            }

    }

}