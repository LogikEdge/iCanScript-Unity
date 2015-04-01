using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionCallDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
		iCS_EditorObject	myNode= null;
		
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
        }
        
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        public override string GenerateCode(int indentSize) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 128);
            // Simplified situation for property get.
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(myNode);
            var functionName= GetPublicFunctionName(myNode);
            if(IsPropertyGet(memberInfo)) {
                // Declare return variable.
                result.Append(DeclareReturnVariable(myNode));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                // Generate function call.
                result.Append(ToPropertyName(myNode));
                result.Append(";\n");
                return result.ToString();
            }
            // Determine parameters.
            var parameters= GetParameters(myNode);
            var pLen= parameters.Length;
            var paramStrings= new string[pLen];
            var outputParams= new List<iCS_EditorObject>();
            foreach(var p in parameters) {
                if(p.IsInputPort) {
                    var producerPort= p.FirstProducerPort;
                    if(producerPort != null && producerPort != p) {
                        paramStrings[p.PortIndex]= GetNameFor(producerPort);
                    }
                    else {
                        var v= p.InitialValue;
                        paramStrings[p.PortIndex]= ToValueString(v);
                    }
                }
                else {
                    outputParams.Add(p);
                    paramStrings[p.PortIndex]= "out "+GetLocalVariableName(p);
                }
            }
            // Special case for property set.
            if(IsPropertySet(memberInfo)) {
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                result.Append(ToPropertyName(myNode));
                result.Append("= ");
                result.Append(paramStrings[0]);
            }
            // Generate function call.        
            else {
                // Declare the output parameters.
                result.Append(DeclarelocalVariablesForOutputParameters(indentSize, outputParams));
                // Declare return variable.
                result.Append(DeclareReturnVariable(myNode));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, myNode));
                // Declare function call.
                result.Append(GenerateFunctionCall(indentSize, functionName, paramStrings));            
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
                        var producerNode= producerPort.ParentNode;
                        if(producerNode.IsConstructor) {
                            result.Append(GetNameFor(producerNode));                                                
                        }
                        else {
                            result.Append(GetNameFor(producerPort));                        
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
            result.Append(GetLocalVariableName(returnPort));
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
        string DeclarelocalVariablesForOutputParameters(int indentSize, List<iCS_EditorObject> outputParams) {
            if(outputParams.Count == 0) return "";
            var result= new StringBuilder(128);
            foreach(var p in outputParams) {
                result.Append(ToTypeName(p.RuntimeType));
                result.Append(" ");
                result.Append(GetLocalVariableName(p));
                result.Append(";\n");
                result.Append(ToIndent(indentSize));
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
        /// Returns _'true'_ if the node is a property get function.
        static bool IsPropertyGet(iCS_MemberInfo memberInfo) {
            var propertyInfo= memberInfo.ToPropertyInfo;
            if(propertyInfo == null) return false;
            return propertyInfo.IsGet;
        }
    	// -------------------------------------------------------------------------
        /// Returns _'false'_ if the node is a property get function.
        static bool IsPropertySet(iCS_MemberInfo memberInfo) {
            var propertyInfo= memberInfo.ToPropertyInfo;
            if(propertyInfo == null) return false;
            return propertyInfo.IsSet;
        }

    }

}