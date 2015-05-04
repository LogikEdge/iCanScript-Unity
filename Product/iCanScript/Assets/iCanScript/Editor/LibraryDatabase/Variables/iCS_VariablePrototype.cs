using UnityEngine;
using System;

namespace iCanScript.Editor {
    
    public class iCS_VariablePrototype {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    	public string   name= null;     ///< The name of the variable
    	public Type     type= null;     ///< The type of the variable

        // ======================================================================
        // Constructor/Destructor
        // ----------------------------------------------------------------------
        public iCS_VariablePrototype() {}
        public iCS_VariablePrototype(string _name, Type _type) {
            name= _name;
            type= _type;
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

}
