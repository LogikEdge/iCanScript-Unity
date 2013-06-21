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
    public iCS_Parameter[]      parameters= null;
    public iCS_FunctionReturn   functionReturn= null;
    public iCS_StorageClass 	storageClass= iCS_StorageClass.Instance;      

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isInstanceMember {
        get { return storageClass == iCS_StorageClass.Instance; }
    }
    public bool isClassMember {
        get { return storageClass == iCS_StorageClass.Class; }
    }
    public Type returnType {
        get { return functionReturn != null ? functionReturn.type : typeof(void); }
    }
    public virtual string methodName {
        get { return null; }
    }
    public string returnName {
        get { return functionReturn != null ? functionReturn.name : "out"; }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MethodBaseInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _classInfo,
                              string _name, string _description, string _iconPath,
                              iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
                              iCS_StorageClass _storageClass)
    : base(objType, _classInfo, _name, _description, _iconPath) {
        parameters    = _parameters;
        functionReturn= _functionReturn;
        storageClass= _storageClass;
    }

    // ======================================================================
    // Instance Method
    // ----------------------------------------------------------------------
    public string functionSignature {
        get {
            string signature= displayName;
			// Build input string
			string inputStr= "";
            if(isInstanceMethodBase) {
                inputStr+= iCS_Strings.InstanceObjectName+":"+iCS_Types.TypeName(classType)+", ";
            }
            foreach(var param in parameters) {
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
            foreach(var param in parameters) {
				if(param.type.IsByRef) {
	                outputStr+= param.name+":"+iCS_Types.TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(returnType != null && returnType != typeof(void)) {
				++nbOfOutputs;
				if(returnName != null && returnName != "" && returnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/returnName;
				} else {
					outputStr+= ":"+iCS_Types.TypeName(returnType);
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
			string inputStr= functionInputSignatureNoThis;
	        signature+= " ("+inputStr+")";						
			// Build output string
			int nbOfOutputs= 0;
			string outputStr= "";
            foreach(var param in parameters) {
				if(param.type.IsByRef) {
	                outputStr+= param.name+":"+iCS_Types.TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(returnType != null && returnType != typeof(void)) {
				++nbOfOutputs;
				if(returnName != null && returnName != "" && returnName != iCS_Strings.DefaultFunctionReturnName) {
					outputStr+= /*" "+*/returnName;
				} else {
					outputStr+= ":"+iCS_Types.TypeName(returnType);
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
            var paramNames= new List<string>();
            foreach(var p in parameters) {
                if(!p.type.IsByRef) {
                    paramNames.Add(p.name);
                }
            }
            return paramNames;
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> inputParameterTypes {
        get {
            var paramTypes= new List<Type>();
            foreach(var p in parameters) {
                if(!p.type.IsByRef) {
                    paramTypes.Add(p.type);
                }
            }
            return paramTypes;
        }
    }
    // ----------------------------------------------------------------------
    public List<string> outputParameterNames {
        get {
            var paramNames= new List<string>();
            foreach(var p in parameters) {
                if(p.type.IsByRef) {
                    paramNames.Add(p.name);
                }
            }
            if(returnType != null && returnType != typeof(void)) {
                paramNames.Add(returnName ?? iCS_Strings.DefaultFunctionReturnName);
			}
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> outputParameterTypes {
        get {
            var paramTypes= new List<Type>();
            foreach(var p in parameters) {
                if(p.type.IsByRef) {
                    paramTypes.Add(p.type);
                }
            }
            if(returnType != null && returnType != typeof(void)) {
                paramTypes.Add(returnType);
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
            foreach(var param in parameters) {
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
			path+= iCS_Types.TypeName(classType);
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

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    public string toString {
        get {
            return functionPath+memberSeparator+functionSignature;            
        }
    }
    public override string ToString() {
        return toString;
    }

}
