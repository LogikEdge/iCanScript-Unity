using UnityEngine;
using System;
using System.Collections;

public class iCS_Parameter : iCS_Variable {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ParamDirection direction= iCS_ParamDirection.In;

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
    public iCS_Parameter() {}
    public iCS_Parameter(string _name, Type _type, System.Object _initialValue, iCS_ParamDirection _direction)
    : base(_name, _type, _initialValue) {
        direction= _direction;
    }
}
