using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_PropertyInfo : iCS_MethodInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet { get { return parameters.Length == 0; }}
    public bool isSet { get { return !isGet; }}
    public string propertyName { get { return displayName.Substring(4); }}
    public Type type {
        get {
            return isGet ? functionReturn.type : parameters[0].type;
        }
    }
    
    // ======================================================================
    // Builders
    // ----------------------------------------------------------------------
    public iCS_PropertyInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                            string _name, string _description, string _iconPath,
                            iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
							MethodBase _methodBase)
    : base(_objType, _parentTypeInfo, _name, _description, _iconPath, _parameters, _functionReturn, _methodBase)
	{}

}
