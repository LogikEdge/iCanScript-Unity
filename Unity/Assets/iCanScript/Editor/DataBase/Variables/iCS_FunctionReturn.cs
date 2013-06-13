using UnityEngine;
using System;
using System.Collections;

public class iCS_FunctionReturn : iCS_Parameter {
    public iCS_FunctionReturn(string _name, Type _type, iCS_ReflectionInfo _parent)
    : base(_name, _type, iCS_Types.DefaultValue(_type), iCS_ParamDirection.Out, _parent) {}
}
