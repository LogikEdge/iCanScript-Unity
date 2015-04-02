using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class ClassFieldDefinition : VariableDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a class field description..
        ///
        /// @param fieldObjects VS objects associated with this field.
        /// @param accessType THe Access property of the field.
        /// @param scopeType The Scope property of the field.
        /// @return The newly created field defintion.
        ///
        public ClassFieldDefinition(iCS_EditorObject fieldObject, AccessType accessType, ScopeType scopeType)
        : base(CodeType.FIELD, fieldObject, accessType, scopeType) {
        }
            
    }

}
