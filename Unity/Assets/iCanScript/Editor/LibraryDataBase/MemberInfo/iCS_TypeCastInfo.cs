using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_TypeCastInfo : iCS_MethodInfo {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_TypeCastInfo(iCS_TypeInfo _classTypeInfo, string _name,
                            string _description, string _iconPath,
                            MethodBase _methodBase,
                            iCS_Parameter[] _parameters,
                            iCS_FunctionReturn _functionReturn)
    : base(iCS_ObjectTypeEnum.TypeCast, _classTypeInfo, _name, _description, _iconPath, 
           _methodBase, _parameters, _functionReturn)
    {}
}
