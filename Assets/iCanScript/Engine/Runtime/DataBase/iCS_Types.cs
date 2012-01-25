using UnityEngine;
using System;
using System.Collections;

public static class iCS_Types {
    // ----------------------------------------------------------------------
    // Returns true if type can be converted to another type
    public static bool IsA(Type baseType, Type derivedType) {
		if((baseType.IsArray && !derivedType.IsArray) || (!baseType.IsArray && derivedType.IsArray)) return false;
        for(; derivedType != null; derivedType= derivedType.BaseType) {
            if(baseType == derivedType) return true;
        }
        return false;
    }
    public static bool IsA<BASE>(Type derivedType) {
        return IsA(typeof(BASE), derivedType);
    }
    
    // ----------------------------------------------------------------------
    // Returns the coded name for the given type.
    public static string GetName(Type type) {
        string result= type.Name;
        int arg= result.IndexOf('`');
        if(arg < 0) return result;
        result= result.Substring(0, arg);
        if(type.IsGenericType) {
            result+= "<";
            bool comma= false;
            foreach(var t in type.GetGenericArguments()) {
                if(!comma) { comma= true; } else { result+= ","; }
               result+= GetName(t); 
            }
            result+= ">";
        }
        return result;
    }
    
    // ----------------------------------------------------------------------
    // Returns true if the given type has a default constructor.
    public static bool CreateInstanceSupported(Type type) {
        if(type.IsArray) return CreateInstanceSupported(GetElementType(type));
        if(type.GetElementType() == typeof(string)) return true;
        var defaultConstructor= type.GetConstructor(Type.EmptyTypes);
        return defaultConstructor != null;
    }
    public static bool HasDefaultConstructor<T>() {
        return CreateInstanceSupported(typeof(T));
    }
    
    // ----------------------------------------------------------------------
    // Returns the default value of the given type.
    public static object CreateInstance(Type type) {
        if(type == null || type == typeof(void)) return null;
        if(type.IsArray) return Array.CreateInstance(type.GetElementType(),0);
        if(type == typeof(string) || (type.HasElementType && type.GetElementType() == typeof(string))) return "";
        if(IsA<UnityEngine.ScriptableObject>(type)) return ScriptableObject.CreateInstance(type);
        try {
            return Activator.CreateInstance(type);            
        }
        catch {
            return null;
        }
    }

    // ----------------------------------------------------------------------
    // Returns the default value of the given type.
    public static object DefaultValue(Type type) {
        if(type == null || type == typeof(void)) return null;
        if(type.IsEnum) {
            Array enumValues= Enum.GetValues(type);
            if(enumValues.Length <= 0) return null;
            return Enum.ToObject(type, enumValues.GetValue(0));
        }
        return (type.IsValueType || type.IsEnum ? Activator.CreateInstance(type) : null);
    }

	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        Type inDataType= GetElementType(inType);
        Type outDataType= GetElementType(outType);
        return IsA(inDataType, outDataType);
    }
	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithUpConversion(Type outType, Type inType) {
        Type inDataType= GetElementType(inType);
        Type outDataType= GetElementType(outType);
        return IsA(outDataType, inDataType);
    }
	// ----------------------------------------------------------------------
    public static Type GetElementType(Type t) {
        return t.HasElementType ? t.GetElementType() : t;
    }
	// ----------------------------------------------------------------------
    public static string GetTypeName(Type type) {
		return type == typeof(float) ? "float" : type.Name;
    }
}
