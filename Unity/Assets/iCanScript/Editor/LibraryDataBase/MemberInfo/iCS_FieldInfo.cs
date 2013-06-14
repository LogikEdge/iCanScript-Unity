using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public FieldInfo        Field       = null;
    public iCS_Parameter    Parameter   = null;
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
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldInfo(iCS_TypeInfo _classInfo, string _name,
                         string _description, string _iconPath,
                         iCS_ObjectTypeEnum objType, Type classType,
                         FieldInfo _fieldInfo,
                         iCS_Parameter _parameter)
    : base(objType, _name,
           _description, _iconPath,
           _classInfo) {
        Field    = _fieldInfo;
        Parameter= _parameter;
    }

}
