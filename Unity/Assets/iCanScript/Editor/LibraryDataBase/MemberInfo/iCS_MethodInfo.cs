using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public class iCS_MethodInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public MethodBase           method= null;
    public iCS_Parameter[]      parameters= null;
    public iCS_FunctionReturn   functionReturn= null;
    public iCS_StorageSpecifier storageSpecifier= iCS_StorageSpecifier.Instance;      

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isInstanceMember {
        get { return storageSpecifier == iCS_StorageSpecifier.Instance; }
    }
    public bool isClassMember {
        get { return storageSpecifier == iCS_StorageSpecifier.Class; }
    }
    public Type returnType {
        get { return functionReturn != null ? functionReturn.type : typeof(void); }
    } 
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MethodInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _classInfo,
                          string _name, string _description, string _iconPath,
                          MethodBase _methodBase, iCS_Parameter[] _parameters,
                          iCS_FunctionReturn _functionReturn)
    : base(objType, _classInfo, _name, _description, _iconPath) {
        method        = _methodBase;
        parameters    = _parameters;
        functionReturn= _functionReturn;
    }

    // ======================================================================
    // Specialized methods
    // ----------------------------------------------------------------------
    public string methodName {
        get { return method != null ? method.Name : null; }
    }
     
    // ======================================================================
    // From previous MemberInfo .....
    // ----------------------------------------------------------------------
    public string toString {
        get {
            return FunctionPath+iCS_MemberSeparator+FunctionSignature;            
        }
    }
    public override string ToString() {
        return toString;
    }
    // ----------------------------------------------------------------------
    public string FieldName    { get { return displayName.Substring(4); }}
    public string PropertyName { get { return displayName.Substring(4); }}
    public string VariableName { get { return displayName.Substring(4); }}
    // ----------------------------------------------------------------------
    public string MethodName {
        get {
            if(Method != null) return Method.Name;
            if(Field != null) return Field.Name;
            return null;
        }
    }
    // ----------------------------------------------------------------------
    public Type FieldType    { get { return Field.FieldType; }}
    public Type PropertyType { get { return IsGetProperty ? ReturnType : Parameters[0].type; }}
	public Type VariableType { get { return isField ? FieldType : PropertyType; }}
    // ----------------------------------------------------------------------
    public string functionSignature {
        get {
            string signature= displayName;
			// Build input string
			string inputStr= "";
            if(ObjectType == iCS_ObjectTypeEnum.InstanceMethod) {
                inputStr+= iCS_Strings.InstanceObjectName+":"+TypeName(ClassType)+", ";
            }
            foreach(var param in Parameters) {
				if(!param.type.IsByRef) {
	                inputStr+= param.name+":"+TypeName(param.type)+", ";
				}
            }
			// Add inputs to signature.
			if(inputStr != "") {
	            signature+= " ("+inputStr.Substring(0, inputStr.Length-2)+")";						
			}
			// Build output string
			int nbOfOutputs= 0;
			string outputStr= "";
            foreach(var param in Parameters) {
				if(param.type.IsByRef) {
	                outputStr+= param.name+":"+TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(ReturnType != null && ReturnType != typeof(void)) {
				++nbOfOutputs;
				if(ReturnName != null && ReturnName != "" && ReturnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/ReturnName;
				} else {
					outputStr+= ":"+TypeName(ReturnType);
				}
				outputStr+= ", ";
			}
			// Add output to signature.
			if(nbOfOutputs == 1) {
				signature+="->"+outputStr.Substring(0, outputStr.Length-2);
			}
			if(nbOfOutputs > 1) {
				signature+="->("+outputStr.Substring(0, outputStr.Length-2)+")";
			}
			return signature;
        }
    }
    // ----------------------------------------------------------------------
    public string functionSignatureNoThis {
        get {
            string signature= displayName;
			// Add inputs to signature.
			string inputStr= FunctionInputSignatureNoThis;
	        signature+= " ("+inputStr+")";						
			// Build output string
			int nbOfOutputs= 0;
			string outputStr= "";
            foreach(var param in Parameters) {
				if(param.type.IsByRef) {
	                outputStr+= param.name+":"+TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(ReturnType != null && ReturnType != typeof(void)) {
				++nbOfOutputs;
				if(ReturnName != null && ReturnName != "" && ReturnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/ReturnName;
				} else {
					outputStr+= ":"+TypeName(ReturnType);
				}
				outputStr+= ", ";
			}
			// Add output to signature.
			if(nbOfOutputs == 1) {
				signature+=" -> "+outputStr.Substring(0, outputStr.Length-2);
			}
			if(nbOfOutputs > 1) {
				signature+=" -> ("+outputStr.Substring(0, outputStr.Length-2)+")";
			}
			return signature;
        }
    }
    // ----------------------------------------------------------------------
    public List<string> inputParameterNames {
        get {
            return P.map(p-> p.name, P.filter(p-> p.type.IsByRef == false, parameters));
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> inputParameterTypes {
        get {
            return P.map(p-> p.type, P.filter(p-> p.type.IsByRef == false, parameters));
        }
    }
    // ----------------------------------------------------------------------
    public List<string> outputParameterNames {
        get {
            var paramNames= P.map(p-> p.name, P.filter(p-> p.type.IsByRef, parameters));
            if(returnType != null && returnType != typeof(void)) {
                paramNames.Add(ReturnName ?? iCS_Strings.DefaultFunctionReturnName);
			}
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> outputParameterTypes {
        get {
            var paramTypes= P.map(p-> p.type, P.filter(p-> p.type.IsByRef, parameters));
            if(returnType != null && returnType != typeof(void)) {
                paramTypes.Add(ReturnType);
			}
            return paramTypes;            
        }
    }
    // ----------------------------------------------------------------------
    public string functionSignatureNoThisNoOutput {
        get {
            string signature= displayName;
			// Add inputs to signature.
			string inputStr= functionInputSignatureNoThis;
	        return signature+" ("+inputStr+")";						
        }
    }
    // ----------------------------------------------------------------------
    public string functionInputSignatureNoThis {
        get {
			string inputStr= "";
            foreach(var param in Parameters) {
				if(!param.type.IsByRef) {
	                inputStr+= param.name+":"+iCS_Types.TypeName(param.type)+", ";
				}
            }
            if(inputStr.Length != 0) inputStr= inputStr.Substring(0, inputStr.Length-2);
            return inputStr;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/class".
    public string functionPath {
        get {
			string path= "";
			if(!String.IsNullOrEmpty(company)) path= company+"/";
			if(!String.IsNullOrEmpty(package)) path+= package+"/";
			path+= iCS_Types.TypeName(ClassType);
			return path;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/class/displayName".
    public string functionPathAndName {
        get {
            return functionPath+"/"+displayName;
        }
    }
}
