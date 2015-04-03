using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionCallDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
		iCS_EditorObject	myNode= null;
        List<VariableDefinition> myOutputVariables= new List<VariableDefinition>();
        VariableDefinition       myReturnVariable = null;
		
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
        public FunctionCallDefinition(iCS_EditorObject vsObj)
        : base(CodeType.FUNCTION_CALL) {
        	myNode= vsObj;
            // Gather output parameter information.
            var outputPorts= GetOutputDataPorts();
            foreach(var p in outputPorts) {
                AddVariable(new VariableDefinition(p, AccessType.PRIVATE, ScopeType.NONSTATIC));
            }
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			foreach(var v in myOutputVariables) {
				v.ResolveDependencies();
			}
			myReturnVariable.ResolveDependencies();
		}

        // -------------------------------------------------------------------
        /// Adds a field definition to the class.
        ///
        /// @param vsObj VS object that represents the field.
        ///
        public override void AddVariable(VariableDefinition outputVariable) {
            if(outputVariable.myVSObject.IsReturnPort) {
                myReturnVariable= outputVariable;
            }
            else {
                myOutputVariables.Add(outputVariable);                
            }
            outputVariable.Parent= this;
        }

        // -------------------------------------------------------------------
        public override void AddExecutable(CodeContext executableDefinition)    { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a function call definition."); }
        public override void AddType(TypeDefinition typeDefinition)             { Debug.LogWarning("iCanScript: Trying to add a type definition to a function call definition."); }
        public override void AddFunction(FunctionDefinition functionDefinition) { Debug.LogWarning("iCanScript: Trying to add a function definition to a function call definition."); }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        public override string GenerateCode(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(128);
            // Simplified situation for property get.
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(myNode);
            var functionName= memberInfo.ToFunctionPrototypeInfo.MethodName;
            if(IsFieldOrPropertyGet(memberInfo)) {
                // Declare return variable.
                result.Append(indent);
                result.Append(DeclareReturnVariable(myNode));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                // Generate function call.
                result.Append(ToFieldOrPropertyName(memberInfo));
                result.Append(GenerateReturnTypeCastFragment(myNode));
                result.Append(";\n");
                return result.ToString();
            }
            // Declare the output parameters.
            result.Append(DeclarelocalVariablesForOutputParameters(indentSize));
            // Determine parameters information.
            var parameters= GetParameters(myNode);
            var pLen= parameters.Length;
            var paramStrings= new string[pLen];
            foreach(var p in parameters) {
                if(p.IsInputPort) {
                    var producerPort= p.FirstProducerPort;
                    if(producerPort != null && producerPort != p) {
                        paramStrings[p.PortIndex]= GetNameFor(producerPort);
                        var producerCommonType= GetCommonBaseTypeForProducerPort(producerPort);
                        var portTypeName= ToTypeName(p.RuntimeType);
                        if(portTypeName != ToTypeName(producerCommonType)) {
                            paramStrings[p.PortIndex]+= " as "+portTypeName;
                        }
                    }
                    else {
                        var v= p.InitialValue;
                        paramStrings[p.PortIndex]= ToValueString(v);
                    }
                }
                else {
                    paramStrings[p.PortIndex]= "out "+GetNameFor(p);
                }
            }
            // Special case for property set.
            result.Append(indent);
            if(IsFieldOrPropertySet(memberInfo)) {
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                result.Append(ToFieldOrPropertyName(memberInfo));
                result.Append("= ");
                result.Append(paramStrings[0]);
            }
            // Generate function call.        
            else {
                // Declare return variable.
                result.Append(DeclareReturnVariable(myNode));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                // Declare function call.
                result.Append(GenerateFunctionCall(indentSize, functionName, paramStrings));
                result.Append(GenerateReturnTypeCastFragment(myNode));            
            }
            result.Append(";\n");
            return result.ToString();
        }
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
                    var producerPort= thisPort.FirstProducerPort;
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
            result.Append("var ");
            result.Append(Parent.GetLocalVariableName(returnPort));
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
        /// Returns the input port representing the _'self'_ connection.
        ///
        /// @param node The node in which to search for the _'self'_ port.
        ///
        /// @return _'null'_ is returned if the port is not found.
        ///
        static iCS_EditorObject GetThisPort(iCS_EditorObject node) {
            iCS_EditorObject result= null;
            node.ForEachChildPort(
                p=> {
                    if(p.PortIndex == (int)iCS_PortIndex.InInstance) {
                        result= p;
                    }
                }
            );
            return result;
        }
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
            var parameters= GetParameters(myNode);
            return P.filter(p=> p.IsOutDataPort, parameters);
        }

    	// -------------------------------------------------------------------------
        void DetermineVariableIssue(iCS_EditorObject producerPort) {
            var consumers= producerPort.EndConsumerPorts;
            foreach(var c in consumers) {
                if(c != producerPort) {
                    var parent= c.ParentNode;
                    if(parent.IsKindOfFunction) {
//                        var definition= GetRootContext().FindDefinitionFor(parent);
                    }
                }
            }
        }
    }

}