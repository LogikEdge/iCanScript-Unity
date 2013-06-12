using UnityEngine;
using System;
using System.Collections;

public class iCS_FunctionReturn : iCS_Parameter {
    public iCS_FunctionReturn(string _name, Type _type, System.Object _initialValue)
    : base(_name, _type, _initialValue, iCS_ParamDirection.Out) {
    }
}
