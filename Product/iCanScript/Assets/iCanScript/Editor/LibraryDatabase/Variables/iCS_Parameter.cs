using UnityEngine;
using System;
using System.Collections;

public class iCS_Parameter : iCS_Variable {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_ParamDirection       direction         = iCS_ParamDirection.In;

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
    public iCS_Parameter() {}
    public iCS_Parameter(string _name, Type _type, System.Object _initialValue= null, iCS_ParamDirection _direction= iCS_ParamDirection.In)
    : base(_name, _type, _initialValue) {
        direction= _direction;
    }

    // ======================================================================
    // Common object methods.
    // ----------------------------------------------------------------------
	public override string ToString() {
	    return "{"+ToStringWithoutBraces()+"}";
	}
	public new string ToStringWithoutBraces() {
	    return base.ToStringWithoutBraces()+" :"+direction;	    
	}

}
