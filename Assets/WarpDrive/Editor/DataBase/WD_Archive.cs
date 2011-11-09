using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_Archive {
    // ======================================================================
    //  string to/from object conversions
	// ----------------------------------------------------------------------
    public static string Encode(Type type) {
        return type.AssemblyQualifiedName;
    }
    public static string Encode(object obj) {
        if(obj.GetType().IsEnum) {
            int enumValue= (int)obj;
            return enumValue.ToString()+"("+obj.ToString()+")";
        }
        return obj.ToString();
    }
	// ----------------------------------------------------------------------
    public static T Decode<T>(string valueStr) {
        return (T)Decode(valueStr, typeof(T));
    }
	// ----------------------------------------------------------------------
    public static object Decode(string valueStr, Type type) {
        if(type == typeof(Type)) {
            return Type.GetType(valueStr);
        }
        if(type.IsEnum) {
            int end= valueStr.IndexOf('(');
            if(end < 0) end= valueStr.Length;
            return Decode<int>(valueStr.Substring(0, end));
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
