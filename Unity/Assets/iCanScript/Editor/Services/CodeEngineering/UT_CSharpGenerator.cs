using UnityEngine;
using System;
using System.Text;
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
                    return CSharpGenerator.GenerateClass(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, className, typeof(MonoBehaviour), GenerateClassBody);
                };

            // Generate namespace.
            var code= CSharpGenerator.GenerateNamespace(namespaceName, classGenerator);

            // Write final code to file.
            CSharpFileUtils.WriteCSharpFile("", className, usingDirectives+code);
        }
		public static string GenerateClassBody(int indent) {
			var result= new StringBuilder(CSharpGenerator.GenerateVariable(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, typeof(int), "x", "3"));
			result.Append(CSharpGenerator.GenerateVariable(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, typeof(Vector3), "v3", null));
			result.Append("\n");
			result.Append(GenerateFunctions(indent));
			return result.ToString();
		}
        public static string GenerateFunctions(int indent) {
            var result= "";
            result= CSharpGenerator.GenerateFunction(indent, AccessType.PUBLIC, ScopeType.NONSTATIC, "void", "Update", new string[0], new string[0], (_)=>"");
            return result;
        }
    }

}

