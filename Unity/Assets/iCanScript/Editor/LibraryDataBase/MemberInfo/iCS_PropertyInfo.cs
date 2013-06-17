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
    
    // ======================================================================
    // Builders
    // ----------------------------------------------------------------------
    public iCS_PropertyInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                            string _name, string _description, string _iconPath,
                            MethodBase _methodBase,
                            iCS_Parameter[] _parameters,
                            iCS_FunctionReturn _functionReturn)
    : base(_objType, _parentTypeInfo, _name, _description, _iconPath, _methodBase, _parameters, _functionReturn) {
        
    }
}
