//#define SHOW_DEBUG
using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using iCanScript.Internal.Engine;

namespace iCanScript.Internal {
    using P= Prelude;
    
    public static class iCS_Types {
        // ----------------------------------------------------------------------
        // Returns true if type can be converted to another type
        public static bool IsA(Type baseType, Type derivedType) {
    		if(baseType == null || derivedType == null) return false;
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
        // Returns true if type is a language primitive type.
        public static bool IsPrimitive(Type type) {
			return type.IsPrimitive;
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
        /// Returns the default value of the given type.
        ///
        /// @param type The type for which to get a default value.
        /// @return The default value for the given type.
        ///
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
        /// Returns the string representation for the given object.
        ///
        /// @param value The object to convert to a string.
        /// @return The C# string representation of the object.
        ///
        public static string ToString(System.Object value) {
            if(value is Color) {
                var c= (Color)value;
                return "Color("+c.r+"f, "+c.g+"f, "+c.b+"f, "+c.a+"f)";
            }
            return value.ToString();
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
            return type.Name;
        }
        // ----------------------------------------------------------------------
    	/// Finds the method information of a function with the given type name.
    	///
    	/// @param typeString The name of the type to search.
    	/// @param functionName The function name to be located.
    	/// @return The found method information if found. _null_ otherwise.
    	///
        public static MethodInfo FindFunction(string typeString, string functionName) {
            string namespaceName= null;
            string typeName     = null;
            SplitTypeString(typeString, out namespaceName, out typeName);
            return FindFunction(typeName, functionName, namespaceName);
        }
        // ----------------------------------------------------------------------
    	/// Finds the method information of a function of the given type name
    	/// within the given namespace.
    	///
    	/// @param typeName The name of the type to search.
    	/// @param functionName The function name to be located.
    	/// @param namespaceName The namespace that contains the type.
    	/// @return The found method information if found. _null_ otherwise.
    	///
        public static MethodInfo FindFunction(string typeName, string functionName, string namespaceName) {
            var interfaceType= iCS_Types.FindType(namespaceName, typeName);
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
        // ======================================================================
        // TYPE STRING UTILITIES
        // ----------------------------------------------------------------------
		/// Returns the type string associated with the given type.
		///
		/// @param type The type from which to generate the type string.
		/// @return The formatted type string.
		///
        public static string ToTypeString(Type type) {
            var namespaceName= type.Namespace;
            if(!string.IsNullOrEmpty(namespaceName)) {
                return namespaceName+"."+type.Name;
            }
            return type.Name;
        }
        // ----------------------------------------------------------------------
		// Extracts the namespace from the type string.
		///
		/// @param typeString The type string.
		/// @return The namespace of the type.
		///
		public static string GetNamespaceFromTypeString(string typeString) {
			string namespaceName;
			string typeName;
			SplitTypeString(typeString, out namespaceName, out typeName);
			return namespaceName;
		}
        // ----------------------------------------------------------------------
        /// Extract the namespace and type names from a type string.
        ///
        /// @param typeString The _'namespace.type'_ formatted string.
        /// @param namespaceName The extracted namespace name. _null_ if not found.
        /// @param typeName The extracted type name. _null_ if not found.
        /// @return _true_ if type string properly decoded. _false_ otherwise.
        ///
        public static bool SplitTypeString(string typeString,
                                           out string namespaceName,
                                           out string typeName) {
            typeName= null;
            namespaceName  = null;
            // -- Separate namespace name from type name --
            var len= typeString.Length;
            var splitIdx= typeString.LastIndexOf('.');
            if(splitIdx > 0 && splitIdx < len) {
                namespaceName= typeString.Substring(0, splitIdx);
                typeName= typeString.Substring(splitIdx+1, len-splitIdx-1);
            }
            else {
                typeName= typeString;
            }
            return !string.IsNullOrEmpty(typeName);
        }
        // ----------------------------------------------------------------------
        /// Extract the type information from a type string.
        ///
        /// @param typeString The _'namespace.type'_ formatted string.
        /// @param namespaceName The extracted namespace name. _null_ if not found.
        /// @param typeName The extracted type name. _null_ if not found.
        /// @return The type if found. _null_ otherwise.
        ///
        public static Type GetTypeFromTypeString(string typeString,
                                                 out string namespaceName,
                                                 out string typeName) {
            // -- Separate namespace name from type name --
            SplitTypeString(typeString, out namespaceName, out typeName);
            // -- Find the corresponding type --
            return FindType(namespaceName, typeName);
        }
        // ----------------------------------------------------------------------
        /// Extract the type information from a type string.
        ///
        /// @param typeString The _'namespace.type'_ formatted string.
        /// @return The type if found. _null_ otherwise.
        ///
        public static Type GetTypeFromTypeString(string typeString) {
            // -- Separate namespace name from type name --
            string namespaceName= null;
            string typeName     = null;
            SplitTypeString(typeString, out namespaceName, out typeName);
            // -- Find the corresponding type --
            return FindType(namespaceName, typeName);
        }
        // ----------------------------------------------------------------------
    	/// Seraches the application domain for the type with the given name in
    	/// the given namespace.
    	///
    	/// @param namespaceName The namespace that contains the type.
    	/// @param typeName The name of the type to search.
    	/// @return The found type descriptor if found. _null_ otherwise.
    	///
        public static Type FindType(string namespaceName, string typeName) {
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(var classType in assembly.GetTypes()) {
                    if(classType.Name == typeName && classType.Namespace == namespaceName) {
                        return classType;
                    }
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        /// Returns all types that is or derives from the given type.
        ///
        /// @param type The base type to start the search.
        /// @return The list of type that derive from the given type.
        ///
        public static Type[] GetAllTypesThatDeriveFrom(Type type) {
            var derivedTypes= new List<Type>();
            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach(var t in assembly.GetTypes()) {
                    if(iCS_Types.IsA(type, t)) {
                        derivedTypes.Add(t);
                    }
                }
            }
            return derivedTypes.ToArray();
        }
        
        // ======================================================================
        // ----------------------------------------------------------------------
        public static bool IsGeneratedByiCanScript(string namespaceName, string typeName) {
            var type= FindType(namespaceName, typeName);
            if(type == null || type == typeof(void)) return false;
            foreach(var attribute in type.GetCustomAttributes(true)) {
                if(attribute is iCS_ClassAttribute) return true;
            }
            return false;
        }
        // ----------------------------------------------------------------------
        public static string GetICanScriptFile(Type type) {
            foreach(var attribute in type.GetCustomAttributes(true)) {
                if(attribute is iCS_FileSpecAttribute) {
                    var fileSpec= attribute as iCS_FileSpecAttribute;
                    return fileSpec.iCanScriptFile;
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public static string GetICanScriptFileGUID(Type type) {
            foreach(var attribute in type.GetCustomAttributes(true)) {
                if(attribute is iCS_FileSpecAttribute) {
                    var fileSpec= attribute as iCS_FileSpecAttribute;
                    return fileSpec.iCanScriptFileGUID;
                }
            }
            return null;
        }
        // ----------------------------------------------------------------------
        public static MethodInfo[] GetAbstractMethods(Type type) {
            var methods= type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return P.filter(m=> m.IsAbstract, methods);
        }
    
        // ======================================================================
        // ASSEMBLY TYPE STRING UTILITIES
        // ----------------------------------------------------------------------
        public static Type TypeFromAssemblyQualifiedName(string assemblyQualifiedName) {
            bool dummy= false;
            return TypeFromAssemblyQualifiedName(assemblyQualifiedName, out dummy);
        }
        // ----------------------------------------------------------------------
        public static void GetAssemblyQualifiedNameComponents(string assemblyQualifiedName, ref string typeName, ref string assemblyName) {
    //		Debug.Log("Assemlby=> "+assemblyQualifiedName);
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
#if SHOW_DEBUG
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
#if SHOW_DEBUG
            Debug.LogWarning("iCanScript: Unable to locate type: "+typeName+" from assembly: "+assemblyName);
#endif
            return null;        
        }

    }

}
