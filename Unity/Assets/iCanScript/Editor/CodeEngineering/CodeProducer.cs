using UnityEngine;
using System;
using System.Text;
using System.Collections;
using CodeGenerator= iCanScript.Editor.CodeEngineering.CSharpGenerator.CodeGenerator;
using AccessType= iCanScript.Editor.CodeEngineering.CSharpGenerator.AccessType;
using ScopeType= iCanScript.Editor.CodeEngineering.CSharpGenerator.ScopeType;


namespace iCanScript.Editor.CodeEngineering {

public static class CodeProducer {
	// -------------------------------------------------------------------------
    public static void GenerateCodeFor(iCS_IStorage iStorage) {
        var namespaceName= "iCanScript.Engine.GeneratedCode";
        var className= iCS_TextUtility.ToClassName(iStorage.HostGameObject.name);

        // Generate using directives.
        var usingDirectives= CSharpGenerator.GenerateUsingDirectives(new string[]{"UnityEngine"});

        // Define function to define class
        CodeGenerator classGenerator=
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
		// Find root variables.
		iStorage[0].ForEachChildNode(
			n=> {
				if(n.IsConstructor) {
					result.Append(CSharpGenerator.GenerateVariable(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, n.RuntimeType, n.Name, null));						
				}
			}
		);
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
    public static string GenerateFunctionBody(int indent, iCS_EditorObject eObj) {
        var result= new StringBuilder();
        eObj.ForEachChildNode(
            n=> {
                if(n.IsKindOfFunction) {
                    var functionName= n.Name;
                    result.Append(CSharpGenerator.GenerateFunctionCall(indent, functionName));
                    result.Append(";\n");
                }
            }
        );
        return result.ToString();
    }
}

}
