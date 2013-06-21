using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_EventInfo : iCS_MethodBaseInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_EventInfo(iCS_TypeInfo _parentTypeInfo, string _displayName,
                         string _description, string _iconPath,
                         iCS_Parameter[] _parameters,
                         iCS_FunctionReturn _functionReturn,
                         iCS_StorageClass _storageClass)
    : base(_storageClass == iCS_StorageClass.Instance ? iCS_ObjectTypeEnum.InstanceEvent : iCS_ObjectTypeEnum.ClassEvent, _parentTypeInfo,
           _displayName, _description, _iconPath,
           _parameters, _functionReturn, _storageClass)
    {
    }

}
