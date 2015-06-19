using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class ValueDefinition : CodeBase {
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a visual script value.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created value.
        ///
        public ValueDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {}
    
    }
}