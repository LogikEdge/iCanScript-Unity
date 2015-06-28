using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;
using P=iCanScript.Internal.Prelude;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class FunctionCallDefinition : CodeBase {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        CodeBase[]               myParameters     = null;
        List<VariableDefinition> myOutputVariables= new List<VariableDefinition>();
        CodeBase                 myReturnVariable = null;
        CodeBase                 myTargetCode     = null;
		
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        protected CodeBase[] Parameters {
            get { return myParameters; }
        }
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a function call code context.
        ///
        /// @param vsObj VS node associated with the function call.
        /// @param codeBlock The code block this assignment belongs to.
        /// @return The newly created function call definition.
        ///
        public FunctionCallDefinition(iCS_EditorObject vsObj, CodeBase parent)
        : base(vsObj, parent) {
            BuildParameterInformation();
            BuildOutputParameters();
        }
        
        // -------------------------------------------------------------------
        /// Set the new code block for the assignment code
        ///
        /// @param newParent The new code block to be assigned.
        ///
        public override void OnParentChange(CodeBase newParent) {
            foreach(var p in myParameters)      { p.Parent= newParent; }
            foreach(var v in myOutputVariables) { v.Parent= newParent; }
            if(myReturnVariable != null) {
                myReturnVariable.Parent= newParent;
            }
        }

        // -------------------------------------------------------------------
        /// Build information for parameters.
        void BuildParameterInformation() {
            var parameters= GetParameters(VSObject);
            var pLen= parameters.Length;
            myParameters= new CodeBase[pLen];
            foreach(var p in parameters) {
                int idx= p.PortIndex;
                if(idx >= parameters.Length) {
                    VSObject.ForEachChildPort(
                        dp=> {
                            Debug.Log("Index: "+dp.PortIndex+" => "+dp.FullName);                            
                        }
                    );
                }
                if(p.IsInputPort) {
                    var producerPort= GraphInfo.GetProducerPort(p);
                    if(producerPort != null && producerPort != p) {
                        myParameters[idx]= new FunctionCallParameterDefinition(p, this, p.RuntimeType);
                    }
                    else {
                        // Generate class variable for UnityEngine.Objects
                        var portType= p.RuntimeType;
                        if(iCS_Types.IsA<UnityEngine.Object>(portType)) {
                            myParameters[idx]= new FunctionCallParameterDefinition(p, this, p.RuntimeType);
                            var typeDef= GetClassDefinition();
                            var v= new VariableDefinition(p, typeDef, AccessSpecifier.Public, ScopeSpecifier.NonStatic);
                            typeDef.AddVariable(v);
                        }
                        else {
                            myParameters[idx]= new ConstantDefinition(p, this);                            
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
                AddVariable(new VariableDefinition(p, myParent, AccessSpecifier.Private, ScopeSpecifier.NonStatic));
            }
            // Return value.
            // TODO: Build proper definition for return variable.
            var returnPort= GetReturnPort(VSObject);
            if(returnPort != null) {
                myReturnVariable= new ReturnVariableDefinition(returnPort, myParent);
            }
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves any dependencies that this code has.
		public override void ResolveDependencies() {
            // Optimize input parameters to fields/properties
            for(int i= 0; i < myParameters.Length; ++i) {
                var code= myParameters[i];
                var producerCode= OptimizeInputParameter(code, myParent);
                if(producerCode != null) {
                    myParameters[i]= producerCode;
                    producerCode.Parent= myParent;
                }
				myParameters[i].ResolveDependencies();
            }
            // -- Optimize target port from get fields/properties. --
            if(!IsStatic()) {
                var targetPort= GraphInfo.GetTargetPort(VSObject);
                if(targetPort != null) {
                    var producerPort= GraphInfo.GetProducerPort(targetPort);
					var producerCode= Context.GetCodeFor(producerPort.ParentNode);
                    if(producerCode is GetPropertyCallDefinition) {
                        if(producerPort.ConsumerPorts.Length == 1) {
                            myTargetCode= producerCode;
                            producerCode.Parent.Remove(producerCode);
                            producerCode.Parent= myParent;
                        }
                    }
                }
            }
            // Ask output objects to resolve their own child dependencies.
			foreach(var v in myOutputVariables) {
				v.ResolveDependencies();
			}
			if(myReturnVariable != null) {
                myReturnVariable.ResolveDependencies();

                // Return variable relocation
                var returnParent= GetProperParentForProducerPort(myReturnVariable);
                if(returnParent != null && returnParent != myParent) {
                    var returnPort= myReturnVariable.VSObject;
                    if(returnParent is ClassDefinition) {
                        var v= new VariableDefinition(returnPort, returnParent, AccessSpecifier.Private, ScopeSpecifier.NonStatic);
                        returnParent.AddVariable(v);
                        myReturnVariable= null;
						v.ResolveDependencies();
                    }
                }
            }
            
		}

        // -------------------------------------------------------------------
        /// Returns a list of all enable ports that affects this function call.
        public override iCS_EditorObject[] GetRelatedEnablePorts() {
            return ControlFlow.GetAllRelatedEnablePorts(VSObject);
        }

        // -------------------------------------------------------------------
        /// Returns the list of all visual script objects this function call
        /// depends on.
        public override iCS_EditorObject[] GetDependencies() {
            return GetNodeCodeDependencies(VSObject);
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
            outputVariable.Parent= myParent;
        }

        // -------------------------------------------------------------------
        public override void AddExecutable(CodeBase executableDefinition)       { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a function call definition."); }
        public override void AddType(ClassDefinition typeDefinition)            { Debug.LogWarning("iCanScript: Trying to add a type definition to a function call definition."); }
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
            // Declare the output parameters.
            result.Append(DeclarelocalVariablesForOutputParameters(indentSize));
            // Declare return variable.
            result.Append(indent);
            result.Append(DeclareReturnVariable(VSObject));
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
            bool isOperator= false;
            var functionName= FunctionName(out isOperator);                
            // Determine parameters information.
            var parameters= GetParameters(VSObject);
            var pLen= parameters.Length;
            var paramStrings= new string[pLen];
            foreach(var p in parameters) {
                int idx= p.PortIndex;
                paramStrings[idx]= myParameters[idx].GenerateBody(indentSize);
            }
            // Declare function call.
            if(isOperator) {
				// -- The first parameter is the instance if not static. --
                int paramOffset= 0;
				if(!IsStatic()) {
					var variable= FunctionCallPrefix(VSObject);
					var variableLen= variable.Length;
					if(variableLen != 0) {
                        paramOffset= -1;
						var len= paramStrings.Length;
						Array.Resize(ref paramStrings, len+1);
						for(int i= len-1; i >= 0; --i) {
							paramStrings[i+1]= paramStrings[i]; 
						}
			        	paramStrings[0]= variable.Substring(0, variableLen-1);
					}
				}
	            result.Append(GenerateOperator(indentSize, functionName, paramStrings, paramOffset));
            }
            // Special case for array subscript operator.
            else if(functionName == "get_Item" || functionName == "set_Item") {
                result.Append(paramStrings[0]);
                result.Append("[");
                result.Append(paramStrings[1]);
                result.Append("]");
                if(functionName == "set_Item") {
                    result.Append("= ");
                    result.Append(paramStrings[2]);
                }
            }
            else {
	        	result.Append(FunctionCallPrefix(VSObject));
				result.Append(GenerateFunctionCall(indentSize, functionName, paramStrings));                    
            }
            result.Append(GenerateReturnTypeCastFragment(VSObject));            
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
        /// Returns the method name from the member information.
        ///
        /// @param isUnaryOperator _true_ if this is a unary operator.
		/// @param isOperator _true_ if this is an operator.
        /// @return The method name.
        ///
        string FunctionName(out bool isOperator) {
            if(VSObject.IsConstructor) {
                isOperator= false;
                return ToTypeName(VSObject.RuntimeType);
            }
            var functionName= VSObject.CodeName;
            isOperator= functionName.StartsWith("op_");
            return functionName;
        }
        
        // -------------------------------------------------------------------
        /// Determines if the function call is static.
        bool IsStatic() {
            if(VSObject.IsStaticFunction || VSObject.IsStaticField) return true;
            return false;
        }
        
        // -------------------------------------------------------------------
        public string GenerateFunctionCall(int indentSize, string functionName, string[] paramValues) {
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
        public string GenerateOperator(int indentSize, string functionName, string[] paramValues, int paramOffset) {
            StringBuilder result= new StringBuilder();
            var symbol= OperatorNameToSymbol(functionName);
            var len= paramValues.Length;
            switch(len) {
                case 1: {
                    result.Append(symbol);
                    var isParamAnOperator= false;
                    if(paramOffset >= 0) {
                       isParamAnOperator= myParameters[paramOffset].IsOperatorFunctionCall();
                       if(isParamAnOperator) result.Append("(");                        
                    }
                    result.Append(paramValues[0]);
                    if(isParamAnOperator) result.Append(")");
                    break;
                }
                case 2: {
                    var isParamAnOperator= false;
                    if(paramOffset >= 0) {
                        isParamAnOperator= myParameters[paramOffset].IsOperatorFunctionCall();
                        if(isParamAnOperator) result.Append("(");
                    }
                    result.Append(paramValues[0]);
                    if(isParamAnOperator) result.Append(")");
                    result.Append(" ");
                    result.Append(symbol);
                    result.Append(" ");
                    isParamAnOperator= myParameters[1+paramOffset].IsOperatorFunctionCall();
                    if(isParamAnOperator) result.Append("(");
                    result.Append(paramValues[1]);
                    if(isParamAnOperator) result.Append(")");
                    break;
                }
                default: {
                    Debug.LogWarning("iCanScript: Unknown trinary operator=> "+symbol);
                    break;
                }
            }				
			return result.ToString();
        }

    	// -------------------------------------------------------------------------
        string OperatorNameToSymbol(string operatorName) {
            if(operatorName == "op_Addition")           return "+";
            if(operatorName == "op_Subtraction")        return "-";
            if(operatorName == "op_Multiply")           return "*";
            if(operatorName == "op_Division")           return "/";
            if(operatorName == "op_AdditionAssign")     return "+=";
            if(operatorName == "op_SubtractionAssign")  return "-=";
            if(operatorName == "op_MultiplyAssign")     return "*=";
            if(operatorName == "op_DivisionAssign")     return "/=";
            if(operatorName == "op_Equality")           return "==";
            if(operatorName == "op_Inequality")         return "!=";
			if(operatorName == "op_GreaterThan")        return ">";
			if(operatorName == "op_LessThan")           return "<";
			if(operatorName == "op_GreaterThanOrEqual")	return ">=";
			if(operatorName == "op_LessThanOrEqual")    return "<=";
            if(operatorName == "op_LogicalNot")         return "!";
            if(operatorName == "op_LogicalOr")          return "||";
            if(operatorName == "op_LogicalAnd")         return "&&";
            if(operatorName == "op_BitwiseOr")          return "|";
            if(operatorName == "op_BitwiseAnd")         return "&";
            if(operatorName == "op_ExclusiveOr")        return "^";
            if(operatorName == "op_BitwiseOrAssign")    return "|=";
            if(operatorName == "op_BitwiseAndAssign")   return "&=";
            if(operatorName == "op_ExclusiveOrAssign")  return "^=";
            if(operatorName == "op_Assignment")         return "=";
            if(operatorName == "op_Increment")          return "++";
            if(operatorName == "op_Decrement")          return "--";
            if(operatorName == "op_LeftShift")          return "<<";
            if(operatorName == "op_RightShift")         return ">>";
            if(operatorName == "op_LeftShiftAssign")    return "<<=";
            if(operatorName == "op_RightShiftAssign")   return ">>=";
            
            Debug.LogWarning("iCanScript: Unknown operator=> "+operatorName);
            return operatorName;
        }
        
    	// -------------------------------------------------------------------------
        /// Generate return type cast.
        ///
        /// @param node The function call VS node.
        /// @return The return cast code string.
        ///
        protected string GenerateReturnTypeCastFragment(iCS_EditorObject node) {
            var returnPort= GetReturnPort(node);
            if(returnPort == null) return "";
            var consumerType= GetMostSpecializedTypeForProducerPort(returnPort);
            if(consumerType == typeof(void) || iCS_Types.IsA(consumerType, returnPort.RuntimeType)) {
                return "";
            }
            // Change return variable type.
            var returnVariable= myReturnVariable as ReturnVariableDefinition;
            if(returnVariable != null) {
                returnVariable.SetRuntimeType(consumerType);
            } 
            return " as "+ToTypeName(consumerType);
        }
    	// -------------------------------------------------------------------------
        /// Generates the function call prefix code fragment.
        ///
        /// @param memberInfo The member information of the function to call.
        /// @param node Visual script function call node.
        /// @return The code fragment to prepend to the function call.
        ///
        protected string FunctionCallPrefix(iCS_EditorObject node) {
            var result= new StringBuilder(32);
            if(IsStatic()) {
                if(!GraphInfo.IsLocalType(VSObject)) {
                    result.Append(ToTypeName(node.RuntimeType));
                    result.Append(".");
                }
            }
            else {
                var thisPort= GraphInfo.GetTargetPort(node);
                if(thisPort != null) {
//					result.Append(GetExpressionFor(thisPort));
//					result.Append(".");
                    // -- Prepend target code producer if optimized. --
                    if(myTargetCode != null) {
                        result.Append(myTargetCode.GenerateBody(0));
                        result.Append(".");
                        return result.ToString();
                    }
                    var producerPort= GraphInfo.GetProducerPort(thisPort);
	                var producerType= Context.GetRuntimeTypeFor(producerPort);
					var producerCode= Context.GetCodeFor(producerPort);
					if(producerCode is FunctionDefinitionParameter) {
                        var producerPortName= GetNameFor(producerPort);
                        if(producerPortName == null || producerPortName == "null") {
                            Debug.LogWarning("producer port is null: "+producerPort.CodeName);
                        }
						result.Append(GetNameFor(producerPort));
						result.Append(".");
					}
					else if(producerCode is LocalVariableDefinition) {
                        var producerPortName= Parent.GetLocalVariableName(producerPort);
                        if(producerPortName == null || producerPortName == "null") {
                            Debug.LogWarning("producer port is null: "+producerPort.CodeName);
                        }
						result.Append(producerPortName);
						result.Append(".");
					}
					else if(producerPort.IsOwner) {
						if(producerType == typeof(Transform)) {
							result.Append("transform.");
						}
						else if(producerType == typeof(GameObject)) {
							result.Append("gameObject.");
						}
					}
                    else if(producerPort != thisPort) {
						var desiredType= VSObject.RuntimeType;
		                var desiredTypeName= ToTypeName(desiredType);
		                var isUpcastNeeded= producerType != desiredType && iCS_Types.IsA(producerType, desiredType);
                        if(isUpcastNeeded) {
                            result.Append("(");
                        }
                        var producerNode= producerPort.ParentNode;
                        if(producerNode.IsConstructor) {
                            result.Append(GetNameFor(producerNode));                                                
                        }
                        else {
                            result.Append(GetNameFor(producerPort));                        
                        }
                        if(isUpcastNeeded) {
                            result.Append(" as ");
                            result.Append(desiredTypeName);
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
            var consumerPorts= returnPort.SegmentEndConsumerPorts;
            if(consumerPorts.Length == 0) {
                return "";
            }
            // Don't need to generate return variable if no real consumer
            var hasConsumer= false;
            foreach(var c in consumerPorts) {
                if(c.IsEnablePort || c.ParentNode.IsKindOfFunction) {
                    hasConsumer= true;
                }
                else {
                    var consumerCode= Context.GetCodeFor(c);
                    if(consumerCode != null) {
                        hasConsumer= true;
                    }
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
    
    	// -------------------------------------------------------------------------
        /// Returns the list of output ports.
        iCS_EditorObject[] GetOutputDataPorts() {
            var parameters= GetParameters(VSObject);
            return P.filter(p=> p.IsOutDataPort, parameters);
        }

    }

}