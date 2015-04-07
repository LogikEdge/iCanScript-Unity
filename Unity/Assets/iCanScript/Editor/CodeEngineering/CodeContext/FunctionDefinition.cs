using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

    public class FunctionDefinition : ExecutionBlockDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        protected AccessType                    myAccessType    = AccessType.PRIVATE;
        protected ScopeType                     myScopeType     = ScopeType.NONSTATIC;
        protected FunctionParameterDefinition[] myParameters    = null;
        protected List<VariableDefinition>      myVariables     = new List<VariableDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param node VS objects associated with the function.
        /// @return The newly created code context.
        ///
        public FunctionDefinition(iCS_EditorObject node, CodeBase parent, AccessType accessType, ScopeType scopeType)
        : base(node, parent) {
            myAccessType  = accessType;
            myScopeType   = scopeType;
            
            // Build parameter list.
            BuildParameterList();
            
            // Build execution list.
    		var functionNodes= GetFunctionBodyParts(node);
    		functionNodes= SortDependencies(functionNodes);
			BuildExecutionList(functionNodes);
        }

        // -------------------------------------------------------------------
        /// Builds the list of function parameters.
        protected virtual void BuildParameterList() {
            var parameters= GetParameters(VSObject);
            myParameters= new FunctionParameterDefinition[parameters.Length];
            foreach(var p in parameters) {
                myParameters[p.PortIndex]= new FunctionParameterDefinition(p, this);
            }
        }
        
        // ===================================================================
        // COMMON INTERFACE FUNCTIONS
        // -------------------------------------------------------------------
		/// Resolves code dependencies.
		public override void ResolveDependencies() {
			foreach(var v in myVariables.ToArray()) {
				v.ResolveDependencies();
			}
			foreach(var e in myExecutionList.ToArray()) {
				e.ResolveDependencies();
			}
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
        /// Removes a code context from the function.
        ///
        /// @param toRemove The code context to be removed.
        ///
        public override void Remove(CodeBase toRemove) {
            if(toRemove is VariableDefinition) {
                if(myVariables.Remove(toRemove as VariableDefinition)) {
                    toRemove.Parent= null;
                }
            }
            else {
                if(myExecutionList.Remove(toRemove)) {
                    toRemove.Parent= null;
                }
            }
        }
        
        // -------------------------------------------------------------------
		void BuildExecutionList(iCS_EditorObject[] functions) {
			EnableBlockDefinition currentEnableBlock= null;
			iCS_EditorObject[] currentEnables= new iCS_EditorObject[0];
			var len= functions.Length;
			for(int i= 0; i < len; ++i) {
				var function= functions[i];
				var functionEnables= GetAllRelatedEnablePorts(function);
	            if(!IsSameConditionalContext(currentEnables, functionEnables)) {
					var enableIdx= LengthOfSameEnables(currentEnables, functionEnables);
					if(enableIdx < currentEnables.Length) {
						// Terminate existing If-Statement(s)
						var removeIdx= enableIdx;
						while(removeIdx < currentEnables.Length) {
							currentEnableBlock= currentEnableBlock.Parent as EnableBlockDefinition;
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
							var newEnableBlock= new EnableBlockDefinition(this, ifEnables.ToArray());
							if(currentEnableBlock == null) {
								AddExecutable(newEnableBlock);
							}
							else {
								currentEnableBlock.AddExecutable(newEnableBlock);
							}
							currentEnableBlock= newEnableBlock;
						}
					}
					currentEnables= functionEnables;
				}
	            var funcDef= new FunctionCallDefinition(function, this);
				if(currentEnableBlock == null) {
					AddExecutable(funcDef);
				}
				else {
					currentEnableBlock.AddExecutable(funcDef);
				}				
			}
		}

        // ===================================================================
        // CODE GENERATION FUNCTIONS
        // -------------------------------------------------------------------
        /// Generate the enable block header code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted header code for the if-statement.
        ///
        public override string GenerateHeader(int indentSize) {
            string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder("\n"+indent);
            // Add iCanScript tag for public functions.
            if(myAccessType == AccessType.PUBLIC) {
                result.Append("[iCS_Function");
                if(VSObject != null && !string.IsNullOrEmpty(VSObject.Tooltip)) {
                    result.Append("(Tooltip=\"");
                    result.Append(VSObject.Tooltip);
                    result.Append("\")");
                }
                result.Append("]\n");
                result.Append(indent);
            }
            // Add Access & Scope specifiers.
            result.Append(ToAccessString(myAccessType));
            result.Append(" ");
            result.Append(ToScopeString(myScopeType));
            // Add return type
            result.Append(" ");
            var returnPort= GetReturnPort(VSObject);
            if(returnPort == null) {
                result.Append("void");
            }
            else {
                result.Append(ToTypeName(returnPort.RuntimeType));
            }
            // Add function name.
            result.Append(" ");
            if(myAccessType == AccessType.PUBLIC) {
                result.Append(GetPublicFunctionName(VSObject));
            }
            else {
                result.Append(GetPrivateFunctionName(VSObject));
            }
            // Add parameters.
            result.Append("(");
			int len= myParameters.Length;
			for(int i= 0; i < len; ++i) {
				result.Append(myParameters[i].GenerateBody(0));
				if(i+1 != len) {
					result.Append(", ");
				}
			}
            result.Append(") {\n");
            return result.ToString();
        }

        // -------------------------------------------------------------------
        /// Generate the enable block trailer code.
        ///
        /// @param indentSize The indentation needed for the class definition.
        /// @return The formatted trailer code for the if-statement.
        ///
        public override string GenerateTrailer(int indentSize) {
            return ToIndent(indentSize)+"}\n";
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
    	iCS_EditorObject[] SortDependencies(iCS_EditorObject[] nodes) {
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
    	/// @param remainingNodes	List of nodes that should not be producing data for _'node'_
    	///
    	bool IsIndependentFrom(iCS_EditorObject node, List<iCS_EditorObject> remainingNodes) {
            // Get all dependencies.
            var dependencies= GetNodeCodeDependencies(node);
            foreach(var d in dependencies) {
                // TODO: Should consider buffering the data of circular dependencies.
                // Ok if the dependency is on the given node.
                if(d == node) continue;
                // Verify that the node is not dependent on a remaining node.
                foreach(var n in remainingNodes) {
                    if(d == n) {
                        return false;
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
            return P.map(fc=> GetAllRelatedEnablePorts(fc), funcCalls);
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

    }

}