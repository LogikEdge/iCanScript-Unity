using UnityEngine;
using System;
using System.Collections;

public static class iCS_Types {
    // ----------------------------------------------------------------------
    // Returns true if type can be converted to another type
    public static bool IsA(Type baseType, Type derivedType) {
        for(; derivedType != null; derivedType= derivedType.BaseType) {
            if(baseType == derivedType) return true;
        }
        return false;
    }
    public static bool IsA<BASE>(Type derivedType) {
        return IsA(typeof(BASE), derivedType);
    }
    
    // ----------------------------------------------------------------------
    // Returns the default value of the given type.
    public static object DefaultValue(Type type) {
       return (type == null || type == typeof(void)) ? null : (type.IsValueType ? Activator.CreateInstance(type) : null);
    }

	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        Type inDataType= GetDataType(inType);
        Type outDataType= GetDataType(outType);
        return IsA(inDataType, outDataType);
    }
	// ----------------------------------------------------------------------
    public static Type GetDataType(Type t) {
        return t.HasElementType ? t.GetElementType() : t;
    }
}
