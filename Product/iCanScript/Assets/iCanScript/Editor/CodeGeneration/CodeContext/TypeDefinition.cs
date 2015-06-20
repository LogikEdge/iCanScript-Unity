using UnityEngine;
using System;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class TypeDefinition : CodeBase {
    
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a reference to a visual script type.
        ///
        /// @param vsObject VS objects being referenced.
        /// @param parent The parent code context.
        /// @return The newly created type.
        ///
        public TypeDefinition(iCS_EditorObject vsObject, CodeBase parent)
            : base(vsObject, parent) {}
    
    }
}