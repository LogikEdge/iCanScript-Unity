using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public class VariableDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        public iCS_EditorObject    myVSObject= null;
        public AccessType          myAccessType = AccessType.PRIVATE;
        public ScopeType           myScopeType  = ScopeType.NONSTATIC;
        
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
        public VariableDefinition(iCS_EditorObject vsObject, AccessType accessType, ScopeType scopeType)
        : base(CodeType.VARIABLE) {
            myVSObject= vsObject;
            myAccessType = accessType;
            myScopeType  = scopeType;
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
        public override void AddVariable(VariableDefinition variableDefinition) { Debug.LogWarning("iCanScript: Trying to add a variable defintion to a variable definition."); }
        public override void AddExecutable(CodeContext executableDefinition)    { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a variable definition."); }
        public override void AddType(TypeDefinition typeDefinition)             { Debug.LogWarning("iCanScript: Trying to add a type definition to a variable definition."); }
        public override void AddFunction(FunctionDefinition functionDefinition) { Debug.LogWarning("iCanScript: Trying to add a function definition to a variable definition."); }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the code for all fields of the class.
        ///
        /// @param indentSize The indentation of the fields.
        /// @param field The field for which to generate code.
        /// @return The generated code for the given field.
        ///
        public override string GenerateCode(int indentSize) {
            var result= new StringBuilder(128);
            var fieldType= myVSObject.RuntimeType;
            string initializer= "";
            // Generate variable from port.
            if(myVSObject.IsDataPort) {
                initializer= GetNameFor(myVSObject, /*valueInsteadOfSelf=*/true);
            }
    		// Generate variable from constructor.
            else if(myVSObject.IsConstructor) {
                var nbOfParams= GetNbOfParameters(myVSObject);
                var initValues= new string[nbOfParams];
                myVSObject.ForEachChildPort(
                    p=> {
                        if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                            initValues[p.PortIndex]= GetNameFor(p.FirstProducerPort, /*valueInsteadOfSelf=*/true);
                        }
                    }
                );
                initializer= GenerateAllocatorFragment(fieldType, initValues);
            }
            else {
                Debug.LogWarning("iCanScript: Unreconized variable type");
                return "";
            }
            string variableName;
            if(Parent is TypeDefinition) {
                if(myAccessType == AccessType.PUBLIC) {
                    variableName= Parent.GetPublicFieldName(myVSObject);
                }
                else {
                    variableName= Parent.GetPrivateFieldName(myVSObject);
                }                
            }
            else {
                // FIXME: should be going to common parent.
                variableName= Parent.Parent.GetLocalVariableName(myVSObject);
                initializer= null;
            }
			result.Append(GenerateVariable(indentSize, myAccessType, myScopeType, fieldType, variableName, initializer));                    
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for a class field.
        ///
		public string GenerateVariable(int indentSize, AccessType accessType, ScopeType scopeType,
									   Type variableType, string variableName, string initializer) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            if(Parent is TypeDefinition) {
                if(accessType == AccessType.PUBLIC) {
                    result.Append("[iCS_InOutPort]\n");
                    result.Append(indent);
                }
                result.Append(ToAccessString(accessType));
                result.Append(" ");
                result.Append(ToScopeString(scopeType));
                result.Append(" ");                
            }
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
