using UnityEngine;
using System;
using System.Reflection;
using System.Collections;


public class iCS_ReflectionDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum   ObjectType = iCS_ObjectTypeEnum.Unknown;
    public string               Company    = "(no company)";
    public string               Package    = "(no package)";
    public string               DisplayName= null;
    public Type                 ClassType  = null;
    public MethodBase           Method = null;
    public FieldInfo            Field= null;
    public string               ToolTip= null;
    public string               IconPath= null;
	public string[]				ParamNames= null;
	public Type[]				ParamTypes= null;
	public bool[]				ParamIsOuts= null;
	public string				ReturnName= null;
	public Type					ReturnType= null;
	public object[]				ParamInitialValues= null;


    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ReflectionDesc(string company, string package, string name,
                              string toolTip, string iconPath,
                              iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase, FieldInfo fieldInfo,
                              bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                              string returnName, Type returnType) {
        // Editor object information.
		ObjectType        = objType;
        Company           = company;
        Package           = package;
		DisplayName       = name;
		ClassType         = classType;
		Method            = methodBase;
		Field             = fieldInfo;
        ToolTip           = toolTip;
        IconPath          = iconPath;
		ParamNames        = paramNames;
		ParamTypes        = paramTypes;
		ParamIsOuts       = paramIsOuts;
		ReturnName        = returnName;
		ReturnType        = returnType;
		ParamInitialValues= paramDefaultValues;
    }
    // ----------------------------------------------------------------------
    public override string ToString() {
        return FunctionPath+"/"+FunctionSignature;
    }
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public string MethodName {
        get {
            if(Method != null) return Method.Name;
            if(Field != null) return Field.Name;
            return null;
        }
    }
    // ----------------------------------------------------------------------
    public string FunctionSignature {
        get {
            string signature= TypeName(ReturnType);
            signature+= " "+DisplayName+"(";
            if(ObjectType == iCS_ObjectTypeEnum.InstanceMethod) {
                signature+= TypeName(ClassType)+" this";
                if(ParamNames.Length != 0) signature+=", ";
            }
            for(int i= 0; i < ParamNames.Length; ++i) {
                signature+= TypeName(ParamTypes[i])+" "+ParamNames[i];
                if(i != ParamNames.Length-1) signature+=", ";
            }
            return signature+")";            
        }
    }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public string FunctionPath { get { return Company+"/"+Package; } }
    // ----------------------------------------------------------------------
    // Returns the function name in the form of "company/package/displayName".
    public string FunctionName { get { return FunctionPath+"/"+DisplayName; }}

    // ======================================================================
    // Utilities
    // ----------------------------------------------------------------------
    static string TypeName(Type type) {
        if(type == null) return "void";
        if(type == typeof(float)) return "float";
        return type.Name;
    }
    
}
