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
    public string               MethodName = null;
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
                              iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase,
                              bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                              string returnName, Type returnType) {
        // Editor object information.
		ObjectType        = objType;
        Company           = company;
        Package           = package;
		DisplayName       = name;
		ClassType         = classType;
		MethodName        = methodBase != null ? methodBase.Name : null;
        ToolTip           = toolTip;
        IconPath          = iconPath;
		ParamNames        = paramNames;
		ParamTypes        = paramTypes;
		ParamIsOuts       = paramIsOuts;
		ReturnName        = returnName;
		ReturnType        = returnType;
		ParamInitialValues= paramDefaultValues;
    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
	public MethodBase Method {
		get {
	        // Extract MethodBase for constructor.
	        MethodBase method= null;
	        if(ObjectType == iCS_ObjectTypeEnum.Constructor) {
	            method= ClassType.GetConstructor(ParamTypes);
	            if(method == null) {
	                string signature="(";
	                bool first= true;
	                foreach(var param in ParamTypes) {
	                    if(first) { first= false; } else { signature+=", "; }
	                    signature+= param.Name;
	                }
	                signature+=")";
	                Debug.LogWarning("Unable to extract constructor: "+ClassType.Name+signature);
	            }
	            return method;
	        }
	        // Extract MethodBase for class methods.
	        if(MethodName == null) return null;
			Type[] paramTypes= ParamTypes;
	        method= ClassType.GetMethod(MethodName, paramTypes);            
	        if(method == null) {
	            string signature="(";
	            bool first= true;
	            foreach(var param in paramTypes) {
	                if(first) { first= false; } else { signature+=", "; }
	                signature+= param.Name;
	            }
	            signature+=")";
	            Debug.LogWarning("iCanScript: Unable to extract MethodInfo from RuntimeDesc: "+MethodName+signature);
	        }
	        return method;				
		}
	}
	public FieldInfo Field {
		get {
	        if(MethodName == null) return null;
	        FieldInfo field= ClassType.GetField(MethodName);
	        if(field == null) {
	            Debug.LogWarning("iCanScript: Unable to extract FieldInfo from RuntimeDesc: "+MethodName);                
	        }
	        return field;					
		}
	}

}
