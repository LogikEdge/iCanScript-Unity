using UnityEngine;
using System;
using System.Collections;
using CodeGenerator= iCanScript.Editor.CodeEngineering.CSharpGenerator.CodeGenerator;
using AccessType= iCanScript.Editor.CodeEngineering.CSharpGenerator.AccessType;
using ScopeType= iCanScript.Editor.CodeEngineering.CSharpGenerator.ScopeType;

public class UnitTest {}

namespace iCanScript.Editor.CodeEngineering {

    public class UT_CSharpGenerator : UnitTest {
        
        public static void GenerateTestCSharpFile() {
            var namespaceName= "iCanScript.Engine.GeneratedCode";
            var className= iCS_TextUtility.ToClassName("Fred");

            // Generate using directives.
            var usingDirectives= CSharpGenerator.GenerateUsingDirectives(new string[]{"UnityEngine"});

            // Define function to define class
            CodeGenerator classGenerator=
                (indent)=> {
                    return CSharpGenerator.GenerateClass(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, className, typeof(MonoBehaviour), GenerateFunctions);
                };

            // Generate namespace.
            var code= CSharpGenerator.GenerateNamespace(namespaceName, classGenerator);

            // Write final code to file.
            CSharpFileUtils.WriteCSharpFile("", className, usingDirectives+code);
        }
        public static string GenerateFunctions(int indent) {
            var result= "";
            result= CSharpGenerator.GenerateFunction(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, "void", "Update", new string[0], new string[0], (_)=>"");
            return result;
        }
    }

}

