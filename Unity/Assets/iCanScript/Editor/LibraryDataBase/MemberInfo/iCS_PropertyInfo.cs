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
    public iCS_PropertyInfo(iCS_TypeInfo _parentTypeInfo, string _name,
                          string _toolTip, string _iconPath,
                          iCS_ObjectTypeEnum _objType, MethodBase methodBase,
                          iCS_Parameter[] parameters,
                          string returnName)
    : base(_parentTypeInfo, _name, _toolTip, _iconPath, _objType, methodBase, parameters, returnName) {
        
    }
}
