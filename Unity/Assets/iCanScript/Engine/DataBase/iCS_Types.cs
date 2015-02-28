//#define DEBUG
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using iCanScript.Engine;

public static class iCS_Types {
    // ----------------------------------------------------------------------
    // Returns true if type can be converted to another type
    public static bool IsA(Type baseType, Type derivedType) {
		if((baseType.IsArray && !derivedType.IsArray) || (!baseType.IsArray && derivedType.IsArray)) return false;
        baseType= RemoveRefOrPointer(baseType);
        for(; derivedType != null; derivedType= derivedType.BaseType) {
			derivedType= RemoveRefOrPointer(derivedType);
            if(baseType == derivedType) return true;
        }
        return false;
    }
    public static bool IsA<BASE>(Type derivedType) {
        return IsA(typeof(BASE), derivedType);
    }
    
    // ----------------------------------------------------------------------
    // Returns true if the given type is a static class.
    public static bool IsStaticClass(Type t) {
        return t.IsAbstract && t.IsSealed;
    }
    // ----------------------------------------------------------------------
    // Returns true if type is passed by reference
    public static bool IsPassedByRef(Type type) {
        return GetElementType(type).IsClass;
    }
    // ----------------------------------------------------------------------
    // Returns the coded name for the given type.
    public static string GetName(Type type) {
        string result= RemoveProductPrefix(type.Name);
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
    public static string RemoveProductPrefix(string name) {
        if(name.StartsWith(iCS_Config.ProductPrefix)) {
            return name.Substring(iCS_Config.ProductPrefix.Length);
        }
        return name;
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
        // Can't create default value if we don't have a type...
        if(type == null || type == typeof(void)) return null;
        // Take the first enum value for enumerations.
        if(type.IsEnum) {
            Array enumValues= Enum.GetValues(type);
            if(enumValues.Length <= 0) return null;
            return Enum.ToObject(type, enumValues.GetValue(0));
        }
        // Remove reference & pointer modifiers.
        type= RemoveRefOrPointer(type);
        // Create default value type.
        return (type.IsValueType || type.IsEnum ? Activator.CreateInstance(type) : null);
    }

	// ----------------------------------------------------------------------
    public static bool CanBeConnectedWithoutConversion(Type outType, Type inType) {
        // Accept all .NET native array connections.
        if(inType == typeof(System.Array) && outType.IsArray) {
            return true;
        }
        // Otherwise verify to type compatibility.
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
	public static Type RemoveRefOrPointer(Type type) {
        if(type == null) return null;
        if(type.IsByRef || type.IsPointer) type= GetElementType(type);		
		return type;
	}
	// ----------------------------------------------------------------------
    public static Type GetElementType(Type t) {
		if(t == null) return null;
        return t.HasElementType ? t.GetElementType() : t;
    }
    // ----------------------------------------------------------------------
    public static string TypeName(Type type) {
        if(type == null) return "void";
        string name= null;
        if(type.HasElementType) {
            name= TypeName(type.GetElementType());
            if(type.IsArray) name+= "[]";
            if(type.IsByRef) name+= "&";
            if(type.IsPointer) name+= "*";
            return name;
        }
        if(type == typeof(float)) return "float";
        if(type == typeof(bool)) return "bool";
        if(type == typeof(void)) return "void";
        return RemoveProductPrefix(type.Name);
    }
    // ----------------------------------------------------------------------
    public static Type FindType(string name) {
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var classType in assembly.GetTypes()) {
                if(classType.Name == name) {
                    return classType;
                }
            }
        }
        return null;
    }
    // ----------------------------------------------------------------------
    public static MethodInfo FindFunction(string typeName, string functionName) {
        var interfaceType= iCS_Types.FindType(typeName);
        if(interfaceType == null) {
            Debug.LogWarning("iCanScript: unable to find type=> "+typeName+" <= in application.");
            return null;
        }
        MethodInfo methodInfo= interfaceType.GetMethod(functionName,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) {
            Debug.LogWarning("iCanScript: unable to find function: "+functionName);
            return null;
        }
        return methodInfo;
    }
    // ----------------------------------------------------------------------
    public static Type TypeFromAssemblyQualifiedName(string assemblyQualifiedName) {
        bool dummy= false;
        return TypeFromAssemblyQualifiedName(assemblyQualifiedName, out dummy);
    }
    // ----------------------------------------------------------------------
    public static void GetAssemblyQualifiedNameComponents(string assemblyQualifiedName, ref string typeName, ref string assemblyName) {
        var typeIdent= assemblyQualifiedName.Split(new char[]{','});
        typeName= typeIdent[0];
        assemblyName= typeIdent[1];
    }
    // ----------------------------------------------------------------------
    public static Type TypeFromAssemblyQualifiedName(string assemblyQualifiedName, out bool conversionPerformed) {
        conversionPerformed= false;
        if(string.IsNullOrEmpty(assemblyQualifiedName)) return null;
        var t= Type.GetType(assemblyQualifiedName);
        if(t != null) return t;
        // Attempt to find type in different assembly.
        string typeName= null;
        string assemblyName= null;
        GetAssemblyQualifiedNameComponents( assemblyQualifiedName, ref typeName, ref assemblyName );
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            var newType= assembly.GetType(typeName);
            if(newType != null) {
#if DEBUG
                // Don't give warning for known conversions.
                var newAssemblyName= newType.Assembly.FullName.Split(new char[]{','})[0];
                if(assemblyName != "iCanScriptEngine" && newAssemblyName != "iCanScriptEngine") {
                    Debug.LogWarning("iCanScript: Unable to locate type: "+typeName+" from assembly: "+assemblyName+" ... using assembly: "+newAssemblyName+" instead.");                        
                }
#endif
                // Correct assembly qualified name.
                conversionPerformed= true;
                return newType;
            }
        }
#if DEBUG
        Debug.LogWarning("iCanScript: Unable to locate type: "+typeName+" from assembly: "+assemblyName);
#endif
        return null;        
    }
}
