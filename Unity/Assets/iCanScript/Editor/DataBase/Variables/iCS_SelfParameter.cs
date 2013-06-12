using UnityEngine;
using System;
using System.Collections;

public class iCS_SelfParameter : iCS_Parameter {
    public iCS_SelfParameter(Type _type) : base(iCS_Strings.InstanceObjectName, _type, null, iCS_ParamDirection.In) {}
}
