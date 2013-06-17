using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ConstructorInfo : iCS_MethodInfo {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ConstructorInfo(iCS_TypeInfo _classInfo,
                               string _displayName, string _description, string _iconPath,
                               iCS_Parameter[] _parameters, MethodBase _methodBase)
    : base(iCS_ObjectTypeEnum.Constructor, _classInfo,
           _displayName, _description, _iconPath,
           _methodBase, _parameters,
           iCS_FunctionReturn(iCS_Strings.InstanceObjectName, _classInfo.compilerType))
    {
    }
}
