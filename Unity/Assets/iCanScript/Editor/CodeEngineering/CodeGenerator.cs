using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
		var result= new StringBuilder(GenerateVariables(indent, iStorage));
		result.Append("\n");
		result.Append(GenerateFunctions(indent, iStorage));
		return result.ToString();
	}

	// -------------------------------------------------------------------------
	public string GenerateVariables(int indent, iCS_IStorage iStorage) {
        var result= new StringBuilder();
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
        var result= new StringBuilder();
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
        var result= new StringBuilder();
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
					paramNames[i]= myNameMgr.GetNameFor(p);
				}
			}
		);
		result.Append(
            CSharpGenerator.GenerateFunction(indent,
                                             AccessType.PUBLIC,
                                             ScopeType.NONSTATIC,
                                             returnType,
                                             eObj.Name,
                                             paramTypes,
                                             paramNames,
                                             (i)=> GenerateFunctionBody(i, eObj),
                                             eObj));						
        return result.ToString();
    }
	// -------------------------------------------------------------------------
    public string GenerateFunctionBody(int indent, iCS_EditorObject node) {
        var result= new StringBuilder();
		var functionNodes= GetFunctionBodyParts(node);
		functionNodes= SortDependencies(functionNodes);
		foreach(var n in functionNodes) {
            result.Append(GenerateFunctionCall(indent, n));			
		}
        return result.ToString();
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
        var indent= CSharpGenerator.ToIndent(indentSize);
        var result= new StringBuilder(indent);
        // Declare return variable.
        var returnPort= GetReturnPort(node);
        if(returnPort != null && returnPort.EndConsumerPorts.Length != 0) {
            result.Append("var ");
            result.Append(CSharpGenerator.ToGeneratedPortName(returnPort));
            result.Append("= ");
        }
        // Simplified situation for property get.
        var memberInfo= iCS_LibraryDatabase.GetAssociatedDescriptor(node);
        var functionName= CSharpGenerator.ToMethodName(node);
        if(IsPropertyGet(memberInfo)) {
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
        foreach(var p in parameters) {
            if(p.IsInputPort) {
                var producer= p.FirstProducerPort;
                if(producer != null && producer != p) {
                    paramStrings[p.PortIndex]= myNameMgr.GetNameFor(producer);
                }
                else {
                    var v= p.InitialValue;
                    paramStrings[p.PortIndex]= CSharpGenerator.ToValueString(v);
                }
            }
            else {
                paramStrings[p.PortIndex]= "out "+myNameMgr.GetNameFor(p);
            }
        }
        // Determine function prefix.
        result.Append(FunctionCallPrefix(memberInfo, node));
        // Special case for property set.
        if(IsPropertySet(memberInfo)) {
            result.Append(CSharpGenerator.ToPropertyName(functionName));
            result.Append("= ");
            result.Append(paramStrings[0]);
        }
        // Generate function call.        
        else {
            result.Append(CSharpGenerator.GenerateFunctionCall(indentSize, functionName, paramStrings));            
        }
        result.Append(";\n");
        return result.ToString();
    }
	// -------------------------------------------------------------------------
    string FunctionCallPrefix(iCS_MemberInfo memberInfo, iCS_EditorObject node) {
        var result= new StringBuilder();
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
