using UnityEngine;
using System;
using System.Text;
using System.Collections;
using CodeProducer= iCanScript.Editor.CodeEngineering.CodeGenerator.CodeProducer;
using AccessType= iCanScript.Editor.CodeEngineering.CodeGenerator.AccessType;
using ScopeType= iCanScript.Editor.CodeEngineering.CodeGenerator.ScopeType;

namespace iCanScript.Editor.CodeEngineering {

    public static class CSharpGenerator {
		public static string ToTypeName(Type type) {
            type= iCS_Types.RemoveRefOrPointer(type);
			if(type == typeof(void))   return "void";
			if(type == typeof(int))    return "int";
			if(type == typeof(uint))   return "uint";
			if(type == typeof(bool))   return "bool";
			if(type == typeof(string)) return "string";
            if(type == typeof(float))  return "float";
			return type.Name;
		}
        public static string ToMethodName(iCS_EditorObject vsObj) {
            var n= vsObj.MethodName;
            if(n == ".ctor") {
                return vsObj.RuntimeType.Name;
            }
            return iCS_TextUtility.ToCSharpName(n);
        }
        public static string ToVariableName(iCS_EditorObject eObj) {
            if(eObj.IsConstructor) {
                if(string.IsNullOrEmpty(eObj.DisplayName)) {
                    var typeName= ToTypeName(eObj.RuntimeType);
                    if(typeName.StartsWith("iCS_")) {
                        typeName= typeName.Substring(4);
                    }
                    return "my"+typeName;
                }
            }
            var variableName= ToValidIdent(eObj.DisplayName);
            variableName= Char.ToLower(variableName[0])+variableName.Substring(1);
            return variableName;
        }
        public static string ToGeneratedNodeName(iCS_EditorObject node) {
            var methodName= ToMethodName(node);
            if(!string.IsNullOrEmpty(methodName)) {
                return methodName+node.InstanceId;
            }
            return "node"+node.InstanceId;
        }
        public static string ToValidIdent(string str) {
            var result= new StringBuilder();
            for(int cursor= 0; cursor < str.Length; ++cursor) {
                var c= str[cursor];
                if(Char.IsLetterOrDigit(c) || c == '_') {
                    result.Append(c);
                }
                else {
                    switch(c) {
                        case ' ': break;
                        case '+': result.Append("_plus_"); break;
                        case '-': result.Append("_minus_"); break;
                        case '*': result.Append("_mul_"); break;
                        case '/': result.Append("_div_"); break;
                        case '&': result.Append("_and_"); break;
                        case '|': result.Append("_or_"); break;
                        default: result.Append('_'); break;
                    }
                }
            }
            return result.ToString();
        }
    }
    
}