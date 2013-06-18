using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_PropertyInfo : iCS_MethodInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_AccessorType accessorType= iCS_AccessorType.None;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet {
        get { return accessorType == iCS_AccessorType.Get || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public bool isSet {
        get { return accessorType == iCS_AccessorType.Set || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public string propertyName { get { return displayName.Substring(4); }}
    public Type type {
        get {
            return isGet ? _functionReturn.type : parameters[0].type;
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
	{
		accessorType= (_functionReturn == null || _functionReturn.type == typeof(void)) ?
							iCS_AccessorType.Set :
							iCS_AccessorType.Get;
    }

    // ======================================================================
    // Instance specific methods
    // ----------------------------------------------------------------------

}
