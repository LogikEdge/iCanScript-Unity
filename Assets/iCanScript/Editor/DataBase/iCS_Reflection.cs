using UnityEngine;
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
    static void AddToAllTypes(Type type) {
        if(type == null || type.Name.Length <= 0) return;
//        Debug.Log("Add type: "+type.Name);
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
        // Remove all previously registered functions.
        iCS_DataBase.Clear();
        // Scan the application for functions/methods/conversions to register.
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var classType in assembly.GetTypes()) {
                AddToAllTypes(classType);
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
                        string classCompany= classAttribute.Company;
                        string classPackage= classAttribute.Package ?? classType.Name;
                        string classToolTip= classAttribute.ToolTip;
                        string classIcon   = classAttribute.Icon;
                        DecodeClassInfo(classType, classCompany, classPackage, classToolTip, classIcon);
                    }
                }
            }
        }
//        iCS_UnityClasses.PopulateDataBase();
//        iCS_NETClasses.PopulateDataBase();
        NeedToRunInstaller= true;
        AllTypesWithDefaultConstructor.Sort((t1,t2)=>{ return String.Compare(t1.Name, t2.Name); });
    }
    // ----------------------------------------------------------------------
    public static void DecodeClassInfo(Type classType, string company, string package, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        if(classType.IsGenericType) {
            Debug.LogWarning("iCanScript: Generic class not supported yet.  Skiping: "+classType.Name);
            return;
        }
        DecodeConstructors(classType, company, package, classToolTip, classIconPath, acceptAllPublic);
        DecodeClassFields(classType, company, package, classToolTip, classIconPath, acceptAllPublic);
        DecodeFunctionsAndMethods(classType, company, package, classToolTip, classIconPath, acceptAllPublic);
    }
    // ----------------------------------------------------------------------
    static void DecodeClassFields(Type classType, string company, string package, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        // Gather field information.
        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerField= false;
            iCS_ParamDirectionEnum direction= iCS_ParamDirectionEnum.InOut;
            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                if(!field.IsPublic && (fieldAttr is iCS_InPortAttribute || fieldAttr is iCS_OutPortAttribute || fieldAttr is iCS_InOutPortAttribute)) {
                    Debug.LogWarning("iCanScript: Field "+field.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring field !!!");
                    continue;
                }
                if(fieldAttr is iCS_InPortAttribute) {
                    direction= iCS_ParamDirectionEnum.In;
                    registerField= true;
                }
                if(fieldAttr is iCS_OutPortAttribute) {
                    direction= iCS_ParamDirectionEnum.Out;
                    registerField= true;
                }
                if(fieldAttr is iCS_InOutPortAttribute) {
                    direction= iCS_ParamDirectionEnum.InOut;
                    registerField= true;
                }
            }
            if(acceptAllPublic && field.IsPublic) {
                registerField= true;
                direction= iCS_ParamDirectionEnum.InOut;
            }
            if(registerField) {
                if(field.IsStatic) {
                    DecodeStaticField(company, package, classToolTip, classIconPath, classType, field, direction);
                } else {
                    DecodeInstanceField(company, package, classToolTip, classIconPath, classType, field, direction);
                }                
            }
        }        
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticField(string company, string package, string toolTip, string iconPath, Type classType, FieldInfo field, iCS_ParamDirectionEnum dir) {
        object[] paramDefaultValues= new object[1]{iCS_Types.DefaultValue(field.FieldType)};
        if((dir == iCS_ParamDirectionEnum.In || dir == iCS_ParamDirectionEnum.InOut) && !field.IsInitOnly) {
            string[] paramNames = new string[1]{field.Name};
            bool[]   paramIsOuts= new bool[1]{false};
            Type[]   paramTypes = new Type[1]{field.FieldType};
            iCS_DataBase.AddStaticField(company, package, "set_"+field.Name, toolTip, iconPath, classType, field, paramIsOuts, paramNames, paramTypes, paramDefaultValues, null);                    
        }
        if(dir == iCS_ParamDirectionEnum.Out || dir == iCS_ParamDirectionEnum.InOut) {
            string[] paramNames = new string[0];
            bool[]   paramIsOuts= new bool[0];
            Type[]   paramTypes = new Type[0];
            iCS_DataBase.AddStaticField(company, package, "get_"+field.Name, toolTip, iconPath, classType, field, paramIsOuts, paramNames, paramTypes, paramDefaultValues, field.Name);                    
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceField(string company, string package, string toolTip, string iconPath, Type classType, FieldInfo field, iCS_ParamDirectionEnum dir) {
        object[] paramDefaultValues= new object[3]{iCS_Types.DefaultValue(classType), iCS_Types.DefaultValue(field.FieldType),iCS_Types.DefaultValue(classType)};
        if((dir == iCS_ParamDirectionEnum.In || dir == iCS_ParamDirectionEnum.InOut) && !field.IsInitOnly) {
            string[] paramNames = new string[1]{field.Name};
            bool[]   paramIsOuts= new bool[1]{false};
            Type[]   paramTypes = new Type[1]{field.FieldType};
            iCS_DataBase.AddInstanceField(company, package, "set_"+field.Name, toolTip, iconPath, classType, field, paramIsOuts, paramNames, paramTypes, paramDefaultValues, null);                    
        }
        if(dir == iCS_ParamDirectionEnum.Out || dir == iCS_ParamDirectionEnum.InOut) {
            string[] paramNames = new string[0];
            bool[]   paramIsOuts= new bool[0];
            Type[]   paramTypes = new Type[0];
            iCS_DataBase.AddInstanceField(company, package, "get_"+field.Name, toolTip, iconPath, classType, field, paramIsOuts, paramNames, paramTypes, paramDefaultValues, field.Name);                    
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeConstructors(Type classType, string company, string package, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        foreach(var constructor in classType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) {
            bool registerMethod= false;
            string displayName= classType.Name;
            string returnName= "";
            string toolTip= classToolTip;
            string iconPath= classIconPath;
            foreach(var constructorAttribute in constructor.GetCustomAttributes(true)) {
                if(constructorAttribute is iCS_FunctionAttribute) {                                    
                    if(constructor.IsPublic) {
                        registerMethod= true;
                        // Register execution functions/methods.
                        iCS_FunctionAttribute funcAttr= constructorAttribute as iCS_FunctionAttribute;
                        if(funcAttr.Name    != null) displayName= funcAttr.Name; 
                        if(funcAttr.Return  != null) returnName = funcAttr.Return;
                        if(funcAttr.ToolTip != null) toolTip    = funcAttr.ToolTip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("iCanScript: Constrcutor of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring constructor !!!");                        
                    }
                    break;                                        
                }
            }
            if(acceptAllPublic && constructor.IsPublic) {
                registerMethod= true;
            }
            if(registerMethod) {
                if(constructor.IsGenericMethod) {
                    Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping constrcutor from class "+classType.Name);
                    continue;
                }
                DecodeConstructor(company, package, displayName, toolTip, iconPath, classType, constructor, returnName);
            }
        }                               
    }
    // ----------------------------------------------------------------------
    static void DecodeConstructor(string company, string package, string displayName, string toolTip, string iconPath, Type classType, ConstructorInfo constructor, string retName) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(constructor)) return;
        Type[]   paramTypes   = ParseParameterTypes(constructor);
        string[] paramNames   = ParseParameterNames(constructor);
        bool[]   paramIsOut   = ParseParameterIsOuts(constructor);
        object[] paramDefaults= ParseParameterDefaults(constructor);

        iCS_DataBase.AddConstructor(company, package, displayName, toolTip, iconPath,
                                    classType, constructor,
                                    paramIsOut, paramNames, paramTypes, paramDefaults);
    }
    // ----------------------------------------------------------------------
    static void DecodeFunctionsAndMethods(Type classType, string company, string package, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerMethod= false;
            string displayName= method.Name;
            string returnName= null;
            string toolTip= classToolTip;
            string iconPath= classIconPath;
            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                if(methodAttribute is iCS_ConversionAttribute) {
                    if(method.IsPublic) {
                        iCS_ConversionAttribute convAttr= methodAttribute as iCS_ConversionAttribute;
                        iconPath= convAttr.Icon ?? classIconPath;
                        DecodeConversion(company, package, iconPath, classType, method);
                    } else {                        
                        Debug.LogWarning("iCanScript: Conversion "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");
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
                        if(funcAttr.ToolTip != null) toolTip    = funcAttr.ToolTip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("iCanScript: Function "+method.Name+" of class "+classType.Name+" is not public and tagged for "+iCS_Config.ProductName+". Ignoring function !!!");                        
                    }
                    break;                                        
                }
            }
            if(acceptAllPublic && method.IsPublic) {
                registerMethod= true;
            }
            if(registerMethod) {
                if(method.IsGenericMethod) {
//                    Debug.LogWarning("iCanScript: Generic method not yet supported.  Skiping "+method.Name+" from class "+className);
                    continue;
                }
                if(method.IsStatic) {
                    DecodeStaticMethod(company, package, displayName, toolTip, iconPath, classType, method, returnName);
                } else {
                    DecodeInstanceMethod(company, package, displayName, toolTip, iconPath, classType, method, returnName);
                }
            }
        }                               
    }
    // ----------------------------------------------------------------------
    static void DecodeConversion(string company, string package, string iconPath, Type classType, MethodInfo method) {
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
        iCS_DataBase.AddConversion(company, package, iconPath, classType, method, fromType);                                        
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(method)) return;        
        Type[]   paramTypes   = ParseParameterTypes(method);
        string[] paramNames   = ParseParameterNames(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        iCS_DataBase.AddInstanceMethod(company, package, displayName, toolTip, iconPath,
                                      classType, method,
                                      paramIsOut, paramNames, paramTypes, paramDefaults,
                                      retName);
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse parameters.
        if(!AreAllParamTypesSupported(method)) return;
        Type[]   paramTypes   = ParseParameterTypes(method);
        string[] paramNames   = ParseParameterNames(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        iCS_DataBase.AddStaticMethod(company, package, displayName, toolTip, iconPath,
                                     classType, method,
                                     paramIsOut, paramNames, paramTypes, paramDefaults,
                                     retName);
    }
    // ----------------------------------------------------------------------
    static string[] ParseParameterNames(MethodBase method) {
        ParameterInfo[] parameters= method.GetParameters();
        string[] paramNames= new string[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramNames[i]= parameters[i].Name;
        }
        return paramNames;
    }
    // ----------------------------------------------------------------------
    static Type[] ParseParameterTypes(MethodBase method) {
        ParameterInfo[] parameters= method.GetParameters();
        Type[]   paramTypes= new Type[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramTypes[i]= parameters[i].ParameterType;
        }        
        return paramTypes;
    }
    // ----------------------------------------------------------------------
    static bool[] ParseParameterIsOuts(MethodBase method) {
        ParameterInfo[] parameters= method.GetParameters();
        bool[]   paramIsOuts= new bool[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramIsOuts[i]= parameters[i].IsOut;
        }
        return paramIsOuts;
    }
    // ----------------------------------------------------------------------
    static object[] ParseParameterDefaults(MethodBase method) {
        ParameterInfo[] parameters= method.GetParameters();
        object[]   paramDefaults= new object[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            object defaultValue= parameters[i].DefaultValue; 
            paramDefaults[i]= (defaultValue == null || defaultValue.GetType() != parameters[i].ParameterType) ? null : defaultValue;
        }        
        return paramDefaults;
    }
    // ----------------------------------------------------------------------
    static bool AreAllParamTypesSupported(MethodBase method) {
        foreach(var paramInfo in method.GetParameters()) {
            if(paramInfo.ParameterType.IsPointer) return false;
        }
        return true;
    }
}
