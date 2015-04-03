using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

public class EventHandlerDefinition : FunctionDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param node VS objects associated with the function.
        /// @return The newly created code context.
        ///
        public EventHandlerDefinition(iCS_EditorObject node, CodeContext parent, AccessType accessType, ScopeType scopeType)
        : base(node, parent, accessType, scopeType) {
        }
    
        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the code for a function definition.
        ///
        /// @param indentSize The indentation of the function.
        /// @return The generated code for the given function.
        ///
        public override string GenerateCode(int indentSize) {
            var result= new StringBuilder(1024);
    		// Get return information.
    		string returnType= ToTypeName(typeof(void));
            // Build parameter information 
    		var parameters= new List<iCS_EditorObject>();
            var components = new List<iCS_EditorObject>();
    		myFunctionNode.ForEachChildPort(
    			p=> {
    				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        if(p.IsFixDataPort) {
                            parameters.Add(p);
                        }
                        else if(p.IsInProposedDataPort && iCS_Types.IsA<Component>(p.RuntimeType)) {
                            components.Add(p);
                        }
    				}
    				if(p.PortIndex == (int)iCS_PortIndex.Return) {
    					returnType= ToTypeName(p.RuntimeType);
    				}
    			}
    		);
    		// Build parameters
            var parametersLen= parameters.Count;
    		var paramTypes= new string[parametersLen];
    		var paramNames= new string[parametersLen];
    		parameters.ForEach(
    			p=> {
    				var i= p.PortIndex;
					paramNames[i]= GetFunctionParameterName(p);
					paramTypes[i]= ToTypeName(p.RuntimeType);
                    if(p.IsOutputPort) {
                        paramTypes[i]= "out "+paramTypes[i];
                    }
    			}
    		);
            // Build components
            var componentsLen= components.Count;
            var componentsCode= new string[componentsLen];
            for(int i= 0; i < componentsLen; ++i) {
                var p= components[i];
                var componentCode= new StringBuilder(128);
                componentCode.Append("var ");
                componentCode.Append(GetLocalVariableName(p));
                componentCode.Append("= GetComponent<");
                componentCode.Append(ToTypeName(p.RuntimeType));
                componentCode.Append(">();");
                componentsCode[i]= componentCode.ToString();
            }
            string functionName;
            if(myAccessType == AccessType.PUBLIC) {
                functionName= GetPublicFunctionName(myFunctionNode);
            }
            else {
                functionName= GetPrivateFunctionName(myFunctionNode);
            }
    		result.Append(
                GenerateFunction(indentSize,
                                 myAccessType,
                                 myScopeType,
                                 returnType,
                                 functionName,
                                 paramTypes,
                                 paramNames,
                                 componentsCode,
                                 (i)=> GenerateFunctionBody(i, myFunctionNode),
                                 myFunctionNode));						
            return result.ToString();
        }
        // -------------------------------------------------------------------
        static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              string returnType, string functionName,
                                              string[] paramTypes, string[] paramNames,
                                              string[] startCode,
                                              CodeProducer functionBody,
                                              iCS_EditorObject vsObj= null) {
            string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder("\n"+indent);
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_Function");
                if(vsObj != null && !string.IsNullOrEmpty(vsObj.Tooltip)) {
                    result.Append("(Tooltip=\"");
                    result.Append(vsObj.Tooltip);
                    result.Append("\")");
                }
                result.Append("]\n");
                result.Append(indent);
            }
            result.Append(ToAccessString(accessType));
            result.Append(" ");
            result.Append(ToScopeString(scopeType));
            result.Append(" ");
            result.Append(returnType);
            result.Append(" ");
            result.Append(functionName);
            result.Append("(");
    		int len= paramTypes.Length;
    		for(int i= 0; i < len; ++i) {
    			result.Append(paramTypes[i]);
    			result.Append(" ");
    			result.Append(paramNames[i]);
    			if(i+1 != len) {
    				result.Append(", ");
    			}
    		}
            result.Append(") {\n");
            // Add function startup code.
            if(startCode.Length != 0) {
                var indent_1= ToIndent(indentSize+1);
                foreach(var c in startCode) {
                    result.Append(indent_1);
                    result.Append(c);
                    result.Append("\n");
                }
            }
            result.Append(functionBody(indentSize+1));
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }

}

}