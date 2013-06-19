using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class iCS_Reflection {
    // ======================================================================
    // Fileds.
    // ----------------------------------------------------------------------
    public static List<Type>    AllTypesWithDefaultConstructor= new List<Type>();
    public static bool          NeedToRunInstaller= false;
    
    // ----------------------------------------------------------------------
    /*
        TODO: Should remove all types if not used.
    */
    static void AddToAllTypes(Type type) {
        if(type == null || type.Name.Length <= 0) return;
        if(type.Name[0] != '<' && iCS_Types.CreateInstanceSupported(type)) {
            AllTypesWithDefaultConstructor.Add(type);
        }
    }
    // ----------------------------------------------------------------------
    public static Type[] GetAllTypesWithDefaultConstructorThatDeriveFrom(Type baseType) {
        List<Type> result= new List<Type>();
        foreach(var type in AllTypesWithDefaultConstructor) {
            if(iCS_Types.IsA(baseType, type)) result.Add(type);
        }
        return result.ToArray();
    }
    
    // ----------------------------------------------------------------------
    // Returns the list of defined input fields.
    public static List<FieldInfo> GetInPortFields(Type objType) {
        List<FieldInfo> list= new List<FieldInfo>();
        foreach(var field in objType.GetFields()) {
            foreach(var attribute in field.GetCustomAttributes(true)) {
                if((attribute is iCS_InPortAttribute)) {
                    list.Add(field);
                }
            }
        }        
        return list;
    }
    // ----------------------------------------------------------------------
    // Returns the list of defined output fields.
    public static List<FieldInfo> GetOutPortFields(Type objType) {
        List<FieldInfo> list= new List<FieldInfo>();
        foreach(var field in objType.GetFields()) {
            foreach(var attribute in field.GetCustomAttributes(true)) {
                if((attribute is iCS_OutPortAttribute)) {
                    list.Add(field);
                }
            }
        }        
        return list;
    }

    // ----------------------------------------------------------------------
    // Returns the type of the given port.
    public static Type GetPortFieldType(iCS_EditorObject port, iCS_EditorObject portParent) {
        FieldInfo fieldInfo= portParent.RuntimeType.GetField(port.Name);
        if(fieldInfo == null) {
            Debug.LogWarning("iCanScript: Invalid port name");            
        }
        return fieldInfo.FieldType;
    }

    // ----------------------------------------------------------------------
    // Scan the application for uCode attributes.
    public static void ParseAppDomain() {
        // Type used for user installs
        Type installerType= null;
        // Remove all previously registered functions.
        iCS_LibraryDataBase.Clear();
        // Scan the application for functions/methods/conversions to register.
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var classType in assembly.GetTypes()) {
                AddToAllTypes(classType);
                if(classType.Name == "iCS_Installer") {
                    installerType= classType;
                    continue;
                }
                foreach(var classCustomAttribute in classType.GetCustomAttributes(true)) {
                    // Only register classes that have been tagged for uCode.
                    if(classCustomAttribute is iCS_ClassAttribute) {
                        // Validate that the class is public.
                        if(classType.IsPublic == false) {
                            Debug.LogWarning("iCanScript: Class "+classType+" is not public and tagged for "+iCS_Config.ProductName+".  Ignoring class !!!");
                            continue;
                        }
                        // Extract class information.
                        iCS_ClassAttribute classAttribute= classCustomAttribute as iCS_ClassAttribute;
                        string  classCompany       = classAttribute.Company;
                        string  classPackage       = classAttribute.Package;
                        string  classDescription   = classAttribute.Tooltip;
                        string  classIcon          = classAttribute.Icon;
                        bool    classBaseVisibility= classAttribute.BaseVisibility;
                        DecodeClassInfo(classType, classCompany, classPackage, classDescription, classIcon, false, classBaseVisibility);
                    }
                }
            }
        }
        // Invoke user installation.
        NeedToRunInstaller= true;
        // Run user installer
        if(installerType != null) {
            var installMethod= installerType.GetMethod("Install");
            if(installMethod != null) {
                installMethod.Invoke(null, null);
            }
        }
        AllTypesWithDefaultConstructor.Sort((t1,t2)=>{ return String.Compare(t1.Name, t2.Name); });
    }
    // ----------------------------------------------------------------------
    public static void DecodeClassInfo(Type classType, string company, string package, string classDescription, string classIconPath,
                                       bool acceptAllPublic= false,
                                       bool baseVisibility= false) {
        if(classType.IsGenericType) {
            Debug.LogWarning("iCanScript: Generic class not supported yet.  Skiping: "+classType.Name);
            return;
        }
        // Build TypeInfo.
        var _classTypeInfo= iCS_LibraryDataBase.AddTypeInfo(company, package, classType, null, classType.Name, classDescription, classIconPath);
        // Decode class members
        DecodeConstructors(_classTypeInfo, acceptAllPublic);
        DecodeClassFields(_classTypeInfo, acceptAllPublic, baseVisibility);
        DecodeFunctionsAndMethods(_classTypeInfo, acceptAllPublic, baseVisibility);
    }
	// ======================================================================
	// Decode Constructors
    // ----------------------------------------------------------------------
    static void DecodeConstructors(iCS_TypeInfo _classTypeInfo, bool acceptAllPublic= false) {
        foreach(var constructor in _classTypeInfo.compilerType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            bool registerMethod= false;
            string displayName= iCS_Types.TypeName(_classTypeInfo.compilerType);
            string description= "";
            string iconPath= "";
            foreach(var constructorAttribute in constructor.GetCustomAttributes(true)) {
                if(constructorAttribute is iCS_FunctionAttribute) {                                    
                    if(constructor.IsPublic) {
                        registerMethod= true;
                        // Register execution functions/methods.
                        iCS_FunctionAttribute funcAttr= constructorAttribute as iCS_FunctionAttribute;
                        if(funcAttr.Name    != null) displayName= funcAttr.Name; 
                        if(funcAttr.Tooltip != null) description= funcAttr.Tooltip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("iCanScript: Constrcutor of class "+_classTypeInfo.displayName+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring constructor !!!");                        
                    }
                    break;                                        
                }
            }
            if(acceptAllPublic && constructor.IsPublic) {
                registerMethod= true;
            }
            if(registerMethod) {
                if(constructor.IsGenericMethod) {
                    Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping constrcutor from class "+_classTypeInfo.displayName);
                    continue;
                }
                DecodeConstructor(_classTypeInfo, displayName, description, iconPath, constructor);
            }
        }                               
    }
    // ----------------------------------------------------------------------
    static void DecodeConstructor(iCS_TypeInfo _classTypeInfo, string displayName, string description, string iconPath, ConstructorInfo constructor) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(constructor)) return;
        var parameters= ParseParameters(constructor);
        
        iCS_LibraryDataBase.AddConstructor(_classTypeInfo, displayName, description, iconPath,
                                    	   parameters, constructor);
    }
    // ----------------------------------------------------------------------
    static void DecodeClassFields(iCS_TypeInfo _classTypeInfo,
                                  bool acceptAllPublic= false,
                                  bool baseVisibility= false) {
        // Gather field information.
        var classType= _classTypeInfo.compilerType;
        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerField= false;
            iCS_ParamDirection direction= iCS_ParamDirection.InOut;
            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                if(!field.IsPublic && (fieldAttr is iCS_InPortAttribute || fieldAttr is iCS_OutPortAttribute || fieldAttr is iCS_InOutPortAttribute)) {
                    Debug.LogWarning("iCanScript: Field "+field.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring field !!!");
                    continue;
                }
                if(fieldAttr is iCS_InPortAttribute) {
                    direction= iCS_ParamDirection.In;
                    registerField= true;
                }
                if(fieldAttr is iCS_OutPortAttribute) {
                    direction= iCS_ParamDirection.Out;
                    registerField= true;
                }
                if(fieldAttr is iCS_InOutPortAttribute) {
                    direction= iCS_ParamDirection.InOut;
                    registerField= true;
                }
            }
            if(registerField == false && field.IsPublic) {
                if(acceptAllPublic) {
                    registerField= true;
                    direction= iCS_ParamDirection.InOut;
                } else if(baseVisibility) {
                    var declaringType= field.DeclaringType;
                    if(declaringType != classType) {
                        // Don't override iCS attributes
                        bool isTagged= false;
                        foreach(var baseAttr in declaringType.GetCustomAttributes(true)) {
                            if(baseAttr is iCS_ClassAttribute) {
                                isTagged= true;
                                break;
                            }
                        }
                        // Add base fields if class is not using iCS attributes.
                        if(isTagged == false) {
                            registerField= true;
                            direction= iCS_ParamDirection.InOut;                            
                        }
                    }
                }                
            }
            if(registerField) {
                if(field.IsStatic) {
                    DecodeStaticField(_classTypeInfo, field, direction);
                } else {
                    DecodeInstanceField(_classTypeInfo, field, direction);
                }                
            }
        }        
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticField(iCS_TypeInfo _classTypeInfo, FieldInfo field, iCS_ParamDirection dir) {
        string description= "";
        string iconPath= _classTypeInfo.iconPath;
        if((dir == iCS_ParamDirection.In || dir == iCS_ParamDirection.InOut) && !field.IsInitOnly) {
            var parameters            = new iCS_Parameter[1];
            parameters[0]             = new iCS_Parameter();
            parameters[0].name        = field.Name;
            parameters[0].type        = field.FieldType;
            parameters[0].direction   = iCS_ParamDirection.In;
            parameters[0].initialValue= iCS_Types.DefaultValue(field.FieldType);
            iCS_LibraryDataBase.AddField(_classTypeInfo, "set_"+field.Name, description, iconPath,
                                         parameters, null,
                                         iCS_StorageClass.Class, iCS_AccessorType.Set, field);                    
        }
        if(dir == iCS_ParamDirection.Out || dir == iCS_ParamDirection.InOut) {
            var returnType= new iCS_FunctionReturn(field.Name, field.FieldType);
            iCS_LibraryDataBase.AddField(_classTypeInfo, "get_"+field.Name, description, iconPath,
                                         new iCS_Parameter[0], returnType,
                                         iCS_StorageClass.Class, iCS_AccessorType.Get, field);                    
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceField(iCS_TypeInfo _classTypeInfo, FieldInfo field, iCS_ParamDirection dir) {
        string description= "";
        string iconPath= _classTypeInfo.iconPath;
        if((dir == iCS_ParamDirection.In || dir == iCS_ParamDirection.InOut) && !field.IsInitOnly) {
            var parameters        = new iCS_Parameter[1];
            parameters[0]         = new iCS_Parameter();
            parameters[0].name        = field.Name;
            parameters[0].type        = field.FieldType;
            parameters[0].direction   = iCS_ParamDirection.In;
            parameters[0].initialValue= iCS_Types.DefaultValue(field.FieldType);
            iCS_LibraryDataBase.AddField(_classTypeInfo, "set_"+field.Name, description, iconPath,
                                         parameters, null,
                                         iCS_StorageClass.Instance, iCS_AccessorType.Set, field);                    
        }
        if(dir == iCS_ParamDirection.Out || dir == iCS_ParamDirection.InOut) {
            var returnType= new iCS_FunctionReturn(field.Name, field.FieldType);
            iCS_LibraryDataBase.AddField(_classTypeInfo, "get_"+field.Name, description, iconPath,
                                         new iCS_Parameter[0], returnType,
                                         iCS_StorageClass.Instance, iCS_AccessorType.Get, field);                    
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeFunctionsAndMethods(iCS_TypeInfo _classTypeInfo,
                                          bool acceptAllPublic= false,
                                          bool baseVisibility= false) {
        var classType= _classTypeInfo.compilerType;
        foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerMethod= false;
            string displayName= method.Name;
            string returnName= null;
            string classDescription= _classTypeInfo.description;
            string classIconPath= _classTypeInfo.iconPath;
            string description= classDescription;
            string iconPath= classIconPath;
            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                if(methodAttribute is iCS_TypeCastAttribute) {
                    if(method.IsPublic) {
                        iCS_TypeCastAttribute typeCastAttr= methodAttribute as iCS_TypeCastAttribute;
                        iconPath= typeCastAttr.Icon ?? classIconPath;
                        DecodeTypeCast(_classTypeInfo, iconPath, method);
                    } else {                        
                        Debug.LogWarning("iCanScript: Type Cast "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");
                    }
                    break;
                }
                else if(methodAttribute is iCS_FunctionAttribute) {                                    
                    if(method.IsPublic) {
                        registerMethod= true;
                        // Register execution functions/methods.
                        iCS_FunctionAttribute funcAttr= methodAttribute as iCS_FunctionAttribute;
                        if(funcAttr.Name    != null) displayName= funcAttr.Name; 
                        if(funcAttr.Return  != null) returnName = funcAttr.Return;
                        if(funcAttr.Tooltip != null) description= funcAttr.Tooltip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("iCanScript: Function "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");                        
                    }
                    break;                                        
                }
            }
            if(registerMethod == false && method.IsPublic) {
                if(acceptAllPublic) {
                    registerMethod= true;
                } else if(baseVisibility) {
                    var declaringType= method.DeclaringType;
                    if(declaringType != classType) {
                        // Don't override iCS attributes
                        bool isTagged= false;
                        foreach(var baseAttr in declaringType.GetCustomAttributes(true)) {
                            if(baseAttr is iCS_ClassAttribute) {
                                isTagged= true;
                                break;
                            }
                        }
                        // Add base fields if class is not using iCS attributes.
                        if(isTagged == false) {
                            registerMethod= true;
                        }
                    }
                }
            }
            if(registerMethod) {
                if(method.IsGenericMethod) {
//                    Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping "+method.Name+" from class "+className);
                    continue;
                }
                if(method.IsStatic) {
                    DecodeStaticMethod(_classTypeInfo, displayName, description, iconPath, returnName, method);
                } else {
                    DecodeInstanceMethod(_classTypeInfo, displayName, description, iconPath, returnName, method);
                }
            }
        }                               
    }
    // ----------------------------------------------------------------------
    static void DecodeTypeCast(iCS_TypeInfo _classTypeInfo, string iconPath, MethodInfo method) {
        var classType= _classTypeInfo.compilerType;
        Type toType= method.ReturnType;
        ParameterInfo[] parameters= method.GetParameters();
        if(parameters.Length != 1 || toType == null) {
            Debug.LogWarning("iCanScript: Conversion function must have one return type and one parameter. Ignoring conversion function in "+classType.Name);
            return;
        }
        Type fromType= parameters[0].ParameterType;
        if(method.IsPublic == false) {
            Debug.LogWarning("iCanScript: Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                             " is not public and tagged for "+iCS_Config.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        if(method.IsStatic == false) {
            Debug.LogWarning("iCanScript: Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                             " is not static and tagged for "+iCS_Config.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        iCS_LibraryDataBase.AddTypeCast(_classTypeInfo, iconPath, fromType, method);                                        
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceMethod(iCS_TypeInfo _classTypeInfo,
                                     string displayName, string description, string iconPath,
                                     string retName, MethodInfo method) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(method)) return; 
        var parameters= ParseParameters(method);
        var returnType= new iCS_FunctionReturn(retName, method.ReturnType);       

        // Register a property if the method has only one input or one output.
        bool isProperty= false;
        iCS_AccessorType accessor= iCS_AccessorType.None;
        if(parameters.Length == 0 && returnType.type != typeof(void) && displayName.StartsWith("get_")) {
            isProperty= true;
            accessor= iCS_AccessorType.Get;
        }
        if(parameters.Length == 1 && returnType.type == typeof(void) && displayName.StartsWith("set_")) {
            isProperty= true;
            accessor= iCS_AccessorType.Set;
        }
        if(isProperty) {
            iCS_LibraryDataBase.AddProperty(_classTypeInfo,
                                            displayName, description, iconPath,
                                            parameters, returnType,
                                            iCS_StorageClass.Instance, accessor,
                                            method);
        } else {
//            if(displayName.StartsWith("get_") || displayName.StartsWith("set_")) {
//                Debug.Log("Method: "+displayName+" #p=> "+parameters.Length+" return=> "+returnType.type);
//            }
            iCS_LibraryDataBase.AddMethod(_classTypeInfo, displayName, description, iconPath,
                                          parameters, returnType,
                                          iCS_StorageClass.Instance, method);            
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticMethod(iCS_TypeInfo _classTypeInfo,
                                   string displayName, string description, string iconPath,
                                   string retName, MethodInfo method) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(method)) return;
        var parameters= ParseParameters(method);       
        var returnType= new iCS_FunctionReturn(retName, method.ReturnType);       

        // Register a property if the method has only one input or one output.
        bool isProperty= false;
        iCS_AccessorType accessor= iCS_AccessorType.None;
        if(parameters.Length == 0 && returnType.type != typeof(void) && displayName.StartsWith("get_")) {
            isProperty= true;
            accessor= iCS_AccessorType.Get;
        }
        if(parameters.Length == 1 && returnType.type == typeof(void) && displayName.StartsWith("set_")) {
            isProperty= true;
            accessor= iCS_AccessorType.Set;
        }
        if(isProperty) {
            iCS_LibraryDataBase.AddProperty(_classTypeInfo,
                                            displayName, description, iconPath,
                                            parameters, returnType,
                                            iCS_StorageClass.Class, accessor,
                                            method);            
        } else {
            // Register standard method.
//            if(displayName.StartsWith("get_") || displayName.StartsWith("set_")) {
//                Debug.Log("Function: "+displayName+" #p=> "+parameters.Length+" return=> "+returnType.type);
//            }
            iCS_LibraryDataBase.AddMethod(_classTypeInfo,
                                          displayName, description, iconPath,
                                          parameters, returnType,
                                          iCS_StorageClass.Class, method);
        }
    }

   
    // ======================================================================
    // ----------------------------------------------------------------------
    static iCS_Parameter[] ParseParameters(MethodBase method) {
        ParameterInfo[] paramInfo= method.GetParameters();
        iCS_Parameter[] parameters= new iCS_Parameter[paramInfo.Length];
        if(paramInfo.Length == 0) return parameters;
        for(int i= 0; i < paramInfo.Length; ++i) {
            var p= paramInfo[i];
            parameters[i]= new iCS_Parameter();
            parameters[i].name= p.Name;
            parameters[i].type= p.ParameterType;
            // IMPROVE: Mono seems broken for ParameterInfo.IsIn. So we use !IsOut as IsIn. That prevents InOut parameters...
            parameters[i].direction= p.IsOut ? (p.IsIn ? iCS_ParamDirection.InOut : iCS_ParamDirection.Out) : iCS_ParamDirection.In;
            object defaultValue= p.DefaultValue; 
            parameters[i].initialValue= (defaultValue == null || defaultValue.GetType() != p.ParameterType) ? null : defaultValue;
        }
        return parameters;   
    }
    // ----------------------------------------------------------------------
    static bool AreAllParamTypesSupported(MethodBase method) {
        foreach(var paramInfo in method.GetParameters()) {
            if(paramInfo.ParameterType.IsPointer) return false;
        }
        return true;
    }
}
