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
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_Class(Library=\"Visual Script\")]\n");
                result.Append(indent);
            }
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
                                              CodeProducer functionBody,
                                              iCS_EditorObject vsObj= null) {
            var paramTypeStrings= new String[paramTypes.Length];
            for(int i= 0; i < paramTypes.Length; ++i) {
                paramTypeStrings[i]= ToTypeName(paramTypes[i]);
            }
            return GenerateFunction(indentSize, accessType, scopeType, ToTypeName(returnType), functionName, paramTypeStrings, paramNames, functionBody, vsObj);
        }
        public static string GenerateFunction(int indentSize, AccessType accessType, ScopeType scopeType,
                                              string returnType, string functionName,
                                              string[] paramTypes, string[] paramNames,
                                              CodeProducer functionBody,
                                              iCS_EditorObject vsObj= null) {
            functionName= ToCSharpName(functionName);
            string indent= ToIndent(indentSize);
            StringBuilder result= new StringBuilder("\n"+indent);
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_Function");
                if(vsObj != null && !string.IsNullOrEmpty(vsObj.Tooltip)) {
                    result.Append("(Tooltip=\"");
                    result.Append(vsObj.Tooltip);
                    result.Append("\")");
                }
                result.Append("]\n");
                result.Append(indent);
            }
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
            if(accessType == AccessType.PUBLIC) {
                result.Append("[iCS_InOutPort]\n");
                result.Append(indent);
            }
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
        public static string GenerateFunctionCall(int indentSize, string functionName, string[] paramValues) {
            StringBuilder result= new StringBuilder();
            result.Append(functionName);
            result.Append("(");
            var len= paramValues.Length;
            for(int i= 0; i < len; ++i) {
                result.Append(paramValues[i]);
                if(i+1 < len) {
                    result.Append(", ");                    
                }
            }
            result.Append(")");
            return result.ToString();
        }
        // -------------------------------------------------------------------
        public static string GenerateAllocator(Type type, string[] paramValues) {
            var result= new StringBuilder(" new ");
            result.Append(ToTypeName(type));
            result.Append("(");
            int len= paramValues.Length;
            for(int i= 0; i < len; ++i) {
                result.Append(paramValues[i]);
                if(i+1 < len) {
                    result.Append(", ");
                }
            }
            result.Append(")");
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
            type= iCS_Types.RemoveRefOrPointer(type);
			if(type == typeof(void))   return "void";
			if(type == typeof(int))    return "int";
			if(type == typeof(uint))   return "uint";
			if(type == typeof(bool))   return "bool";
			if(type == typeof(string)) return "string";
            if(type == typeof(float))  return "float";
			return type.Name;
		}
        public static string ToValueString(System.Object obj) {
            if(obj == null) return "null";
            var objType= obj.GetType();
            if(obj is bool) {
                return ((bool)obj) ? "true" : "false";
            }
            if(obj is string) {
                return "\""+obj.ToString()+"\"";
            }
            if(obj is char) {
                return "\'"+obj.ToString()+"\'";
            }
            if(objType.IsEnum) {
                return ToTypeName(obj.GetType())+"."+obj.ToString();
            }
            return obj.ToString();
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
                if(string.IsNullOrEmpty(eObj.Name)) {
                    var typeName= ToTypeName(eObj.RuntimeType);
                    if(typeName.StartsWith("iCS_")) {
                        typeName= typeName.Substring(4);
                    }
                    return "my"+typeName;
                }
            }
            var variableName= ToValidIdent(eObj.Name);
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
        public static string ToGeneratedPortName(iCS_EditorObject port) {
            // Return variable name if parent is a constructor.
            var parent= port.ParentNode;
            if(parent.IsConstructor) {
                return ToVariableName(parent);
            }
            // Try with port name.
            if(!string.IsNullOrEmpty(port.Name)) {
                var name= ToValidIdent(port.Name);
                return Char.ToLower(name[0])+name.Substring(1);
            }
            // Generate unique port name.
            var generatedParentName= ToGeneratedNodeName(parent);
            if(port.PortIndex == (int)iCS_PortIndex.Return) {
                return "out_"+generatedParentName;
            }
            return "p"+port.PortIndex+"_"+generatedParentName;
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
        public static string ToPropertyName(string propertyFunctionName) {
            return propertyFunctionName.Substring(4);
        }
        public static string ToCSharpName(string s) {
            return iCS_TextUtility.ToCSharpName(s);
        }
        // -------------------------------------------------------------------
        public static void WriteFile(string path, string fileName, string code) {
            TextFileUtils.WriteFile("Assets/"+path+"/"+fileName+".cs", code);
        }
    }
    
}