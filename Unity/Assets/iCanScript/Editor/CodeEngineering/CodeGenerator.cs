using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace iCanScript.Editor.CodeEngineering {

public static class CodeGenerator {
    // -------------------------------------------------------------------
    public delegate string CodeProducer(int indent);
    public enum AccessType { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
    public enum ScopeType  { STATIC, NONSTATIC, VIRTUAL };
    
	// -------------------------------------------------------------------------
    public static void GenerateCodeFor(iCS_IStorage iStorage) {
        var namespaceName= "iCanScript.Engine.GeneratedCode";
        var className= iCS_TextUtility.ToClassName(iStorage.HostGameObject.name);

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
        CSharpFileUtils.WriteCSharpFile("", className, usingDirectives+code);
    }

	// -------------------------------------------------------------------------
	public static string GenerateClassBody(int indent, iCS_IStorage iStorage) {
		var result= new StringBuilder(GenerateVariables(indent, iStorage));
		result.Append("\n");
		result.Append(GenerateFunctions(indent, iStorage));
		return result.ToString();
	}

	// -------------------------------------------------------------------------
	public static string GenerateVariables(int indent, iCS_IStorage iStorage) {
        var result= new StringBuilder();
		// Generate static variables.
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
            var variableName= CSharpGenerator.ToVariableName(n);
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
            var variableName= CSharpGenerator.ToVariableName(n);
			result.Append(CSharpGenerator.GenerateVariable(indent, accessType, ScopeType.NONSTATIC, n.RuntimeType, variableName, initializer));
        }
		return result.ToString();
	}
	// -------------------------------------------------------------------------
    public static string GenerateFunctions(int indent, iCS_IStorage iStorage) {
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
    public static string GenerateFunction(int indent, iCS_EditorObject eObj) {
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
					paramNames[i]= p.Name;
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
                                             (i)=> GenerateFunctionBody(i, eObj)));						
        return result.ToString();
    }
	// -------------------------------------------------------------------------
    public static string GenerateFunctionBody(int indent, iCS_EditorObject node) {
        var result= new StringBuilder();
		var functionNodes= GetFunctionBodyParts(node);
		functionNodes= SortDependencies(functionNodes);
		foreach(var n in functionNodes) {
            result.Append(GenerateFunctionCall(indent, n));			
		}
        return result.ToString();
    }
	// -------------------------------------------------------------------------
    public static string GenerateFunctionCall(int indentSize, iCS_EditorObject node) {
        var result= new StringBuilder(CSharpGenerator.ToIndent(indentSize));
        // Declare return variable.
        var returnPort= GetReturnPort(node);
        if(returnPort != null && returnPort.EndConsumerPorts.Length != 0) {
            result.Append("var ");
            result.Append(CSharpGenerator.ToGeneratedPortName(returnPort));
            result.Append("= ");
        }
        // Determine parameters.
        var parameters= GetParameters(node);
        var pLen= parameters.Length;
        var paramStrings= new string[pLen];        
        foreach(var p in parameters) {
            if(p.IsInputPort) {
                var producer= p.FirstProducerPort;
                if(producer != null && producer != p) {
                    paramStrings[p.PortIndex]= CSharpGenerator.ToGeneratedPortName(producer);
                }
                else {
                    var v= p.InitialValue;
                    paramStrings[p.PortIndex]= CSharpGenerator.ToValueString(v);
                }
            }
            else {
                paramStrings[p.PortIndex]= "Outch!";
            }
        }
        // Determine function prefix.
        var desc= iCS_LibraryDatabase.GetAssociatedDescriptor(node);
        if(desc != null && desc.IsClassFunctionBase) {
            result.Append(CSharpGenerator.ToTypeName(node.RuntimeType));
            result.Append(".");
        }
        else {
            var thisPort= GetThisPort(node);
            if(thisPort != null) {
                var producer= thisPort.FirstProducerPort;
                if(producer != null && producer != thisPort) {
                    result.Append(CSharpGenerator.ToGeneratedPortName(producer));
                    result.Append(".");
                }
            }
        }
        // Generate function call.
        var functionName= CSharpGenerator.ToMethodName(node);
        result.Append(CSharpGenerator.GenerateFunctionCall(indentSize, functionName, paramStrings));
        result.Append(";\n");
        return result.ToString();
    }
    // =========================================================================
    // Utilities
	// -------------------------------------------------------------------------
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
    static iCS_EditorObject[] GetParameters(iCS_EditorObject node) {
        var parameters= new List<iCS_EditorObject>();
        ForEachParameter(node, p=> parameters.Add(p));
        return parameters.ToArray();
    }
	// -------------------------------------------------------------------------
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
    static iCS_EditorObject[] GetClassStaticVariables(iCS_IStorage iStorage) {
        return new iCS_EditorObject[0];
    }
    static iCS_EditorObject[] GetClassNonStaticVariables(iCS_IStorage iStorage) {
		return iStorage[0].Filter(n=> n.IsConstructor).ToArray();
    }
	// -------------------------------------------------------------------------
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
	// Verifies that no input port is binded to a node in the given node list.
	//
	// @param node		The node on which to validate input port dependencies.
	// @param allNodes	List of nodes that should not be producing data for _'node'_
	//
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
}

}
