using UnityEngine;
using System;
using System.Collections;

public static class WD_Types {
    // ----------------------------------------------------------------------
    // Returns true if type can be converted to another type
    public static bool IsA(Type baseType, Type derivedType) {
        if(baseType == derivedType) return true;
        if(derivedType == typeof(object)) return false;
        return IsA(baseType, derivedType.BaseType);
    }
    public static bool IsA<BASE>(Type derivedType) {
        return IsA(typeof(BASE), derivedType);
    }
    
    // ----------------------------------------------------------------------
    // Returns the default value of the given type.
    public static object DefaultValue(Type type) {
       return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        return GetDataType(outType) == GetDataType(inType);
    }
	// ----------------------------------------------------------------------
    public static Type GetDataType(Type t) {
        return t.HasElementType ? t.GetElementType() : t;
    }
}
