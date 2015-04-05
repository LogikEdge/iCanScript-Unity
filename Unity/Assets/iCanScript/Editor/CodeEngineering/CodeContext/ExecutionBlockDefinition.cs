using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class ExecutionBlockDefinition : CodeBase {

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an execution code block.
        ///
        /// @param vsObject The visual script objects associated with this code.
        /// @param parent   The code block parent.
        /// @return The newly created code context.
        ///
        public ExecutionBlockDefinition(iCS_EditorObject vsObject, CodeBase parent)
        : base(vsObject, parent) {
        }

    }

}