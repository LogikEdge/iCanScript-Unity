using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

namespace iCanScript.Editor.CodeEngineering {

public class CodeGenerator {
    // -------------------------------------------------------------------
    public enum CodeType     { CLASS, FUNCTION, VARIABLE, PARAMETER };
    public enum AccessType   { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
    public enum ScopeType    { STATIC, NONSTATIC, VIRTUAL };
    public enum LocationType { LOCAL_TO_FUNCTION, LOCAL_TO_CLASS };
    
    // -------------------------------------------------------------------
    public delegate string CodeProducer(int indent);

    // -------------------------------------------------------------------
    CodeGeneratorNameManager   myNameMgr= null;
    
	// -------------------------------------------------------------------------
    public void GenerateCodeFor(iCS_IStorage iStorage) {
        // Prepare generated name array.
        myNameMgr= new CodeGeneratorNameManager(iStorage);
        
        // Define namespace & class name based on GameObject name.
        var namespaceName= "iCanScript.Engine.GeneratedCode";
        var className= iCS_TextUtility.ToCSharpName(iStorage.HostGameObject.name);

        // Generate using directives.
        var usingDirectives= CSharpGenerator.GenerateUsingDirectives(new string[]{"UnityEngine"});

        // Define function to define class
        CodeProducer classGenerator=
            (indent)=> {
                return CSharpGenerator.GenerateClass(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, className, typeof(MonoBehaviour), (i)=> GenerateClassBody(i, iStorage));
            };

        // Generate namespace.
        var code= CSharpGenerator.GenerateNamespace(namespaceName, classGenerator);

        // Write final code to file.
        CSharpFileUtils.WriteCSharpFile("iCanScript Generated Code", className, usingDirectives+code);
    }

	// -------------------------------------------------------------------------
	public string GenerateClassBody(int indent, iCS_IStorage iStorage) {
		var result= new StringBuilder(GenerateVariables(indent, iStorage), 2048);
		result.Append("\n");
		result.Append(GenerateFunctions(indent, iStorage));
		return result.ToString();
	}

	// -------------------------------------------------------------------------
	public string GenerateVariables(int indent, iCS_IStorage iStorage) {
        var result= new StringBuilder(128);
		// Generate static variables.
        var classNode= iStorage[0];
        var allConstructors= GetClassStaticVariables(iStorage);
        foreach(var n in allConstructors) {
            var nbOfParams= GetNbOfParameters(n);
            var initValues= new string[nbOfParams];
            n.ForEachChildPort(
                p=> {
                    if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        var v= p.InitialValue;
                        initValues[p.PortIndex]= CSharpGenerator.ToValueString(v);                                
                    }
                }
            );
            var initializer= CSharpGenerator.GenerateAllocator(n.RuntimeType, initValues);
            var accessType= n.ParentId == 0 ? AccessType.PUBLIC : AccessType.PRIVATE;
            myNameMgr.SetCodeParent(n, classNode);
            var variableName= myNameMgr.GetNameFor(n);
			result.Append(CSharpGenerator.GenerateVariable(indent, accessType, ScopeType.STATIC, n.RuntimeType, variableName, initializer));
        }
		// Generate non-static variables.
        allConstructors= GetClassNonStaticVariables(iStorage);
        foreach(var n in allConstructors) {
            var nbOfParams= GetNbOfParameters(n);
            var initValues= new string[nbOfParams];
            n.ForEachChildPort(
                p=> {
                    if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                        var v= p.InitialValue;
                        initValues[p.PortIndex]= CSharpGenerator.ToValueString(v);                                
                    }
                }
            );
            var initializer= CSharpGenerator.GenerateAllocator(n.RuntimeType, initValues);
            var accessType= n.ParentId == 0 ? AccessType.PUBLIC : AccessType.PRIVATE;
            myNameMgr.SetCodeParent(n, classNode);
            var variableName= myNameMgr.GetNameFor(n);
			result.Append(CSharpGenerator.GenerateVariable(indent, accessType, ScopeType.NONSTATIC, n.RuntimeType, variableName, initializer));
        }
		return result.ToString();
	}
	// -------------------------------------------------------------------------
    public string GenerateFunctions(int indent, iCS_IStorage iStorage) {
        var result= new StringBuilder(1024);
		// Find root functions.
		iStorage[0].ForEachChildNode(
			n=> {
				if(n.IsMessage || n.IsPublicFunction) {
                    result.Append(GenerateFunction(indent, n));
				}
			}
		);
        return result.ToString();
    }
	// -------------------------------------------------------------------------
    public string GenerateFunction(int indent, iCS_EditorObject eObj) {
        var result= new StringBuilder(1024);
		// Find return type.
		var returnType= typeof(void);
		var nbParams= 0;
		eObj.ForEachChildPort(
			p=> {
				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
					if(p.PortIndex+1 > nbParams) {
						nbParams= p.PortIndex+1;
					}
				}
				if(p.PortIndex == (int)iCS_PortIndex.Return) {
					returnType= p.RuntimeType;
				}
			}
		);
		// Build parameters
		var paramTypes= new Type[nbParams];
		var paramNames= new String[nbParams];
		eObj.ForEachChildPort(
			p=> {
				var i= p.PortIndex;
				if(i < (int)iCS_PortIndex.ParametersEnd) {
					paramTypes[i]= p.RuntimeType;
					paramNames[i]= myNameMgr.ToFunctionParameterName(p);
				}
			}
		);
		result.Append(
            CSharpGenerator.GenerateFunction(indent,
                                             AccessType.PUBLIC,
                                             ScopeType.NONSTATIC,
                                             returnType,
                                             eObj.CodeName,
                                             paramTypes,
                                             paramNames,
                                             (i)=> GenerateFunctionBody(i, eObj),
                                             eObj));						
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
//	// -------------------------------------------------------------------------
//    /// Generates the IF-STATEMENT associated with the active enable ports.
//    ///
//    /// @param indentSize   Indent for the generated source code.
//    /// @param funcNode Visual script node of the function call.
//    /// @param funcCallGenerator The code generator for the function call.
//    public string GenerateConditionalCode(ref int indentSize,
//                                     iCS_EditorObject funcNode,
//                                     CodeProducer funcCallGenerator) {
//        var enablePorts= GetEnablePortsRecursive(funcNode);
//        if(IsSameConditionalContext(enablePorts)) {
//            return funcCallGenerator(indentSize);
//        }
//        var result= new StringBuilder(512);
//        result.Append(GenerateEndConditionalFragment(ref indentSize, enablePorts));
//        result.Append(GenerateOpenConditionalFragment(ref indentSize, enablePorts));
//        myEnablePortsContext= enablePorts;
//        result.Append(funcCallGenerator(indentSize+1));
//        return result.ToString();
//    }
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
        var indent= CSharpGenerator.ToIndent(indentSize);
        var result= new StringBuilder(indent, 128);
        // Simplified situation for property get.
        var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(node);
        var functionName= CSharpGenerator.ToMethodName(node);
        if(IsPropertyGet(memberInfo)) {
            // Declare return variable.
            result.Append(DeclareReturnVariable(node));
            // Determine function prefix.
            result.Append(FunctionCallPrefix(memberInfo, node));
            // Generate function call.
            result.Append(CSharpGenerator.ToPropertyName(functionName));
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
                var producer= p.FirstProducerPort;
                if(producer != null && producer != p) {
                    paramStrings[p.PortIndex]= myNameMgr.ToLocalVariableName(producer);
                }
                else {
                    var v= p.InitialValue;
                    paramStrings[p.PortIndex]= CSharpGenerator.ToValueString(v);
                }
            }
            else {
                outputParams.Add(p);
                paramStrings[p.PortIndex]= "out "+myNameMgr.ToLocalVariableName(p);
            }
        }
        // Special case for property set.
        if(IsPropertySet(memberInfo)) {
            // Determine function prefix.
            result.Append(FunctionCallPrefix(memberInfo, node));
            result.Append(CSharpGenerator.ToPropertyName(functionName));
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
            result.Append(CSharpGenerator.GenerateFunctionCall(indentSize, functionName, paramStrings));            
        }
        result.Append(";\n");
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
            result.Append(CSharpGenerator.ToTypeName(node.RuntimeType));
            result.Append(".");
        }
        else {
            var thisPort= GetThisPort(node);
            if(thisPort != null) {
                var producerPort= thisPort.FirstProducerPort;
                if(producerPort != null && producerPort != thisPort) {
                    var producerNode= producerPort.ParentNode;
                    if(producerNode.IsConstructor) {
                        result.Append(myNameMgr.GetNameFor(producerNode));                                                
                    }
                    else {
                        result.Append(myNameMgr.GetNameFor(producerPort));                        
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
        result.Append(myNameMgr.ToLocalVariableName(returnPort));
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
            result.Append(CSharpGenerator.ToTypeName(p.RuntimeType));
            result.Append(" ");
            result.Append(myNameMgr.ToLocalVariableName(p));
            result.Append(";\n");
            result.Append(CSharpGenerator.ToIndent(indentSize));
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
            result.Append(CSharpGenerator.ToIndent(indentSize));
            result.Append("}\n");
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
        for(; i < len && currentEnablePorts[i] == newEnablePorts[i]; ++i);
        var newLen= newEnablePorts.Length;
        if(i == newLen) return "";
        // Generate opening code.
        var result= new StringBuilder(32);
        for(; i < newLen; ++i) {
            var e= newEnablePorts[i];
            result.Append(CSharpGenerator.ToIndent(indentSize));
            result.Append("if(");
            result.Append(myNameMgr.GetNameFor(e.FirstProducerPort));
            result.Append(") {\n");
            ++indentSize;
        }
        return result.ToString();        
    }

    // =========================================================================
    // Utilities
	// -------------------------------------------------------------------------
    /// Iterates through the parameters of the given node.
    ///
    /// @param node The parent node of the parameter ports to iterate on.
    /// @param fnc  The action to run for each parameter port.
    ///
    static void ForEachParameter(iCS_EditorObject node, Action<iCS_EditorObject> fnc) {
        node.ForEachChildPort(
    		p=> {
    			if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
                    fnc(p);
    			}
    		}        
        );
    }
	// -------------------------------------------------------------------------
    /// Returns a list of parameters for the given node.
    ///
    /// @param node The node from which to extract the parameter ports.
    ///
    /// @return List of existing parameter ports.
    ///
    /// @note Parameters are expected to be continuous from port index 0 to
    ///       _'nbOfParameters'_.
    ///
    static iCS_EditorObject[] GetParameters(iCS_EditorObject node) {
        var parameters= new List<iCS_EditorObject>();
        ForEachParameter(node, p=> parameters.Add(p));
        return parameters.ToArray();
    }
	// -------------------------------------------------------------------------
    /// Returns the number of function parameters for the given node.
    ///
    /// @param node The node from which to get the parameter ports.
    ///
    /// @return The number of parameter ports that exists on the _'node'_.
    ///
    /// @note Parameters are expected to be continuous from port index 0 to
    ///       _'nbOfParameters'_.
    ///
    static int GetNbOfParameters(iCS_EditorObject node) {
		var nbParams= 0;
		node.ForEachChildPort(
			p=> {
				if(p.PortIndex < (int)iCS_PortIndex.ParametersEnd) {
					if(p.PortIndex+1 > nbParams) {
						nbParams= p.PortIndex+1;
					}
				}
			}
		);
        return nbParams;        
    }
	// -------------------------------------------------------------------------
    /// Returns the function return port.
    ///
    /// @param node The node in which to search for a return port.
    ///
    /// @return _'null'_ is return if no return port is found.
    ///
    static iCS_EditorObject GetReturnPort(iCS_EditorObject node) {
        iCS_EditorObject result= null;
        node.ForEachChildPort(
            p=> {
                if(p.PortIndex == (int)iCS_PortIndex.Return) {
                    result= p;
                }
            }
        );
        return result;
    }
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
    /// Returns a list of variable instances that are static member of the class.
    ///
    /// @param iStorage The visual script editor storage of the class.
    /// 
    static iCS_EditorObject[] GetClassStaticVariables(iCS_IStorage iStorage) {
        return new iCS_EditorObject[0];
    }
	// -------------------------------------------------------------------------
    /// Returns a list of variable instances that are non-static memebers
    /// of the class.
    ///
    /// @param iStorage The visual script editor storage of the class.
    /// 
    static iCS_EditorObject[] GetClassNonStaticVariables(iCS_IStorage iStorage) {
		return iStorage[0].Filter(n=> n.IsConstructor).ToArray();
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
