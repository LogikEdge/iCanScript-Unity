using UnityEngine;
using System;
using System.Collections;

public static class WD_Types {
	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        return GetDataType(outType) == GetDataType(inType);
    }
	// ----------------------------------------------------------------------
    public static Type GetDataType(Type t) {
        return t.HasElementType ? t.GetElementType() : t;
    }
}
