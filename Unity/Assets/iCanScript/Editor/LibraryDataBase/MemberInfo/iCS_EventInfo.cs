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
                         iCS_FunctionReturn _functionReturn)
    : base(iCS_ObjectTypeEnum.Event, _parentTypeInfo, _displayName,
           _description, _iconPath,
           _parameters, _functionReturn)
    {
    }

}
