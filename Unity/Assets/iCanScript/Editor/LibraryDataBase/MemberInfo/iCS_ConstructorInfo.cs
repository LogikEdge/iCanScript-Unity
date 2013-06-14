using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ConstructorInfo : iCS_MethodInfo {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ConstructorInfo(iCS_TypeInfo _classInfo,
                               string _description, string _iconPath,
                               MethodBase _methodBase, iCS_Parameter[] _parameters)
    : base(iCS_ObjectTypeEnum.Constructor, _classInfo,
           iCS_Types.TypeName(_classInfo.CompilerType), _description, _iconPath,
           _methodBase, _parameters, iCS_Strings.InstanceObjectName)
    {
    }
}
