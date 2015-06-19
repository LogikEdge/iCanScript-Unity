using UnityEngine;
using System.Collections;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class PropertyCallDefinition : FunctionCallDefinition {
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Build a property/field call definition.
        ///
        /// @param vsObj VS node associated with the property or field.
        /// @return The newly created GET definition.
        ///
        public PropertyCallDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {
        }        

        // ===================================================================
        // PROPERTY / FIELD UTILITIES
    	// -------------------------------------------------------------------------
        /// Returns the name of the field or property.
        ///
        /// @param memberInfo Member information associated with field or property.
        /// @return The name of the filed or property.
        ///
        protected string ToFieldOrPropertyName() {
            var codeName= VSObject.CodeName;
            if(codeName.StartsWith("set_") || codeName.StartsWith("get_")) {
                return codeName.Substring(4);
            }
            return codeName;
        }
        
    }

}