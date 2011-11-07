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

    // ======================================================================
    //  string to/from object conversions
	// ----------------------------------------------------------------------
    public static string ToString(Type type) {
        return type.AssemblyQualifiedName;
    }
    public static string ToString(object obj) {
        return obj.ToString();
    }
	// ----------------------------------------------------------------------
    public static T FromString<T>(string valueStr) {
        return (T)FromString(valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object FromString(string valueStr, Type type) {
        if(type == typeof(Type)) {
            return Type.GetType(valueStr);
        }
        // Use System.Convert for all object that supports IConvertible.
        Type[] useConvert= type.FindInterfaces((t,s)=> t.ToString() == s.ToString(), "System.IConvertible");
        if(useConvert.Length > 0) {
            return Convert.ChangeType(valueStr, type);
        }
        // We don't know this type ...
        Debug.LogWarning("FromString for type: "+type.Name+" is undefined.");
        return null;
    }
}
