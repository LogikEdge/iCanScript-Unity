using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


public abstract class iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum       ObjectType        = iCS_ObjectTypeEnum.Unknown;
    public string                   Company           = "(no company)";
    public string                   Package           = "(no package)";
    public string                   DisplayName       = null;
    public Type                     ClassType         = null;
    public string                   Tooltip           = null;
    public string                   IconPath          = null;
    public iCS_Parameter[]          Parameters        = null;
	public string				    ReturnName        = null;
    public MethodBase               Method            = null;
    public FieldInfo                Field             = null;
    public bool                     IsGetFieldFlag    = true;


    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ReflectionInfo(iCS_ObjectTypeEnum objType, string company, string package, string name,
                              string toolTip, string iconPath,
                              Type classType, MethodBase methodBase, FieldInfo fieldInfo,
                              iCS_Parameter[] parameters,
                              string returnName) {
        // Editor object information.
		ObjectType        = objType;
        Company           = company;
        Package           = package;
		DisplayName       = name;
		ClassType         = classType;
		Method            = methodBase;
		Field             = fieldInfo;
		IsGetFieldFlag    = fieldInfo != null ? (parameters.Length == 0) : true;  // FIXME: This is probably wrong...
        Tooltip           = toolTip;
        IconPath          = iconPath;
        Parameters        = parameters;
		ReturnName        = returnName ?? (Method != null && Method.IsConstructor ? "this" : iCS_Types.TypeName(ReturnType));
		
		// Fix parameter & return name for properties.
		if(IsGetProperty) ReturnName= PropertyName;
		if(IsSetProperty) Parameters[0].name= PropertyName;
    }
    // ----------------------------------------------------------------------
    public override string ToString() {
        return FunctionPath+"/"+FunctionSignature;
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsConstructor         { get { return IsMethod && Method.IsConstructor; }}
    public bool IsMethod              { get { return Method != null; }}
    public bool IsField               { get { return Field != null; }}
    public bool IsEvent               { get { return Method == null && Field == null; }}
    public bool IsInstanceField       { get { return ObjectType == iCS_ObjectTypeEnum.InstanceField; }}
    public bool IsStaticField         { get { return ObjectType == iCS_ObjectTypeEnum.StaticField; }}
    public bool IsGetField            { get { return IsField && IsGetFieldFlag; }}
    public bool IsSetField            { get { return IsField && !IsGetFieldFlag; }}     
    public bool IsGetInstanceField    { get { return IsInstanceField && IsGetField; }}
    public bool IsSetInstanceField    { get { return IsInstanceField && IsSetField; }}
    public bool IsGetStaticField      { get { return IsStaticField && IsGetField; }}
    public bool IsSetStaticField      { get { return IsStaticField && IsSetField; }}
    public bool IsProperty            { get { return IsGetProperty || IsSetProperty; }}
    public bool IsGetProperty         { get { return IsMethod && Parameters.Length == 0 && DisplayName.StartsWith("get_"); }}
    public bool IsSetProperty         { get { return IsMethod && Parameters.Length == 1 && DisplayName.StartsWith("set_"); }}
    public bool IsGetInstanceProperty { get { return ObjectType == iCS_ObjectTypeEnum.InstanceMethod && IsGetProperty; }}
    public bool IsSetInstanceProperty { get { return ObjectType == iCS_ObjectTypeEnum.InstanceMethod && IsSetProperty; }}
    public bool IsGetStaticProperty   { get { return ObjectType == iCS_ObjectTypeEnum.StaticMethod && IsGetProperty; }}
    public bool IsSetStaticProperty   { get { return ObjectType == iCS_ObjectTypeEnum.StaticMethod && IsSetProperty; }}
    // ----------------------------------------------------------------------
    public string FieldName    { get { return DisplayName.Substring(4); }}
    public string PropertyName { get { return DisplayName.Substring(4); }}
    public string VariableName { get { return DisplayName.Substring(4); }}
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
	public Type VariableType { get { return IsField ? FieldType : PropertyType; }}
    public Type ReturnType   { get { return GetReturnType(); }}
    public abstract Type GetReturnType();
    // ----------------------------------------------------------------------
    public string FunctionSignature {
        get {
            string signature= DisplayName;
			// Build input string
			string inputStr= "";
            if(ObjectType == iCS_ObjectTypeEnum.InstanceMethod) {
                inputStr+= "this"+":"+TypeName(ClassType)+", ";
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
				if(ReturnName != null && ReturnName != "" && ReturnName != "out") {
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
	                outputStr+= param.name+":"+TypeName(param.type.GetElementType())+", ";
					++nbOfOutputs;
				}
            }
			if(ReturnType != null && ReturnType != typeof(void)) {
				++nbOfOutputs;
				if(ReturnName != null && ReturnName != "" && ReturnName != "out") {
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
    public List<string> InputParameterNames {
        get {
            List<string> paramNames= new List<string>();
            foreach(var param in Parameters) {
    			if(!param.type.IsByRef) {
                    paramNames.Add(param.name);
                }
            }
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> InputParameterTypes {
        get {
            List<Type> paramNames= new List<Type>();
            foreach(var param in Parameters) {
    			if(!param.type.IsByRef) {
                    paramNames.Add(param.type);
                }
            }
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<string> OutputParameterNames {
        get {
            List<string> paramNames= new List<string>();
            foreach(var param in Parameters) {
    			if(param.type.IsByRef) {
                    paramNames.Add(param.name);
                }
            }
            if(ReturnType != null && ReturnType != typeof(void)) {
                paramNames.Add(ReturnName ?? "out");
			}
            return paramNames;            
        }
    }
    // ----------------------------------------------------------------------
    public List<Type> OutputParameterTypes {
        get {
            List<Type> paramNames= new List<Type>();
            foreach(var param in Parameters) {
    			if(param.type.IsByRef) {
                    paramNames.Add(param.type);
                }
            }
            if(ReturnType != null && ReturnType != typeof(void)) {
                paramNames.Add(ReturnType);
			}
            return paramNames;            
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
	                inputStr+= param.name+":"+TypeName(param.type)+", ";
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
			if(!IsEmptyStr(Company)) path= Company+"/";
			if(!IsEmptyStr(Package)) path+= Package+"/";
			path+= TypeName(ClassType);
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
    // Utilities
    // ----------------------------------------------------------------------
    static string TypeName(Type type) {
        return iCS_Types.TypeName(type);
    }
    static bool IsEmptyStr(string s) {
		return iCS_Strings.IsEmpty(s);
	}
}