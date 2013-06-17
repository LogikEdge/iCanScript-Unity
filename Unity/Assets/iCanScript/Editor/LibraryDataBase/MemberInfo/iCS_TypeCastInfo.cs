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
                            iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
							MethodBase _methodBase)
    : base(iCS_ObjectTypeEnum.TypeCast, _classTypeInfo, _name, _description, _iconPath, 
           _parameters, _functionReturn, _methodBase)
    {}
}
