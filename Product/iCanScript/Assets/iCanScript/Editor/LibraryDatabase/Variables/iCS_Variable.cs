///////////////////////////////////////////////////////////
//  iCS_Variable.cs
//  Created on:      12-Jun-2013 11:51:59 AM
//  Original author: Reinual
///////////////////////////////////////////////////////////
using UnityEngine;
using System;

/// <summary>
/// Contains all attributes needed to describe a variable.  The _Variable_ type will
/// be used has base type for parameters and return values.
/// </summary>
public class iCS_Variable : iCS_VariablePrototype {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public System.Object        initialValue= null;

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
	public iCS_Variable() {}
    public iCS_Variable(string _name, Type _type, System.Object _initialValue)
    : base(_name, _type) {
        initialValue= _initialValue;
    }
	    
    // ======================================================================
    // Common object methods.
    // ----------------------------------------------------------------------
	public override string ToString() {
	    return "{"+ToStringWithoutBraces()+"}";
	}
	public new string ToStringWithoutBraces() {
	    var str= base.ToStringWithoutBraces();
	    if(initialValue != null) {
    	    str+= "= "+initialValue.ToString();	    
	    }
	    return str;
	}
}//end iCS_Variable