using UnityEngine;
using System;
using System.Text;
using System.Collections;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class VariableDefinition : ValueDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        // TODO: Remove access & scope specifiers.
        public AccessSpecifier  myAccessSpecifier = AccessSpecifier.Private;
        public ScopeSpecifier   myScopeSpecifier  = ScopeSpecifier.NonStatic;
		public Type				myRuntimeType     = null;
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        public bool IsPublic  { get { return PortVariable.IsPublic; }}
        public bool IsPrivate { get { return PortVariable.IsPrivate; }}
        public bool IsStatic  { get { return PortVariable.IsStatic; }}
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a class field description..
        ///
        /// @param port The visual script port representing the variable.
        /// @param codeBlock The code block that contains this code.
        /// @param accessType THe Access property of the field.
        /// @param scopeType The Scope property of the field.
        /// @return The newly created variable defintion.
        ///
        public VariableDefinition(iCS_EditorObject vsObject, CodeBase parent, AccessSpecifier accessType, ScopeSpecifier scopeType)
        : base(vsObject, parent) {
            myAccessSpecifier = accessType;
            myScopeSpecifier  = scopeType;
			myRuntimeType     = vsObject.RuntimeType;
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			// Determine proper variable type.
            var consumerType= GetMostSpecializedTypeForProducerPort(VSObject);
            if(consumerType != typeof(void) && !iCS_Types.IsA(consumerType, myRuntimeType)) {
				myRuntimeType= consumerType;
            }			
		}
        // -------------------------------------------------------------------
		/// Returns the runtime type of the variable.
		public override Type GetRuntimeType() {
			return myRuntimeType;
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
        public override string GenerateCode(int indentSize) {
            if(myRuntimeType == null) {
                var message= "Unable to determine type for variable: "+myVSObject.FullName;
                Context.AddError(message, myVSObject.InstanceId);
                return "";
			}
            var result= new StringBuilder(128);
            string initializer= "";
            // Generate variable from port.
            if(VSObject.IsDataPort) {
                if(VSObject.IsInDataPort && !iCS_Types.IsA<UnityEngine.Object>(myRuntimeType)) {
                    initializer= GetValueFor(VSObject);
                }
                else {
                    initializer= "default("+ToTypeName(myRuntimeType)+")";
                }
            }
    		// Generate variable from constructor.
            else if(VSObject.IsConstructor) {
                if(!iCS_Types.IsA<UnityEngine.Object>(myRuntimeType)) {
                    var nbOfParams= GetNbOfParameters(myVSObject);
                    var initValues= new string[nbOfParams];
                    myVSObject.ForEachChildPort(
                        p=> {
                            if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                                initValues[p.PortIndex]= GetValueFor(p);
                            }
                        }
                    );
                    initializer= GenerateAllocatorFragment(myRuntimeType, initValues);                    
                }
                else {
                    initializer= "default("+ToTypeName(myRuntimeType)+")";
                }
            }
            else {
                Debug.LogWarning("iCanScript: Unreconized variable type");
                return "";
            }
            string variableName;
            if(Parent is ClassDefinition) {
                if(IsPublic) {
                    if(IsStatic) {
                        variableName= Parent.GetPublicStaticFieldName(myVSObject);
                    }
                    else {
                        variableName= Parent.GetPublicFieldName(myVSObject);
                    }
                }
                else {
                    if(IsStatic) {
                        variableName= Parent.GetPrivateStaticFieldName(myVSObject);
                    }
                    else {
                        variableName= Parent.GetPrivateFieldName(myVSObject);
                    }
                }
            }
            else {
                // FIXME: should be going to common parent.
                variableName= Parent.Parent.GetLocalVariableName(myVSObject);
                initializer= null;
            }
			result.Append(GenerateVariable(indentSize, myAccessSpecifier, myScopeSpecifier, myRuntimeType, variableName, initializer));                    
            return result.ToString();
        }
        
        // -------------------------------------------------------------------
        /// Generate the code for a class field.
        ///
		public string GenerateVariable(int indentSize, AccessSpecifier accessType, ScopeSpecifier scopeType,
									   Type variableType, string variableName, string initializer) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            if(myParent is ClassDefinition) {
                var port= PortVariable;
                var portSpec= port.PortSpec;
                result.Append(ToAccessString(portSpec));
                result.Append(ToScopeString(portSpec));
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

        // -------------------------------------------------------------------
        /// Returns the port associated with the variable.
        iCS_EditorObject PortVariable {
            get {
                return VSObject.IsConstructor ? VSObject.ReturnPort : VSObject;
            }
        }
    }

}
