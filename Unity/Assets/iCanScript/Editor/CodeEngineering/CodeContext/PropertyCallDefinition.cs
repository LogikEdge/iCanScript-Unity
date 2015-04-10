using UnityEngine;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

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
        protected string ToFieldOrPropertyName(iCS_MemberInfo memberInfo) {
            var propertyInfo= memberInfo.ToPropertyInfo;
            if(propertyInfo != null) {
                return propertyInfo.MethodName.Substring(4);
            }
            var fieldInfo= memberInfo.ToFieldInfo;
            if(fieldInfo != null) {
                return fieldInfo.MethodName;
            }
            return null;            
        }
        
    }

}