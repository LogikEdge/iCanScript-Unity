using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_Reflection {
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the AddChild method.
    public static MethodInfo GetAddChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(WD_EditorStrings.AddChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) return null;
        ParameterInfo[] parameters= methodInfo.GetParameters();
        if(parameters.Length != 1) return null;
        return methodInfo;
    }
    public static void InvokeAddChildIfExists(object parent, object child) {
        MethodInfo method= GetAddChildMethodInfo(parent);
        if(method == null) return;
        method.Invoke(parent, new object[1]{child});
    }
    
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the RemoveChild method.
    public static MethodInfo GetRemoveChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(WD_EditorStrings.RemoveChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) return null;
        ParameterInfo[] parameters= methodInfo.GetParameters();
        if(parameters.Length != 1) return null;
        return methodInfo;
    }
    public static void InvokeRemoveChildIfExists(object parent, object child) {
        MethodInfo method= GetRemoveChildMethodInfo(parent);
        if(method == null) return;
        method.Invoke(parent, new object[1]{child});
    }
    
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the IsValid method.
    public static MethodInfo GetIsValidMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod("get_IsValid",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if(methodInfo == null) return null;
        ParameterInfo[] parameters= methodInfo.GetParameters();
        if(parameters.Length != 0) return null;
        if(methodInfo.ReturnType != typeof(bool)) return null;
        return methodInfo;
    }
    public static bool InvokeIsValid(object obj) {
        MethodInfo method= GetIsValidMethodInfo(obj);
        if(method == null) return true;
        return (bool)method.Invoke(obj, null);
    }
    
    // ----------------------------------------------------------------------
    // Returns the list of defined input fields.
    public static List<FieldInfo> GetInPortFields(Type objType) {
        List<FieldInfo> list= new List<FieldInfo>();
        foreach(var field in objType.GetFields()) {
            foreach(var attribute in field.GetCustomAttributes(true)) {
                if((attribute is WD_InPortAttribute)) {
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
                if((attribute is WD_OutPortAttribute)) {
                    list.Add(field);
                }
            }
        }        
        return list;
    }

    // ----------------------------------------------------------------------
    // Returns the type of the given port.
    public static Type GetPortFieldType(WD_EditorObject port, WD_EditorObject portParent) {
        FieldInfo fieldInfo= portParent.RuntimeType.GetField(port.Name);
        if(fieldInfo == null) {
            Debug.LogWarning("Invalid port name");            
        }
        return fieldInfo.FieldType;
    }


    // ----------------------------------------------------------------------
    // Scan the application for WarpDrive attributes.
    public static void ParseAppDomain() {
        // Remove all previously registered functions.
        WD_DataBase.Clear();
        // Scan the application for functions/methods/conversions to register.
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var classType in assembly.GetTypes()) {
                foreach(var classCustomAttribute in classType.GetCustomAttributes(true)) {
                    // Only register classes that have been tagged for WarpDrive.
                    if(classCustomAttribute is WD_ClassAttribute) {
                        // Validate that the class is public.
                        if(classType.IsPublic == false) {
                            Debug.LogWarning("Class "+classType+" is not public and tagged for "+WD_EditorConfig.ProductName+".  Ignoring class !!!");
                            continue;
                        }
                        // Extract class information.
                        WD_ClassAttribute classAttribute= classCustomAttribute as WD_ClassAttribute;
                        string classCompany= classAttribute.Company ?? "MyComnpany";
                        string className   = classAttribute.Name    ?? classType.Name;
                        string classPackage= classAttribute.Package ?? className;
                        string classToolTip= classAttribute.ToolTip;
                        string classIcon   = classAttribute.Icon;
                        DecodeClassInfo(classType, classCompany, classPackage, className, classToolTip, classIcon);
                    }
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeClassInfo(Type classType, string company, string package, string className, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        DecodeClassFields(classType, company, package, className, classToolTip, classIconPath, acceptAllPublic);
        DecodeFunctionsAndMethods(classType, company, package, className, classToolTip, classIconPath, acceptAllPublic);
    }
    // ----------------------------------------------------------------------
    static void DecodeClassFields(Type classType, string company, string package, string className, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        // Gather field information.
        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerField= false;
            WD_ParamDirectionEnum direction= WD_ParamDirectionEnum.InOut;
            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                if(!field.IsPublic && (fieldAttr is WD_InPortAttribute || fieldAttr is WD_OutPortAttribute)) {
                    Debug.LogWarning("Field "+field.Name+" of class "+classType.Name+" is not public and tagged for "+WD_EditorConfig.ProductName+". Ignoring field !!!");
                    continue;
                }
                if(fieldAttr is WD_InPortAttribute) {
                    direction= WD_ParamDirectionEnum.In;
                    registerField= true;
                }
                if(fieldAttr is WD_OutPortAttribute) {
                    direction= WD_ParamDirectionEnum.Out;
                    registerField= true;
                }
            }
            if(acceptAllPublic && field.IsPublic) {
                registerField= true;
                direction= WD_ParamDirectionEnum.InOut;
            }
            if(registerField) {
                if(field.IsStatic) {
                    DecodeStaticField(company, package, className, classToolTip, classIconPath, classType, field, direction);
                } else {
                    DecodeInstanceField(company, package, className, classToolTip, classIconPath, classType, field, direction);
                }                
            }
        }        
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticField(string company, string package, string displayName, string toolTip, string iconPath, Type classType, FieldInfo field, WD_ParamDirectionEnum dir) {
        WD_DataBase.AddStaticField(company, package, displayName, toolTip, iconPath, classType, field, dir);
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceField(string company, string package, string displayName, string toolTip, string iconPath, Type classType, FieldInfo field, WD_ParamDirectionEnum dir) {
        WD_DataBase.AddInstanceField(company, package, displayName, toolTip, iconPath, classType, field, dir);        
    }
    // ----------------------------------------------------------------------
    static void DecodeFunctionsAndMethods(Type classType, string company, string package, string className, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerMethod= false;
            string displayName= method.Name;
            string returnName= "out";
            string toolTip= classToolTip;
            string iconPath= classIconPath;
            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                if(methodAttribute is WD_ConversionAttribute) {
                    if(method.IsPublic) {
                        WD_ConversionAttribute convAttr= methodAttribute as WD_ConversionAttribute;
                        iconPath= convAttr.Icon ?? classIconPath;
                        DecodeConversion(company, package, iconPath, classType, method);
                    } else {                        
                        Debug.LogWarning("Conversion "+method.Name+" of class "+classType.Name+" is not public and tagged for "+WD_EditorConfig.ProductName+". Ignoring function !!!");
                    }
                    break;
                }
                else if(methodAttribute is WD_FunctionAttribute) {                                    
                    if(method.IsPublic) {
                        registerMethod= true;
                        // Register execution functions/methods.
                        WD_FunctionAttribute funcAttr= methodAttribute as WD_FunctionAttribute;
                        if(funcAttr.Name    != null) displayName= funcAttr.Name; 
                        if(funcAttr.Return  != null) returnName = funcAttr.Return;
                        if(funcAttr.ToolTip != null) toolTip    = funcAttr.ToolTip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("Function "+method.Name+" of class "+classType.Name+" is not public and tagged for "+WD_EditorConfig.ProductName+". Ignoring function !!!");                        
                    }
                    break;                                        
                }
            }
            if(acceptAllPublic && method.IsPublic) {
                registerMethod= true;
            }
            if(registerMethod) {
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
            Debug.LogWarning("Conversion function must have one return type and one parameter. Ignoring conversion function in "+classType.Name);
            return;
        }
        Type fromType= parameters[0].ParameterType;
        if(method.IsPublic == false) {
            Debug.LogWarning("Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                             " is not public and tagged for "+WD_EditorConfig.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        if(method.IsStatic == false) {
            Debug.LogWarning("Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                             " is not static and tagged for "+WD_EditorConfig.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        WD_DataBase.AddConversion(company, package, iconPath, classType, method, fromType, toType);                                        
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse return type.
        Type retType= method.ReturnType;
        if(retType == typeof(void)) {
            retType= null;
            retName= null;
        }
        // Parse parameters.
        string[] paramNames   = ParseParameterNames(method);
        Type[]   paramTypes   = ParseParameterTypes(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        WD_DataBase.AddInstanceMethod(company, package, displayName, toolTip, iconPath,
                                      classType, method,
                                      paramIsOut, paramNames, paramTypes, paramDefaults,
                                      retName, retType);
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse return type.
        Type retType= method.ReturnType;
        if(retType == typeof(void)) {
            retType= null;
            retName= null;
        }
        // Parse parameters.
        string[] paramNames   = ParseParameterNames(method);
        Type[]   paramTypes   = ParseParameterTypes(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        WD_DataBase.AddStaticMethod(company, package, displayName, toolTip, iconPath,
                                    classType, method,
                                    paramIsOut, paramNames, paramTypes, paramDefaults,
                                    retName, retType);
    }
    // ----------------------------------------------------------------------
    static string[] ParseParameterNames(MethodInfo method) {
        ParameterInfo[] parameters= method.GetParameters();
        string[] paramNames= new string[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramNames[i]= parameters[i].Name;
        }
        return paramNames;
    }
    // ----------------------------------------------------------------------
    static Type[] ParseParameterTypes(MethodInfo method) {
        ParameterInfo[] parameters= method.GetParameters();
        Type[]   paramTypes= new Type[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramTypes[i]= parameters[i].ParameterType;
        }        
        return paramTypes;
    }
    // ----------------------------------------------------------------------
    static bool[] ParseParameterIsOuts(MethodInfo method) {
        ParameterInfo[] parameters= method.GetParameters();
        bool[]   paramIsOuts= new bool[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramIsOuts[i]= parameters[i].IsOut;
        }
        return paramIsOuts;
    }
    // ----------------------------------------------------------------------
    static object[] ParseParameterDefaults(MethodInfo method) {
        ParameterInfo[] parameters= method.GetParameters();
        object[]   paramDefaults= new object[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            object defaultValue= parameters[i].DefaultValue; 
            paramDefaults[i]= (defaultValue == null || defaultValue.GetType() != parameters[i].ParameterType) ? null : defaultValue;
        }        
        return paramDefaults;
    }
}
