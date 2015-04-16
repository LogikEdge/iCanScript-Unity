using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Engine;
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
        public FunctionCallDefinition(iCS_EditorObject vsObj, CodeBase codeBlock)
        : base(vsObj, codeBlock) {
            BuildParameterInformation();
            BuildOutputParameters();
        }
        
        // -------------------------------------------------------------------
        /// Set the new code block for the assignment code
        ///
        /// @param newCodeBlock The new code block to be assigned.
        ///
        public override void SetCodeBlock(CodeBase newCodeBlock) {
            myCodeBlock= newCodeBlock;
            foreach(var p in myParameters)      { p.CodeBlock= newCodeBlock; }
            foreach(var v in myOutputVariables) { v.CodeBlock= newCodeBlock; }
            if(myReturnVariable != null) {
                myReturnVariable.CodeBlock= newCodeBlock;
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
                            var v= new VariableDefinition(producerPort, typeDef, AccessSpecifier.PUBLIC, ScopeSpecifier.NONSTATIC);
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
                AddVariable(new VariableDefinition(p, myCodeBlock, AccessSpecifier.PRIVATE, ScopeSpecifier.NONSTATIC));
            }
            // Return value.
            // TODO: Build proper definition for return variable.
            var returnPort= GetReturnPort(VSObject);
            if(returnPort != null) {
                myReturnVariable= new ReturnVariableDefinition(returnPort, myCodeBlock);
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
                var producerCode= OptimizeInputParameter(code, myCodeBlock);
                if(producerCode != null) {
                    myParameters[i]= producerCode;
                    producerCode.CodeBlock= myCodeBlock;
                }
				myParameters[i].ResolveDependencies();
            }
            // Ask output objects to resolve their own child dependencies.
			foreach(var v in myOutputVariables) {
				v.ResolveDependencies();
			}
			if(myReturnVariable != null) {
                myReturnVariable.ResolveDependencies();

                // Return variable relocation
                var returnCodeBlock= GetProperCodeBlockForProducerPort(myReturnVariable);
                if(returnCodeBlock != null && returnCodeBlock != myCodeBlock) {
                    var returnPort= myReturnVariable.VSObject;
                    if(returnCodeBlock is TypeDefinition) {
                        var v= new VariableDefinition(returnPort, returnCodeBlock, AccessSpecifier.PRIVATE, ScopeSpecifier.NONSTATIC);
                        returnCodeBlock.AddVariable(v);
                        myReturnVariable= null;
						v.ResolveDependencies();
                    }
                }
            }
            
		}

        // -------------------------------------------------------------------
        /// Returns a list of all enable ports that affects this function call.
        public override iCS_EditorObject[] GetRelatedEnablePorts() {
            var enablePorts= new List<iCS_EditorObject>();
            var funcNode= VSObject;
            while(funcNode != null) {
                enablePorts.AddRange(GetEnablePorts(funcNode));
                funcNode= funcNode.ParentNode;
            }
            enablePorts.Reverse();
            return enablePorts.ToArray();
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
            outputVariable.CodeBlock= myCodeBlock;
        }

        // -------------------------------------------------------------------
        public override void AddExecutable(CodeBase executableDefinition)       { Debug.LogWarning("iCanScript: Trying to add a child executable definition to a function call definition."); }
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
            var memberInfo= GetMemberInfo();
            if(memberInfo == null) {
                return "";
            }
            bool isSpecial= false;
            bool isOperator= false;
            var functionName= FunctionName(memberInfo, out isSpecial, out isOperator);                
            // Determine parameters information.
            var parameters= GetParameters(VSObject);
            var pLen= parameters.Length;
            var paramStrings= new string[pLen];
            foreach(var p in parameters) {
                int idx= p.PortIndex;
                paramStrings[idx]= myParameters[idx].GenerateBody(indentSize);
            }
            // Generate function call.
            if(isSpecial == false) {
                result.Append(FunctionCallPrefix(memberInfo, VSObject));
            }
            // Declare function call.
            if(isOperator) {
                result.Append(GenerateOperator(indentSize, memberInfo, functionName, paramStrings));
            }
            else {
                result.Append(GenerateFunctionCall(indentSize, memberInfo, functionName, paramStrings));                    
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
        /// @param memberInfo The member information from which to extract the
        ///                   method name.
        /// @param isSpecialName Output _true_ if name is special method.
        /// @return The method name.
        ///
        string FunctionName(iCS_MemberInfo memberInfo, out bool isSpecial, out bool isOperator) {
            // Validate that the function still exists.
            if(memberInfo == null) {
                isSpecial= isOperator= false;
                return "";
            }
            if(memberInfo.IsConstructor) {
                isSpecial= true;
                isOperator= false;
                return ToTypeName(memberInfo.ClassType);
            }
            var functionName= memberInfo.ToFunctionPrototypeInfo.MethodName;
            isSpecial= isOperator= functionName.StartsWith("op_");
            return functionName;
        }
        
        // -------------------------------------------------------------------
        /// Returns the MemberInfo for the current visual script object.
        iCS_MemberInfo GetMemberInfo() {
            return iCS_LibraryDatabase.GetAssociatedDescriptor(VSObject);
        }
        
        // -------------------------------------------------------------------
        public string GenerateFunctionCall(int indentSize, iCS_MemberInfo memberInfo, string functionName, string[] paramValues) {
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
        public string GenerateOperator(int indentSize, iCS_MemberInfo memberInfo, string functionName, string[] paramValues) {
            StringBuilder result= new StringBuilder();
            var symbol= OperatorNameToSymbol(functionName);
            var len= paramValues.Length;
            switch(len) {
                case 1: {
                    result.Append(symbol);
                    result.Append(paramValues[0]);
                    break;
                }
                case 2: {
                    result.Append(paramValues[0]);
                    result.Append(" ");
                    result.Append(symbol);
                    result.Append(" ");
                    result.Append(paramValues[1]);
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
            if(operatorName == "op_Equality")     return "==";
            if(operatorName == "op_Inequality")   return "!=";
            if(operatorName == "op_Addition")     return "+";
            if(operatorName == "op_Subtraction")  return "-";
            if(operatorName == "op_Multiply")     return "*";
            if(operatorName == "op_Division")     return "/";
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
            var consumerType= GetCommonBaseTypeForProducerPort(returnPort);
            if(consumerType == typeof(void) || iCS_Types.IsA(consumerType, returnPort.RuntimeType)) {
                return "";
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
        protected string FunctionCallPrefix(iCS_MemberInfo memberInfo, iCS_EditorObject node) {
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
						var desiredType= VSObject.RuntimeType;
		                var desiredTypeName= ToTypeName(desiredType);
		                var producerType= Context.GetRuntimeTypeFor(producerPort);
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
					else if(thisPort.InitialValue is OwnerTag) {
						var thisType= iCS_Types.RemoveRefOrPointer(thisPort.RuntimeType);
						if(thisType == typeof(Transform)) {
							result.Append("transform.");
						}
						if(thisType == typeof(GameObject)) {
							result.Append("gameObject.");
						}
						// Here we assume that no prefix is required.
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