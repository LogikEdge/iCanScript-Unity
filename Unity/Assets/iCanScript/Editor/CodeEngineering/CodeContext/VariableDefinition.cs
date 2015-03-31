using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class VariableDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject    myFieldObject= null;
        AccessType          myAccessType = AccessType.PRIVATE;
        ScopeType           myScopeType  = ScopeType.NONSTATIC;
        
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
        public VariableDefinition(iCS_EditorObject fieldObject, AccessType accessType, ScopeType scopeType)
        : base(CodeType.FIELD) {
            myFieldObject= fieldObject;
            myAccessType = accessType;
            myScopeType  = scopeType;
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the code for all fields of the class.
        ///
        /// @param indentSize The indentation of the fields.
        /// @param field The field for which to generate code.
        /// @return The generated code for the given field.
        ///
        public string GenerateCode(int indentSize) {
            var result= new StringBuilder(128);
    		// Generate non-static variables.
            if(myFieldObject.IsConstructor) {
                var nbOfParams= GetNbOfParameters(myFieldObject);
                var initValues= new string[nbOfParams];
                myFieldObject.ForEachChildPort(
                    p=> {
                        if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                            var v= p.InitialValue;
                            initValues[p.PortIndex]= ToValueString(v);
                        }
                    }
                );
                var fieldType= myFieldObject.RuntimeType;
                var initializer= GenerateAllocatorFragment(fieldType, initValues);
                string variableName;
                if(myAccessType == AccessType.PUBLIC) {
                    variableName= ToPublicFieldName(myFieldObject);
                }
                else {
                    variableName= ToPrivateFieldName(myFieldObject);
                }
    			result.Append(GenerateVariable(indentSize, myAccessType, myScopeType, fieldType, variableName, initializer));                    
            }
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for a class field.
        ///
		public static string GenerateVariable(int indentSize, AccessType accessType, ScopeType scopeType,
										      Type variableType, string variableName, string initializer) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_InOutPort]\n");
                result.Append(indent);
            }
            result.Append(ToAccessString(accessType));
            result.Append(" ");
            result.Append(ToScopeString(scopeType));
            result.Append(" ");
			result.Append(ToTypeName(variableType));
			result.Append(" ");
			result.Append(variableName);
			if(!String.IsNullOrEmpty(initializer)) {
				result.Append("= ");
				result.Append(initializer);
			}
			result.Append(";\n");
			return result.ToString();
		}

    }

}
