using UnityEngine;
using System;
using System.Collections;

public class iCS_InstanceBinding : iCS_Parameter {
    public iCS_InstanceBinding(iCS_MethodInfo _methodInfo)
    : base(iCS_Strings.InstanceObjectName, _method.classTypeInfo.CompilerType, null, iCS_ParamDirection.In, _methodInfo) {}
}
