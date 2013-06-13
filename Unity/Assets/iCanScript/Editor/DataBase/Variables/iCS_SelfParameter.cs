using UnityEngine;
using System;
using System.Collections;

public class iCS_SelfParameter : iCS_Parameter {
    public iCS_SelfParameter(iCS_ReflectionInfo _parent)
    : base(iCS_Strings.InstanceObjectName, _parent.ClassType, null, iCS_ParamDirection.In, _parent) {}
}
