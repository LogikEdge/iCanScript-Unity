using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using P=Prelude;

public class iCS_MethodBaseInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_Parameter[]      Parameters= null;
    public iCS_FunctionReturn   FunctionReturn= null;
    public iCS_StorageClass 	StorageClass= iCS_StorageClass.Instance;      

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsInstanceMember {
        get { return StorageClass == iCS_StorageClass.Instance; }
    }
    public bool IsClassMember {
        get { return StorageClass == iCS_StorageClass.Class; }
    }
    public Type ReturnType {
        get { return FunctionReturn != null ? FunctionReturn.type : typeof(void); }
    }
    public virtual string MethodName {
        get { return null; }
    }
    public string ReturnName {
        get { return FunctionReturn != null ? FunctionReturn.name : "out"; }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MethodBaseInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _classInfo,
                              string _name, string _description, string _iconPath,
                              iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
                              iCS_StorageClass _storageClass)
    : base(objType, _classInfo, _name, _description, _iconPath) {
        Parameters    = _parameters;
        FunctionReturn= _functionReturn;
        StorageClass  = _storageClass;
    }

    // ======================================================================
    // Instance Method
    // ----------------------------------------------------------------------
    public string FunctionSignature {
        get {
            string signature= DisplayName;
			// Build input string
			string inputStr= "";
            if(IsInstanceMethodBase) {
                inputStr+= iCS_Strings.InstanceObjectName+":"+iCS_Types.TypeName(ClassType)+", ";
            }
            foreach(var param in Parameters) {
				if(!param.type.IsByRef) {
	                inputStr+= param.name+":"+iCS_Types.TypeName(param.type)+", ";
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
	                outputStr+= param.name+":"+iCS_Types.TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(ReturnType != null && ReturnType != typeof(void)) {
				++nbOfOutputs;
				if(ReturnName != null && ReturnName != "" && ReturnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/ReturnName;
				} else {
					outputStr+= ":"+iCS_Types.TypeName(ReturnType);
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
    public string FunctionSignatureNoThis {
        get {
            string signature= DisplayName;
			// Add inputs to signature.
			string inputStr= FunctionInputSignatureNoThis;
	        signature+= " ("+inputStr+")";						
			// Build output string
			int nbOfOutputs= 0;
			string outputStr= "";
            foreach(var param in Parameters) {
				if(param.type.IsByRef) {
	                outputStr+= param.name+":"+iCS_Types.TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(ReturnType != null && ReturnType != typeof(void)) {
				++nbOfOutputs;
				if(ReturnName != null && ReturnName != "" && ReturnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/ReturnName;
				} else {
					outputStr+= ":"+iCS_Types.TypeName(ReturnType);
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
    public List<string> InputParameterNames {
        get {
            var paramNames= new List<string>();
            foreach(var p in Parameters) {
                if(!p.type.IsByRef) {
                    paramNames.Add(p.name);
                }
            }
            return paramNames;
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> InputParameterTypes {
        get {
            var paramTypes= new List<Type>();
            foreach(var p in Parameters) {
                if(!p.type.IsByRef) {
                    paramTypes.Add(p.type);
                }
            }
            return paramTypes;
        }
    }
    // ----------------------------------------------------------------------
    public List<string> OutputParameterNames {
        get {
            var paramNames= new List<string>();
            foreach(var p in Parameters) {
                if(p.type.IsByRef) {
                    paramNames.Add(p.name);
                }
            }
            if(ReturnType != null && ReturnType != typeof(void)) {
                paramNames.Add(ReturnName ?? iCS_Strings.DefaultFunctionReturnName);
			}
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> OutputParameterTypes {
        get {
            var paramTypes= new List<Type>();
            foreach(var p in Parameters) {
                if(p.type.IsByRef) {
                    paramTypes.Add(p.type);
                }
            }
            if(ReturnType != null && ReturnType != typeof(void)) {
                paramTypes.Add(ReturnType);
			}
            return paramTypes;            
        }
    }
    // ----------------------------------------------------------------------
    public string FunctionSignatureNoThisNoOutput {
        get {
            string signature= DisplayName;
			// Add inputs to signature.
			string inputStr= FunctionInputSignatureNoThis;
	        return signature+" ("+inputStr+")";						
        }
    }
    // ----------------------------------------------------------------------
    public string FunctionInputSignatureNoThis {
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
    public string FunctionPath {
        get {
			string path= "";
			if(!String.IsNullOrEmpty(Company)) path= Company+"/";
			if(!String.IsNullOrEmpty(Package)) path+= Package+"/";
			path+= iCS_Types.TypeName(ClassType);
			return path;
        }
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/class/displayName".
    public string FunctionPathAndName {
        get {
            return FunctionPath+"/"+DisplayName;
        }
    }

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    public override string ToString() {
        return FunctionPath+memberSeparator+FunctionSignature;            
    }

}
