#define OPTIMIZATION
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionCallDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeBase[]               myParameters     = null;
        List<VariableDefinition> myOutputVariables= new List<VariableDefinition>();
        CodeBase                 myReturnVariable = null;
		
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a function call code context.
        ///
        /// @param vsObj VS node associated with the function call.
        /// @return The newly created function call definition.
        ///
        public FunctionCallDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {
            BuildParameterInformation();
            BuildOutputParameters();
        }
        
        // -------------------------------------------------------------------
        /// Build information for parameters.
        void BuildParameterInformation() {
            var parameters= GetParameters(VSObject);
            var pLen= parameters.Length;
            myParameters= new CodeBase[pLen];
            foreach(var p in parameters) {
                int idx= p.PortIndex;
                if(p.IsInputPort) {
                    var producerPort= p.FirstProducerPort;
                    if(producerPort != null && producerPort != p) {
                        myParameters[idx]= new FunctionCallParameterDefinition(producerPort, this, p.RuntimeType);
                    }
                    else {
                        // Generate class variable for UnityEngine.Objects
                        var producerPortType= producerPort.RuntimeType;
                        if(iCS_Types.IsA<UnityEngine.Object>(producerPortType)) {
                            myParameters[idx]= new FunctionCallParameterDefinition(producerPort, this, p.RuntimeType);
                            var typeDef= GetTypeDefinition();
                            var v= new VariableDefinition(producerPort, typeDef, AccessType.PUBLIC, ScopeType.NONSTATIC);
                            typeDef.AddVariable(v);
                        }
                        else {
                            myParameters[idx]= new ValueDefinition(p, this);                            
                        }
                    }
                }
                else {
                    myParameters[idx]= new FunctionCallOutParameterDefinition(p, this);
                }
            }            
        }
        // -------------------------------------------------------------------
        /// Build output parameters.
        void BuildOutputParameters() {
            var outputPorts= GetOutputDataPorts();
            foreach(var p in outputPorts) {
                AddVariable(new VariableDefinition(p, Parent, AccessType.PRIVATE, ScopeType.NONSTATIC));
            }
            // Return value.
            // TODO: Build proper definition for return variable.
            var returnPort= GetReturnPort(VSObject);
            if(returnPort != null) {
                myReturnVariable= new ReturnVariableDefinition(returnPort, Parent);
            }
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
            // Optimize input parameters to fields/properties
            for(int i= 0; i < myParameters.Length; ++i) {
                var code= myParameters[i];
                if(code is FunctionCallOutParameterDefinition || code is ValueDefinition) {
                    continue;
                }
                iCS_EditorObject producerParent;
                if(CanReplaceParameterDefinition(code, out producerParent)) {
                    var producerCode= FindCodeBase(producerParent);
                    if(producerCode != null) {
                        producerCode.Parent.Remove(producerCode);
                        myParameters[i]= producerCode;
                        producerCode.Parent= this;
                    }                        
                }
            }
            // Ask output objects to resolve their own child dependencies.
			foreach(var v in myOutputVariables) {
				v.ResolveDependencies();
			}
			if(myReturnVariable != null) {
                myReturnVariable.ResolveDependencies();

                // TEST RETURN RELOCATION
                var returnParent= GetProperParentCodeForProducerPort(myReturnVariable);
                if(returnParent != null && returnParent != this && returnParent != Parent) {
                    var returnPort= myReturnVariable.VSObject;
                    if(returnParent is TypeDefinition) {
                        var v= new VariableDefinition(returnPort, returnParent, AccessType.PRIVATE, ScopeType.NONSTATIC);
                        returnParent.AddVariable(v);
                        myReturnVariable= null;
                    }
                    Debug.LogWarning(returnPort.DisplayName+" needs to be relocated");
                    Debug.LogWarning("New parent should be=> "+returnParent.VSObject.DisplayName);
                }
            }
            
		}

        // -------------------------------------------------------------------
        bool CanReplaceParameterDefinition(CodeBase code, out iCS_EditorObject producerParent) {
            var producerPort= code.VSObject;
            producerParent= producerPort.ParentNode;
            var producerInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(producerParent);
            if(producerInfo == null) return false;
            // Accept get field/property if we are the only consumer.
			if(IsFieldOrPropertyGet(producerInfo)) {
                if(producerPort.ConsumerPorts.Length == 1) {
                    return true;
                }
			}
#if OPTIMIZATION
            // Accept return value if we are the only consumer.
            if(producerPort.PortIndex == (int)iCS_PortIndex.Return) {
                var parameters= GetParameters(producerParent);
                if(P.filter(p=> p.IsOutDataPort, parameters).Length == 0) {
                    if(producerPort.ConsumerPorts.Length == 1) {
                        return true;
                    }                    
                }
            }
#endif
            return false;
        }
        
        // -------------------------------------------------------------------
        /// Adds a field definition to the class.
        ///
        /// @param vsObj VS object that represents the field.
        ///
        public override void AddVariable(VariableDefinition outputVariable) {
            if(outputVariable.VSObject.IsReturnPort) {
                myReturnVariable= outputVariable;
            }
            else {
                myOutputVariables.Add(outputVariable);                
            }
            outputVariable.Parent= this;
        }

        // -------------------------------------------------------------------
        public override void AddExecutable(CodeBase executableDefinition)    { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a function call definition."); }
        public override void AddType(TypeDefinition typeDefinition)             { Debug.LogWarning("iCanScript: Trying to add a type definition to a function call definition."); }
        public override void AddFunction(FunctionDefinition functionDefinition) { Debug.LogWarning("iCanScript: Trying to add a function definition to a function call definition."); }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the function call header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the function call.
        ///
        public override string GenerateHeader(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(128);
            // Generate function call (except for setter).        
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(VSObject);
            if(!IsFieldOrPropertySet(memberInfo)) {
                // Declare the output parameters.
                result.Append(DeclarelocalVariablesForOutputParameters(indentSize));
                // Declare return variable.
                result.Append(indent);
                result.Append(DeclareReturnVariable(VSObject));
            }
            else {
                result.Append(indent);
            }
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the function call code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted body code for the function call.
        ///
        public override string GenerateBody(int indentSize) {
            // Ajust back the indentation.
            var result= new StringBuilder(128);
            // Simplified situation for property get.
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(VSObject);
            var functionName= memberInfo.ToFunctionPrototypeInfo.MethodName;
            if(IsFieldOrPropertyGet(memberInfo)) {
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, VSObject));
                // Generate function call.
                result.Append(ToFieldOrPropertyName(memberInfo));
                result.Append(GenerateReturnTypeCastFragment(VSObject));
                return result.ToString();
            }
            // Determine parameters information.
            var parameters= GetParameters(VSObject);
            var pLen= parameters.Length;
            var paramStrings= new string[pLen];
            foreach(var p in parameters) {
                int idx= p.PortIndex;
                paramStrings[idx]= myParameters[idx].GenerateBody(indentSize);
            }
            // Special case for property set.
            if(IsFieldOrPropertySet(memberInfo)) {
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, VSObject));
                result.Append(ToFieldOrPropertyName(memberInfo));
                result.Append("= ");
                result.Append(paramStrings[0]);
            }
            // Generate function call.        
            else {
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, VSObject));
                // Declare function call.
                result.Append(GenerateFunctionCall(indentSize, functionName, paramStrings));
                result.Append(GenerateReturnTypeCastFragment(VSObject));            
            }
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the function call trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the function call.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ";\n";
        }

        // ===================================================================
        // CODE GENERATION UTILITIES
        // -------------------------------------------------------------------
        public static string GenerateFunctionCall(int indentSize, string functionName, string[] paramValues) {
            StringBuilder result= new StringBuilder();
            result.Append(functionName);
            result.Append("(");
            var len= paramValues.Length;
            for(int i= 0; i < len; ++i) {
                result.Append(paramValues[i]);
                if(i+1 < len) {
                    result.Append(", ");
                }
            }
            result.Append(")");
            return result.ToString();
        }
    	// -------------------------------------------------------------------------
        /// Generate return type cast.
        ///
        /// @param node The function call VS node.
        /// @return The return cast code string.
        ///
        string GenerateReturnTypeCastFragment(iCS_EditorObject node) {
            var returnPort= GetReturnPort(node);
            if(returnPort == null) return "";
            var consumerType= GetCommonBaseTypeForProducerPort(returnPort);
            if(consumerType == typeof(void) || consumerType == returnPort.RuntimeType) return "";
            return " as "+ToTypeName(consumerType);
        }
    	// -------------------------------------------------------------------------
        /// Generates the function call prefix code fragment.
        ///
        /// @param memberInfo The member information of the function to call.
        /// @param node Visual script function call node.
        /// @return The code fragment to prepend to the function call.
        ///
        string FunctionCallPrefix(iCS_MemberInfo memberInfo, iCS_EditorObject node) {
            var result= new StringBuilder(32);
            if(memberInfo != null && memberInfo.IsClassFunctionBase) {
                result.Append(ToTypeName(node.RuntimeType));
                result.Append(".");
            }
            else {
                var thisPort= GetThisPort(node);
                if(thisPort != null) {
                    var producerPort= GetCodeProducerPort(thisPort);
                    if(producerPort != null && producerPort != thisPort) {
                        var portRuntime= ToTypeName(thisPort.RuntimeType);
                        var producerCommonType= GetCommonBaseTypeForProducerPort(producerPort);
                        var producerRuntime= ToTypeName(producerCommonType);
                        if(portRuntime != producerRuntime) {
                            result.Append("(");
                        }
                        var producerNode= producerPort.ParentNode;
                        if(producerNode.IsConstructor) {
                            result.Append(GetNameFor(producerNode));                                                
                        }
                        else {
                            result.Append(GetNameFor(producerPort));                        
                        }
                        if(portRuntime != producerRuntime) {
                            result.Append(" as ");
                            result.Append(portRuntime);
                            result.Append(")");
                        }
                        result.Append(".");
                    }
                }
            }
            return result.ToString();
        }

        // =========================================================================
        // Code snippet decalartion
    	// -------------------------------------------------------------------------
        /// Declares the return value formated as "localVariable= ".
        ///
        /// @param node The node for which the return value will be declared.
        /// @return The return value decalartion.
        ///
        string DeclareReturnVariable(iCS_EditorObject node) {
            // No return variable necessary
            var returnPort= GetReturnPort(node);
            if(returnPort == null) return "";
            var consumerPorts= returnPort.EndConsumerPorts;
            if(consumerPorts.Length == 0) {
                return "";
            }
            // Don't need to generate return variable if no real consumer
            var hasConsumer= false;
            foreach(var c in consumerPorts) {
                if(c.IsEnablePort || c.ParentNode.IsKindOfFunction) {
                    hasConsumer= true;
                }
            }
            if(hasConsumer == false) return "";
            // Build return variable for the given node.
            var result= new StringBuilder(32);
            if(myReturnVariable != null) {
                result.Append(myReturnVariable.GenerateBody(0));
            }
            else {
                result.Append(GetNameFor(returnPort));
            }
//            result.Append("var ");
//            result.Append(Parent.GetLocalVariableName(returnPort));
            result.Append("= ");
            return result.ToString();
        }
    	// -------------------------------------------------------------------------
        /// Declares all output variable that will be used as output variable for a
        /// function call.
        ///
        /// @param indentSize The size of the indent at the beginning of a variable
        ///                   declaration.
        /// @param outParams List of ports that are output variable for function call.
        /// @return The formatted output variables declaration string.
        ///
        string DeclarelocalVariablesForOutputParameters(int indentSize) {
            var result= new StringBuilder(128);
            foreach(var v in myOutputVariables) {
                result.Append(v.GenerateCode(indentSize));
            }
            return result.ToString();
        }
    
        // =========================================================================
        // Utilities
    	// -------------------------------------------------------------------------
        /// Returns _'true'_ if the node is a field or property get function.
        static bool IsFieldOrPropertyGet(iCS_MemberInfo memberInfo) {
            var propertyInfo= memberInfo.ToPropertyInfo;
            if(propertyInfo != null) {
                return propertyInfo.IsGet;
            }
            var fieldInfo= memberInfo.ToFieldInfo;
            if(fieldInfo != null) {
                return fieldInfo.IsGet;
            }
            return false;
        }
    	// -------------------------------------------------------------------------
        /// Returns _'true'_ if the node is a field or property set function.
        static bool IsFieldOrPropertySet(iCS_MemberInfo memberInfo) {
            var propertyInfo= memberInfo.ToPropertyInfo;
            if(propertyInfo != null) {
                return propertyInfo.IsSet;
            }
            var fieldInfo= memberInfo.ToFieldInfo;
            if(fieldInfo != null) {
                return fieldInfo.IsSet;
            }
            return false;
        }

    	// -------------------------------------------------------------------------
        /// Returns the name of the field or property.
        ///
        /// @param memberInfo Member information associated with field or property.
        /// @return The name of the filed or property.
        ///
        string ToFieldOrPropertyName(iCS_MemberInfo memberInfo) {
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
        
    	// -------------------------------------------------------------------------
        /// Returns the list of output ports.
        iCS_EditorObject[] GetOutputDataPorts() {
            var parameters= GetParameters(VSObject);
            return P.filter(p=> p.IsOutDataPort, parameters);
        }

    }

}