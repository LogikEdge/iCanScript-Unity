using UnityEngine;
using System;
using System.Text;
using System.Collections;

namespace iCanScript.Editor.CodeEngineering {

    public static class CSharpGenerator {
        // -------------------------------------------------------------------
        const int ourTabSize= 4;
        // -------------------------------------------------------------------
        public delegate string CodeGenerator(int indent);
        public enum AccessType { PUBLIC, PRIVATE, PROTECTED, INTERNAL };
        public enum ScopeType  { STATIC, NONSTATIC };
        
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
        public static string GenerateNamespace(string namespaceName, CodeGenerator namespaceBody) {
            StringBuilder result= new StringBuilder("\nnamespace ");
            result.Append(namespaceName);
            result.Append(" {\n");
            result.Append(namespaceBody(1));
            result.Append("\n}\n");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateClass(int indentSize, AccessType accessType, ScopeType scopeType,
                                           string className, Type baseClass, CodeGenerator classBody) {
            return GenerateClass(indentSize, accessType, scopeType, className, baseClass.Name, classBody);
        }
        public static string GenerateClass(int indentSize, AccessType accessType, ScopeType scopeType,
                                           string className, string baseClass, CodeGenerator classBody) {
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
                                              CodeGenerator functionBody) {
            var paramTypeStrings= new String[paramTypes.Length];
            for(int i= 0; i < paramTypes.Length; ++i) {
                paramTypeStrings[i]= paramTypes[i].Name;
            }
            return GenerateFunction(indentSize, accessType, scopeType, returnType.Name, functionName, paramTypeStrings, paramNames, functionBody);
        }
        public static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              string returnType, string functionName,
                                              string[] paramTypes, string[] paramNames,
                                              CodeGenerator functionBody) {
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
            result.Append(") {\n");
            result.Append(functionBody(indentSize+1));
            result.Append("\n");
            result.Append(indent);
            result.Append("}\n");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string ToIndent(int indent) {
            return new String(' ', indent*ourTabSize);
        }
        // -------------------------------------------------------------------
        public static string ToAccessString(AccessType accessType) {
            switch(accessType) {
                case AccessType.PUBLIC: return "public";
                case AccessType.PRIVATE: return "private";
                case AccessType.PROTECTED: return "protected";
                case AccessType.INTERNAL: return "internal";
            }
            return "public";
        }
        public static string ToScopeString(ScopeType scopeType) {
            return scopeType == ScopeType.STATIC ? "static" : ""; 
        }
        // -------------------------------------------------------------------
        public static void WriteFile(string path, string fileName, string code) {
            TextFileUtils.WriteFile("Assets/"+path+"/"+fileName+".cs", code);
        }
    }
    
}