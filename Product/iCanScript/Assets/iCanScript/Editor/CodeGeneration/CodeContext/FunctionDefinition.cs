using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=iCanScript.Internal.Prelude;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal.Editor.CodeGeneration {

    public class FunctionDefinition : ExecutionBlockDefinition {
        // ===================================================================
        // FIELDS
        // -------------------------------------------------------------------
        protected AccessSpecifier               myAccessSpecifier= AccessSpecifier.Private;
        protected ScopeSpecifier                myScopeSpecifier = ScopeSpecifier.NonStatic;
        protected FunctionDefinitionParameter[] myParameters     = null;
        protected List<VariableDefinition>      myVariables      = new List<VariableDefinition>();
        
        // ===================================================================
        // PROPERTIES
        // -------------------------------------------------------------------

        // ===================================================================
        // INFORMATION GATHERING FUNCTIONS
        // -------------------------------------------------------------------
        /// Builds a Function specific code context object.
        ///
        /// @param node VS objects associated with the function.
        /// @param codeBlock The code block this assignment belongs to.
        /// @return The newly created code context.
        ///
        public FunctionDefinition(iCS_EditorObject node, CodeBase parent, AccessSpecifier accessType, ScopeSpecifier scopeType)
        : base(node, parent) {
            myAccessSpecifier  = accessType;
            myScopeSpecifier   = scopeType;
            
            // Build parameter list.
            BuildParameterList();
            
            // Build execution list.
    		var generatedCode= GetFunctionBodyParts(node);
    		generatedCode= SortDependencies(generatedCode);
			BuildExecutionList(generatedCode);
        }

        // -------------------------------------------------------------------
        /// Builds the list of function parameters.
        protected virtual void BuildParameterList() {
            var parameters= GetParameters(VSObject);
			parameters= P.filter(p=> p.IsInDataPort && p.ProducerPort == null, parameters);
            myParameters= new FunctionDefinitionParameter[parameters.Length];
            foreach(var p in parameters) {
				if(p.PortIndex >= parameters.Length) {
					Debug.LogWarning("iCanScript: Internal error: Port index out of range: "+p.PortIndex);
				}
				else {
	                myParameters[p.PortIndex]= new FunctionDefinitionParameter(p, this);
				}
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
			// -- Remove from execution list --
            if(myExecutionList.Remove(toRemove)) {
                toRemove.Parent= null;
            }
			// -- Remove from variable list --
            if(myVariables.Remove(toRemove as VariableDefinition)) {
                toRemove.Parent= null;
            }
        }
        
        // -------------------------------------------------------------------
		void BuildExecutionList(CodeBase[] codeBlock) {
			EnableBlockDefinition currentEnableBlock= null;
			iCS_EditorObject[] currentEnables= new iCS_EditorObject[0];
			var len= codeBlock.Length;
			for(int i= 0; i < len; ++i) {
                var code= codeBlock[i];
				var functionEnables= code.GetRelatedEnablePorts();
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
                // Allocate Code Definition
				if(currentEnableBlock == null) {
					AddExecutable(code);
				}
				else {
					currentEnableBlock.AddExecutable(code);
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
            StringBuilder result= new StringBuilder("\n");
            // Add iCanScript tag for public functions.
            var hasDescription= !string.IsNullOrEmpty(VSObject.Description);
            // Add function comment block.
            if(hasDescription) {
                result.Append(GenerateFunctionComment(indent));
            }
            // Add Access & Scope specifiers.
            result.Append(indent);
            result.Append(ToAccessString(VSObject));
            result.Append(ToScopeString(VSObject));
            // Add return type
            var returnPort= GetReturnPort(VSObject);
            if(returnPort == null) {
                result.Append("void");
            }
            else {
                result.Append(ToTypeName(returnPort.RuntimeType));
            }
            // Add function name.
            result.Append(" ");
            if(myAccessSpecifier == AccessSpecifier.Public) {
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
    	CodeBase[] GetFunctionBodyParts(iCS_EditorObject node) {
            var code= new List<CodeBase>();
    		node.ForEachChildRecursiveDepthFirst(
    			vsObj=> {
					if(Context.IsInError(vsObj.InstanceId)) {
						Debug.LogWarning("iCanScript: Code not generated for node in Error: "+VSObject.FullName);						
						return;
					}
                    if(IsFieldOrPropertyGet(vsObj)) {
                        code.Add(new GetPropertyCallDefinition(vsObj, this));
                    }
                    else if(IsFieldOrPropertySet(vsObj)) {
                        code.Add(new SetPropertyCallDefinition(vsObj, this));
                    }
                    else if(vsObj.IsKindOfFunction && !vsObj.IsConstructor) {
                        code.Add(new FunctionCallDefinition(vsObj, this));
                    }
                    else if(vsObj.IsConstructor && !AreAllInputsConstant(vsObj)) {
                        code.Add(new ConstructorDefinition(vsObj, this));
                    }
                    else if(vsObj.IsInlineCode) {
                        code.Add(new InlineCodeDefinition(vsObj, this));
                    }
                    else if(vsObj.IsTriggerPort) {
                        if(ShouldGenerateTriggerCode(vsObj)) {
							var triggerSet     = new TriggerSetDefinition(vsObj, this);
							var triggerVariable= new TriggerVariableDefinition(vsObj, this, triggerSet);
                            code.Add(triggerVariable);
                            code.Add(triggerSet);
                        }
                    }
                    else if(vsObj.IsOutDataPort && vsObj.ParentNode == node) {
                        var portVariable= Context.GetCodeFor(vsObj);
                        if(portVariable != null) {
                            var producerPort= GraphInfo.GetProducerPort(vsObj);
                            if(producerPort != null) {
                                var consumerCode= new VariableReferenceDefinition(vsObj, this);
                                var producerCode= new VariableReferenceDefinition(producerPort, this);
                                code.Add(new AssignmentDefinition(this, consumerCode, producerCode));
                            }                            
                        }
                    }
    			}
    		);
    		return code.ToArray();
    	}

    	// -------------------------------------------------------------------------
        /// Returns list of trigger ports requiring code generation.
        ///
        /// @param node Root node from which the code will be generated.
        /// @return The list of trigger ports that need code generation.
        ///
    	iCS_EditorObject[] GetTriggerPortsNeedingCode(iCS_EditorObject node) {
    		var triggerPorts= node.FilterChildRecursive(
    			p=> {
    				if(p.IsTriggerPort) {
                        return ShouldGenerateTriggerCode(p);
                    }
    				return false;
    			}
    		);
    		return triggerPorts.ToArray();
    	}
    	// -------------------------------------------------------------------------
        /// Sorts a list a nodes so that the order is from _'producer'_ to _'consumer'_.
        ///
        /// @param nodes    List of nodes to be sorted.
        ///
        /// @todo   Resolve circular dependencies.
        ///
    	CodeBase[] SortDependencies(CodeBase[] codeBlock) {
    		var remainingCode= new List<CodeBase>(codeBlock);
    		var result= new List<CodeBase>();
    		int i= 0;
    		while(i < remainingCode.Count) {
    			if(IsIndependentFrom(remainingCode[i], remainingCode)) {
    				result.Add(remainingCode[i]);
    				remainingCode.RemoveAt(i);
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
    	bool IsIndependentFrom(CodeBase code, List<CodeBase> remainingCode) {
            // Get all dependencies.
            var node= code.VSObject;
            var dependencies= code.GetDependencies();
            foreach(var d in dependencies) {
                // TODO: Should consider buffering the data of circular dependencies.
                // Ok if the dependency is on the given node.
                if(d == node) continue;
                // Verify that the node is not dependent on a remaining node.
                foreach(var n in remainingCode) {
                    if(d == n.VSObject) {
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
            return P.map(fc=> ControlFlow.GetAllRelatedEnablePorts(fc), funcCalls);
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
        // Comment code generation
    	// -------------------------------------------------------------------------
        /// Generates a function comment block good for Doxygen.
        ///
        /// @param indent The white spaces to put at the beginning of each line.
        /// @return The formatted comment block.
        ///
        string GenerateFunctionComment(string indent) {
            var result= new StringBuilder(indent);
            result.Append(CodeBannerBottom);
            var splitDescription= SplitDescription(VSObject.Description);
            foreach(var d in splitDescription) {
                result.Append(indent);
                result.Append("/// ");
                result.Append(d);
                result.Append("\n");
            }
            var returnPort= GetReturnPort(VSObject);
            var showPorts= myParameters.Length != 0 || returnPort != null;
            if(showPorts) {
                result.Append(indent);
                result.Append("///\n");
            }
            if(myParameters.Length != 0) {
                foreach(var p in myParameters) {
                    result.Append(indent);
                    result.Append("/// @param ");
                    var paramName= NameUtility.ToFunctionParameterName(p.VSObject.CodeName);
                    result.Append(paramName);
                    var description= p.VSObject.Description;
                    if(string.IsNullOrEmpty(description)) {
                        result.Append("\n");
                        continue;
                    }
                    result.Append(' ');
                    var splitParamDescription= SplitDescription(description);
                    if(splitParamDescription != null && splitParamDescription.Length > 0) {
                        result.Append(splitParamDescription[0]);
                        result.Append("\n");
                        for(int i= 1; i < splitParamDescription.Length; ++i) {
                            result.Append(indent);
                            result.Append("///        ");
                            result.Append(new String(' ', paramName.Length+1));
                            result.Append(splitParamDescription[i]);
                            result.Append("\n");
                        }                        
                    }
                }
            }
            if(returnPort != null) {
                result.Append(indent);
                result.Append("/// @return ");
                result.Append(returnPort.Description);
                result.Append("\n");                
            }
            if(showPorts) {
                result.Append(indent);
                result.Append("///\n");
            }
            return result.ToString();
        }

    	// -------------------------------------------------------------------------
        /// Splits the provided user description into lines to be used as code
        /// comments.
        ///
        /// @param description The user description to split into lines.
        /// @return The description split in lines.
        ///
        string[] SplitDescription(string description) {
            // Split into lines.
            var result= description.Split(new Char[1]{'\n'});
            // Remove empty trailing lines.
            for(int i= result.Length-1; i >= 0; --i) {
                if(string.IsNullOrEmpty(result[i].Trim())) {
                    Array.Resize(ref result, i);
                }
                else {
                    break;
                }
            }
            return result;
        }
    }

}