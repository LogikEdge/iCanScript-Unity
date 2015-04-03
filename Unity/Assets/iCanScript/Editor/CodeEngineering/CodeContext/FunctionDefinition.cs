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
        protected iCS_EditorObject         myFunctionNode = null;  ///< VS objects associated with code context
        protected AccessType               myAccessType   = AccessType.PRIVATE;
        protected ScopeType                myScopeType    = ScopeType.NONSTATIC;
        protected List<CodeContext>        myExecutionList= new List<CodeContext>();
        protected List<VariableDefinition> myVariables    = new List<VariableDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------
        /// Returns the VS objects associated with code context
        public iCS_EditorObject FunctionNode {
            get { return myFunctionNode; }
        }
        
        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param node VS objects associated with the function.
        /// @return The newly created code context.
        ///
        public FunctionDefinition(iCS_EditorObject node, CodeContext parent, AccessType accessType, ScopeType scopeType)
        : base(CodeType.FUNCTION, node, parent) {
            myFunctionNode= node;
            myAccessType  = accessType;
            myScopeType   = scopeType;
            
            // Get list of function calls.
    		var functionNodes= GetFunctionBodyParts(node);
            // Rearrange execution order according to dependencies
    		functionNodes= SortDependencies(functionNodes);
            // Build execution list.
			BuildExecutionList(functionNodes);
        }

        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			foreach(var v in myVariables) {
				v.ResolveDependencies();
			}
			foreach(var e in myExecutionList) {
				e.ResolveDependencies();
			}
		}
        // -------------------------------------------------------------------
		/// Adds an executable child code context.
		///
		/// @param executable The child code context to add.
		///
		public override void AddExecutable(CodeContext executable) {
			myExecutionList.Add(executable);
			executable.Parent= this;
		} 
        // -------------------------------------------------------------------
        /// Adds a local variable to the top-level context of the function.
        ///
        /// @param variableDefinition The local variable to add.
        ///
        public override void AddVariable(VariableDefinition variableDefinition) {
            myVariables.Add(variableDefinition);
            variableDefinition.Parent= this;
        }
        // -------------------------------------------------------------------
        public override void AddType(TypeDefinition typeDefinition) {
            Debug.LogWarning("iCanScript: Adding a type definition to function is currently not supported.");
        }
        public override void AddFunction(FunctionDefinition functionDefinition) {
            Debug.LogWarning("iCanScript: Adding a nested function definition inside a function is currently not supported.");
        }

        // -------------------------------------------------------------------
		void BuildExecutionList(iCS_EditorObject[] functions) {
			IfStatementDefinition currentIfStatement= null;
			iCS_EditorObject[] currentEnables= new iCS_EditorObject[0];
			var len= functions.Length;
			for(int i= 0; i < len; ++i) {
				var function= functions[i];
				var functionEnables= GetEnablePortsRecursive(function);
	            if(!IsSameConditionalContext(currentEnables, functionEnables)) {
					var enableIdx= LengthOfSameEnables(currentEnables, functionEnables);
					if(enableIdx < currentEnables.Length) {
						// Terminate existing If-Statement(s)
						var removeIdx= enableIdx;
						while(removeIdx < currentEnables.Length) {
							currentIfStatement= currentIfStatement.Parent as IfStatementDefinition;
							do {
								++removeIdx;								
							} while(removeIdx < currentEnables.Length && currentEnables[removeIdx-1].ParentNode == currentEnables[removeIdx].ParentNode);
						}
					}
					if(enableIdx < functionEnables.Length) {
						// Add new If-Statement(s)
						var addIdx= enableIdx;
						while(addIdx < functionEnables.Length) {
							var ifEnables= new List<iCS_EditorObject>();
							do {
								ifEnables.Add(functionEnables[addIdx]);
								++addIdx;								
							} while(addIdx < functionEnables.Length && functionEnables[addIdx-1].ParentNode == functionEnables[addIdx].ParentNode);
							var newIfStatement= new IfStatementDefinition(this, ifEnables.ToArray());
							if(currentIfStatement == null) {
								AddExecutable(newIfStatement);
							}
							else {
								currentIfStatement.AddExecutable(newIfStatement);
							}
							currentIfStatement= newIfStatement;
						}
					}
					currentEnables= functionEnables;
				}
	            var funcDef= new FunctionCallDefinition(function, this);
				if(currentIfStatement == null) {
					AddExecutable(funcDef);
				}
				else {
					currentIfStatement.AddExecutable(funcDef);
				}				
			}
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
        static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
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
			foreach(var c in myExecutionList) {
				result.Append(c.GenerateCode(indentSize));
			}
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
		/// Returns the length of the common portion of two enable port lists.
		///
		/// @param enablePorts1 The first enable port list.
		/// @param enablePorts2 The second enable port list.
		/// @return The length of the common portion of both input lists.
		///
        int LengthOfSameEnables(iCS_EditorObject[] enablePorts1,
                                iCS_EditorObject[] enablePorts2) {
            var len1= enablePorts1.Length;
            var len2= enablePorts2.Length;
            var minLen= Mathf.Min(len1, len2);
            int i= 0;
            for(; i < minLen && enablePorts1[i] == enablePorts2[i]; ++i);
            return i;
        }
        // =========================================================================
        // Utilities
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

    }

}