using UnityEngine;
using System;
using System.Text;
using System.Collections;
using CodeProducer= iCanScript.Editor.CodeEngineering.CodeGenerator.CodeProducer;
using AccessType= iCanScript.Editor.CodeEngineering.CodeGenerator.AccessType;
using ScopeType= iCanScript.Editor.CodeEngineering.CodeGenerator.ScopeType;

namespace iCanScript.Editor.CodeEngineering {

    public static class CSharpGenerator {
        // -------------------------------------------------------------------
        const int ourTabSize= 4;
        
        // -------------------------------------------------------------------
        public static string GenerateUsingDirectives(string[] usingDirectives) {
            StringBuilder result= new StringBuilder("");
            foreach(var u in usingDirectives) {
                result.Append("using ");
                result.Append(u);
                result.Append(";\n");
            }
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateNamespace(string namespaceName, CodeProducer namespaceBody) {
            StringBuilder result= new StringBuilder("\nnamespace ");
            result.Append(namespaceName);
            result.Append(" {\n");
            result.Append(namespaceBody(1));
            result.Append("\n}\n");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateClass(int indentSize, AccessType accessType, ScopeType scopeType,
                                           string className, Type baseClass, CodeProducer classBody) {
            return GenerateClass(indentSize, accessType, scopeType, className, ToTypeName(baseClass), classBody);
        }
        public static string GenerateClass(int indentSize, AccessType accessType, ScopeType scopeType,
                                           string className, string baseClass, CodeProducer classBody) {
            string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder("\n"+indent);
            result.Append(ToAccessString(accessType));
            result.Append(" ");
            result.Append(ToScopeString(scopeType));
            result.Append(" class ");
            result.Append(className);
            if(!string.IsNullOrEmpty(baseClass)) {
                result.Append(" : ");
                result.Append(baseClass);
            }
            result.Append(" {\n");
            result.Append(classBody(indentSize+1));
            result.Append("\n");
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              Type returnType, string functionName,
                                              Type[] paramTypes, string[] paramNames,
                                              CodeProducer functionBody) {
            var paramTypeStrings= new String[paramTypes.Length];
            for(int i= 0; i < paramTypes.Length; ++i) {
                paramTypeStrings[i]= ToTypeName(paramTypes[i]);
            }
            return GenerateFunction(indentSize, accessType, scopeType, ToTypeName(returnType), functionName, paramTypeStrings, paramNames, functionBody);
        }
        public static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              string returnType, string functionName,
                                              string[] paramTypes, string[] paramNames,
                                              CodeProducer functionBody) {
            string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder("\n"+indent);
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
            result.Append("\n");
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }
        // -------------------------------------------------------------------
		public static string GenerateVariable(int indentSize, AccessType accessType, ScopeType scopeType,
											  Type variableType, string variableName, string initializer) {
			var typeName= ToTypeName(variableType);
			return GenerateVariable(indentSize, accessType, scopeType, typeName, variableName, initializer);
		}
		public static string GenerateVariable(int indentSize, AccessType accessType, ScopeType scopeType,
											  string variableType, string variableName, string initializer) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            result.Append(ToAccessString(accessType));
            result.Append(" ");
            result.Append(ToScopeString(scopeType));
            result.Append(" ");
			result.Append(variableType);
			result.Append(" ");
			result.Append(variableName);
			if(!String.IsNullOrEmpty(initializer)) {
				result.Append("= ");
				result.Append(initializer);
			}
			result.Append(";\n");
			return result.ToString();
		}
        // -------------------------------------------------------------------
        public static string GenerateFunctionCall(int indentSize, string functionName) {
			string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder(indent);
            result.Append(functionName);
            result.Append("()");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string ToIndent(int indent) {
            return new String(' ', indent*ourTabSize);
        }
        // -------------------------------------------------------------------
        public static string ToAccessString(AccessType accessType) {
            switch(accessType) {
                case AccessType.PUBLIC:    return "public";
                case AccessType.PRIVATE:   return "private";
                case AccessType.PROTECTED: return "protected";
                case AccessType.INTERNAL:  return "internal";
            }
            return "public";
        }
        public static string ToScopeString(ScopeType scopeType) {
			switch(scopeType) {
				case ScopeType.STATIC:  return "static";
				case ScopeType.VIRTUAL: return "virtual";
			}
            return ""; 
        }
		public static string ToTypeName(Type type) {
			if(type == typeof(void))   return "void";
			if(type == typeof(int))    return "int";
			if(type == typeof(uint))   return "uint";
			if(type == typeof(bool))   return "bool";
			if(type == typeof(string)) return "string";
			return type.Name;
		}
        // -------------------------------------------------------------------
        public static void WriteFile(string path, string fileName, string code) {
            TextFileUtils.WriteFile("Assets/"+path+"/"+fileName+".cs", code);
        }
    }
    
}