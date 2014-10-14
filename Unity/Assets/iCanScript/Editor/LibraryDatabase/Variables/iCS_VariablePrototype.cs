using UnityEngine;
using System;

public class iCS_VariablePrototype {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public string               name        = null;
	public Type                 type        = null;

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
    public iCS_VariablePrototype() {}
    public iCS_VariablePrototype(string _name, Type _type) {
        name        = _name;
        type        = _type;
    }
	    
    // ======================================================================
    // Common object methods.
    // ----------------------------------------------------------------------
	public override string ToString() {
	    return "{"+ToStringWithoutBraces()+"}";
	}
	public string ToStringWithoutBraces() {
	    return name+":"+type;
	}
}
