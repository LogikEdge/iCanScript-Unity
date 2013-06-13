using UnityEngine;
using System;
using System.Collections;

public class iCS_Parameter : iCS_Variable {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ParamDirection       direction         = iCS_ParamDirection.In;
    public static iCS_Parameter[]   emptyParameterList= new iCS_Parameter[0];

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
    public iCS_Parameter() {}
    public iCS_Parameter(string _name, Type _type, System.Object _initialValue, iCS_ParamDirection _direction, iCS_ReflectionInfo _parent)
    : base(_name, _type, _initialValue, _parent) {
        direction= _direction;
    }
}
