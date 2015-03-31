using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionDefinition : CodeContext {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        iCS_EditorObject  myFunctionNode= null;  ///< VS objects associated with code context
        AccessType        myAccessType= AccessType.PRIVATE;
        ScopeType         myScopeType = ScopeType.NONSTATIC;
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject FunctionNode {
            get { return myFunctionNode; }
        }
        
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param associatedObjects VS objects associated with this code context.
        /// @return The newly created code context.
        ///
        public FunctionDefinition(iCS_EditorObject functionNode, AccessType accessType, ScopeType scopeType)
        : base(CodeType.FUNCTION) {
            myFunctionNode= functionNode;
            myAccessType  = accessType;
            myScopeType   = scopeType;
        }

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the code for a function definition.
        ///
        /// @param indentSize The indentation of the function.
        /// @return The generated code for the given function.
        ///
        public string GenerateCode(int indentSize) {
            var result= new StringBuilder(1024);
    		// Find return type.
    		string returnType= ToTypeName(typeof(void));
    		var nbParams= 0;
    		myFunctionNode.ForEachChildPort(
    			p=> {
    				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
    					if(p.PortIndex+1 > nbParams) {
    						nbParams= p.PortIndex+1;
    					}
    				}
    				if(p.PortIndex == (int)iCS_PortIndex.Return) {
    					returnType= ToTypeName(p.RuntimeType);
    				}
    			}
    		);
    		// Build parameters
    		var paramTypes= new string[nbParams];
    		var paramNames= new string[nbParams];
    		myFunctionNode.ForEachChildPort(
    			p=> {
    				var i= p.PortIndex;
    				if(i < (int)iCS_PortIndex.ParametersEnd) {
    					paramNames[i]= GetFunctionParameterName(p);
    					paramTypes[i]= ToTypeName(p.RuntimeType);
                        if(p.IsOutputPort) {
                            paramTypes[i]= "out "+paramTypes[i];
                        }
    				}
    			}
    		);
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
                                 (i)=> GenerateFunctionBody(i, myFunctionNode),
                                 myFunctionNode));						
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              string returnType, string functionName,
                                              string[] paramTypes, string[] paramNames,
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
            result.Append(functionBody(indentSize+1));
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }

    	// -------------------------------------------------------------------------
        public string GenerateFunctionBody(int indentSize, iCS_EditorObject node) {
            var result= new StringBuilder(512);
    		var functionNodes= GetFunctionBodyParts(node);
    		functionNodes= SortDependencies(functionNodes);
            var conditionalContexts= GetConditionalContexts(functionNodes);
            var len= functionNodes.Length;
            iCS_EditorObject[] currentConditionalContext= new iCS_EditorObject[0];
            for(int i= 0; i < len; ++i) {
                var cond= conditionalContexts[i];
                if(!IsSameConditionalContext(cond, currentConditionalContext)) {
                    result.Append(GenerateEndConditionalFragment(ref indentSize, cond, currentConditionalContext));
                    result.Append(GenerateOpenConditionalFragment(ref indentSize, cond, currentConditionalContext));
                    currentConditionalContext= cond;
                }
                var fc= functionNodes[i];
                result.Append(GenerateFunctionCall(indentSize, fc));
            }
            var closingConditionalContext= new iCS_EditorObject[0];
            result.Append(GenerateEndConditionalFragment(ref indentSize, closingConditionalContext, currentConditionalContext));
            return result.ToString();
        }

    	// -------------------------------------------------------------------------
        /// Returns list of nodes required for code generation
        ///
        /// @param node Root node from which the code will be generated.
        ///
    	static iCS_EditorObject[] GetFunctionBodyParts(iCS_EditorObject node) {
    		var functionBodyParts= node.FilterChildRecursive(
    			p=> {
    				if(p.IsKindOfFunction && !p.IsConstructor) return true;
    				return false;
    			}
    		);
    		return functionBodyParts.ToArray();
    	}
    	// -------------------------------------------------------------------------
        /// Sorts a list a nodes so that the order is from _'producer'_ to _'consumer'_.
        ///
        /// @param nodes    List of nodes to be sorted.
        ///
        /// @todo   Resolve circular dependencies.
        ///
    	static iCS_EditorObject[] SortDependencies(iCS_EditorObject[] nodes) {
    		var remainingNodes= new List<iCS_EditorObject>(nodes);
    		var result= new List<iCS_EditorObject>();
    		int i= 0;
    		while(i < remainingNodes.Count) {
    			if(IsIndependentFrom(remainingNodes[i], remainingNodes)) {
    				result.Add(remainingNodes[i]);
    				remainingNodes.RemoveAt(i);
    				i= 0;				
    			}			
    			else {
    				++i;
    			}
    		}
    		if(i != 0) {
    			Debug.LogWarning("Circular dependency found!!!");
    		}
    		return result.ToArray();
    	}
    	// -------------------------------------------------------------------------
    	/// Verifies that no input port is binded to a node that is included in the
        /// given node list.
    	///
    	/// @param node		The node on which to validate input port dependencies.
    	/// @param allNodes	List of nodes that should not be producing data for _'node'_
    	///
    	static bool IsIndependentFrom(iCS_EditorObject node, List<iCS_EditorObject> allNodes) {
    		var childPorts= node.BuildListOfChildPorts(p=> p.IsInputPort);
    		foreach(var p in childPorts) {
    			var producerPort= p.FirstProducerPort;
    			if(producerPort != null && producerPort != p) {
    				var producerNode= producerPort.ParentNode;
    				if(producerNode != node) {
    					foreach(var n in allNodes) {
    						if(n == producerNode) {
    							return false;
    						}
    					}
    				}
    			}
    		}
    		return true;
    	}

    	// -------------------------------------------------------------------------
        /// Builds of list of conditional contexts associated with the list of
        /// function calls.
        ///
        /// @param funcCalls List of function calls.
        /// @return List of conditional context associated with the list of function
        ///         calls.
        ///
        iCS_EditorObject[][] GetConditionalContexts(iCS_EditorObject[] funcCalls) {
            return P.map(fc=> GetEnablePortsRecursive(fc), funcCalls);
        }

    	// -------------------------------------------------------------------------
        /// Generate a function call to the given node.
        ///
        /// @param indentSize   Indent for the generated source code.
        /// @param node The node representing the function to call.
        ///
        /// @return The source code fragment to call the given function.
        ///
        /// @todo   Assure that local variable created have a unique name.
        /// @todo   Auto-create class variable for external link to objects.
        /// @todo   Support properties.
        ///
        public string GenerateFunctionCall(int indentSize, iCS_EditorObject node) {
            var indent= ToIndent(indentSize);
            var result= new StringBuilder(indent, 128);
            // Simplified situation for property get.
            var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(node);
            var functionName= GetPublicFunctionName(node);
            if(IsPropertyGet(memberInfo)) {
                // Declare return variable.
                result.Append(DeclareReturnVariable(node));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, node));
                // Generate function call.
                result.Append(ToPropertyName(node));
                result.Append(";\n");
                return result.ToString();
            }
            // Determine parameters.
            var parameters= GetParameters(node);
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
                result.Append(FunctionCallPrefix(memberInfo, node));
                result.Append(ToPropertyName(node));
                result.Append("= ");
                result.Append(paramStrings[0]);
            }
            // Generate function call.        
            else {
                // Declare the output parameters.
                result.Append(DeclarelocalVariablesForOutputParameters(indentSize, outputParams));
                // Declare return variable.
                result.Append(DeclareReturnVariable(node));
                // Determine function prefix.
                result.Append(FunctionCallPrefix(memberInfo, node));
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
        // Conditional code generation
    	// -------------------------------------------------------------------------
        /// Determine if the current and the given enable context is the same.
        ///
        /// @param newEnablePorts List for the new enable ports context.
        /// @param currentEnablePorts List of active enable ports.
        /// @return _true_ if the same enable port context;  _false_ otherwise.
        ///
        bool IsSameConditionalContext(iCS_EditorObject[] newEnablePorts, iCS_EditorObject[] currentEnablePorts) {
            var len= newEnablePorts.Length;
            if(len != currentEnablePorts.Length) return false;
            for(int i= 0; i < len; ++i) {
                if(newEnablePorts[i] != currentEnablePorts[i]) {
                    return false;
                }
            }
            return true;
        }
    	// -------------------------------------------------------------------------
        /// Closes the conditional context up until we reach a common parent
        /// context given by the new condition.
        ///
        /// @param newEnablePorts The new enable port context.
        /// @param currentEnablePorts List of active enable ports.
        /// @return The closing conditional code snippet.
        ///
        string GenerateEndConditionalFragment(ref int indentSize,
                                              iCS_EditorObject[] newEnablePorts,
                                              iCS_EditorObject[] currentEnablePorts) {
            // Skip common contexts 
            int i= 0;
            var len= currentEnablePorts.Length;
            var newLen= newEnablePorts.Length;
            var minLen= Mathf.Min(len, newLen);
            for(; i < minLen && currentEnablePorts[i] == newEnablePorts[i]; ++i);
            if(i >= len) return "";
            // Generate closing code.
            var result= new StringBuilder(32);
            for(; i < len; ++i) {
                --indentSize;
                result.Append(ToIndent(indentSize));
                result.Append("}\n");
                // Consume the multiple conditions ored together.
                while(i != len-1) {
                    if(currentEnablePorts[i].ParentNode != currentEnablePorts[i+1].ParentNode) {
                        break;
                    }
                    ++i;
                }
            }
            return result.ToString();
        }
    	// -------------------------------------------------------------------------
        /// Opens the conditional context up until we reach the common parent
        /// context given by the given condition.
        ///
        /// @param newEnablePorts The new enable port context.
        /// @param currentEnablePorts List of active enable ports.
        /// @return The open enable port context code snippet.
        ///
        string GenerateOpenConditionalFragment(ref int indentSize,
                                               iCS_EditorObject[] newEnablePorts,
                                               iCS_EditorObject[] currentEnablePorts) {
            // Skip common contexts 
            int i= 0;
            var len= currentEnablePorts.Length;
            var newLen= newEnablePorts.Length;
            var minLen= Mathf.Min(len, newLen);
            for(; i < minLen && currentEnablePorts[i] == newEnablePorts[i]; ++i);
            if(i == newLen) return "";
            // Generate opening code.
            var result= new StringBuilder(32);
            for(; i < newLen; ++i) {
                var e= newEnablePorts[i];
                result.Append(ToIndent(indentSize));
                result.Append("if(");
                result.Append(GetNameFor(e.FirstProducerPort));
                // OR all enable ports on same node.
                while(i != newLen-1) {
                    if(e.ParentNode != newEnablePorts[i+1].ParentNode) {
                        break;
                    }
                    result.Append(" || ");
                    ++i;
                    e= newEnablePorts[i];
                    result.Append(GetNameFor(e.FirstProducerPort));
                }
                result.Append(") {\n");
                ++indentSize;
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
        /// Returns the list of enable ports that affects the function call
        ///
        /// @param funcNode Visual script representing the function call.
        /// @return Array of all enable ports that affects the function call.
        ///
        static iCS_EditorObject[] GetEnablePortsRecursive(iCS_EditorObject funcNode) {
            var enablePorts= new List<iCS_EditorObject>();
            while(funcNode != null) {
                GetEnablePorts(enablePorts, funcNode);
                funcNode= funcNode.ParentNode;
            }
            enablePorts.Reverse();
            return enablePorts.ToArray();
        }
    	// -------------------------------------------------------------------------
        /// Appends to the given list the enable ports on the given node.
        ///
        /// @param lst The list to append to.
        /// @param node The node from which to extract the enable ports.
        /// @return The input list is updated with the found enable ports.
        ///
        static List<iCS_EditorObject> GetEnablePorts(List<iCS_EditorObject> lst, iCS_EditorObject node) {
            node.ForEachChildPort(
                p=> {
                    if(p.IsEnablePort) {
                        lst.Add(p);
                    }
                }
            );
            return lst;
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