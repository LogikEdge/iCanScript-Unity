using UnityEngine;
using System;
using System.Collections;

public static class WD_Types {
	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        // No problem if both port are of the same type.
        if(outType == inType) return true;
        // Direct connection is also allowed if output is a reference.
        if(!outType.HasElementType) return false;
        outType= outType.GetElementType();
        return outType == inType;
    }
}
