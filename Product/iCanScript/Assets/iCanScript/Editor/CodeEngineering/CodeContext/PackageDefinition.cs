using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class PackageDefinition : ExecutionBlockDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds an package code block.
        ///
        /// @param codeBlock The code block this assignment belongs to.
        /// @param enables The enable ports that affect this code block.
        /// @return The newly created code context.
        ///
        public PackageDefinition(iCS_EditorObject node, CodeBase codeBlock, iCS_EditorObject[] enables)
        : base(node, codeBlock) {
        }

    }  

}