using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


public abstract class iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum       ObjectType      = iCS_ObjectTypeEnum.Unknown;
    public iCS_TypeInfo             parentTypeInfo  = null;
    public string                   DisplayName     = null;
    public string                   description     = null;
    public string                   IconPath        = null;


    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MemberInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                          string _name, string _description, string _iconPath) {
		ObjectType    = _objType;
		parentTypeInfo= _parentTypeInfo;
		DisplayName   = _name;
        description   = _description;
        IconPath      = _iconPath;
    }
    // ----------------------------------------------------------------------
    public override string ToString() {
        return FunctionPath+"/"+FunctionSignature;
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsTypeInfo            { get { return this is iCS_TypeInfo; }}
    public bool IsConstructor         { get { return this is iCS_ConstructorInfo; }}
    public bool IsMethod              { get { return this is iCS_MethodInfo; }}
    public bool IsField               { get { return this is iCS_FieldInfo; }}
    public bool IsEvent               { get { return this is iCS_EventInfo; }}
    public bool IsProperty            { get { return this is iCS_PropertyInfo; }}
    public bool IsInstanceField       { get { return IsField && toFieldInfo.isInstanceMember; }}
    public bool IsStaticField         { get { return IsField && toFieldInfo.isClassMember; }}
    public bool IsGetField            { get { return IsField && toFieldInfo.isGet; }}
    public bool IsSetField            { get { return IsField && toFieldInfo.isSet; }}     
    public bool IsGetInstanceField    { get { return IsInstanceField && IsGetField; }}
    public bool IsSetInstanceField    { get { return IsInstanceField && IsSetField; }}
    public bool IsGetStaticField      { get { return IsStaticField && IsGetField; }}
    public bool IsSetStaticField      { get { return IsStaticField && IsSetField; }}
    public bool IsGetProperty         { get { return IsProperty && toPropertyInfo.isGet; }}
    public bool IsSetProperty         { get { return IsProperty && toPropertyInfo.isSet; }}
    public bool IsGetInstanceProperty { get { return IsGetProperty && toPropertyInfo.isInstanceMember; }}
    public bool IsSetInstanceProperty { get { return IsSetProperty && toPropertyInfo.isInstanceMember; }}
    public bool IsGetStaticProperty   { get { return IsGetProperty && toPropertyInfo.isClassMember; }}
    public bool IsSetStaticProperty   { get { return IsSetProperty && toPropertyInfo.isClassMember; }}
    // ----------------------------------------------------------------------
    public iCS_TypeInfo         toTypeInfo          { get { return this as iCS_TypeInfo; }}
    public iCS_ConstrcutorInfo  toConstructorInfo   { get { return this as iCS_ConstrcutorInfo; }}
    public iCS_MethodInfo       toMethodInfo        { get { return this as iCS_MethodInfo; }}
    public iCS_FieldInfo        toFieldInfo         { get { return this as iCS_FieldInfo; }}
    public iCS_PropertyInfo     toPropertyInfo      { get { return this as iCS_PropertyInfo; }}
    public iCS_EventInfo        toEventInfo         { get { return this as iCS_EventInfo; }}
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
