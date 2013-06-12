///////////////////////////////////////////////////////////
//  iCS_Variable.cs
//  Created on:      12-Jun-2013 11:51:59 AM
//  Original author: Reinual
///////////////////////////////////////////////////////////
using UnityEngine;
using System;

/// <summary>
/// Contains all attributes needed to describe a variable.  The Variable type will
/// be used has base type for parameters and return values.
/// </summary>
public class iCS_Variable {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
	public string          name        = null;
	public Type            type        = null;
	public System.Object   initialValue= null;

    // ======================================================================
    // Constructor/Destructor
    // ----------------------------------------------------------------------
	public iCS_Variable() {}
	    public iCS_Variable(string _name, Type _type, System.Object _initialValue) {
	        name= _name;
	        type= _type;
	        initialValue= _initialValue;
	    }
}//end iCS_Variable