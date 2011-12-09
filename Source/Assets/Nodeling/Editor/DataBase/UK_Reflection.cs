using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class UK_Reflection {
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the AddChild method.
    public static MethodInfo GetAddChildMethodInfo(object obj) {
        if(obj == null) return null;
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod(UK_EditorStrings.AddChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
        MethodInfo methodInfo= objType.GetMethod(UK_EditorStrings.RemoveChildMethod,BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
    // Returns the list of defined input fields.
    public static List<FieldInfo> GetInPortFields(Type objType) {
        List<FieldInfo> list= new List<FieldInfo>();
        foreach(var field in objType.GetFields()) {
            foreach(var attribute in field.GetCustomAttributes(true)) {
                if((attribute is UK_InPortAttribute)) {
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
                if((attribute is UK_OutPortAttribute)) {
                    list.Add(field);
                }
            }
        }        
        return list;
    }

    // ----------------------------------------------------------------------
    // Returns the type of the given port.
    public static Type GetPortFieldType(UK_EditorObject port, UK_EditorObject portParent) {
        FieldInfo fieldInfo= portParent.RuntimeType.GetField(port.Name);
        if(fieldInfo == null) {
            Debug.LogWarning("Invalid port name");            
        }
        return fieldInfo.FieldType;
    }

    // ----------------------------------------------------------------------
    // Scan the application for uCode attributes.
    public static void ParseAppDomain() {
        // Remove all previously registered functions.
        UK_DataBase.Clear();
        // Scan the application for functions/methods/conversions to register.
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var classType in assembly.GetTypes()) {
                foreach(var classCustomAttribute in classType.GetCustomAttributes(true)) {
                    // Only register classes that have been tagged for uCode.
                    if(classCustomAttribute is UK_ClassAttribute) {
                        // Validate that the class is public.
                        if(classType.IsPublic == false) {
                            Debug.LogWarning("Class "+classType+" is not public and tagged for "+UK_EditorConfig.ProductName+".  Ignoring class !!!");
                            continue;
                        }
                        // Extract class information.
                        UK_ClassAttribute classAttribute= classCustomAttribute as UK_ClassAttribute;
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
        UK_UnityClasses.PopulateDataBase();
        UK_NETClasses.PopulateDataBase();
    }
    // ----------------------------------------------------------------------
    public static void DecodeClassInfo(Type classType, string company, string package, string className, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        DecodeClassFields(classType, company, package, className, classToolTip, classIconPath, acceptAllPublic);
        DecodeFunctionsAndMethods(classType, company, package, className, classToolTip, classIconPath, acceptAllPublic);
    }
    // ----------------------------------------------------------------------
    static void DecodeClassFields(Type classType, string company, string package, string className, string classToolTip, string classIconPath, bool acceptAllPublic= false) {
        // Gather field information.
        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
            bool registerField= false;
            UK_ParamDirectionEnum direction= UK_ParamDirectionEnum.InOut;
            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                if(!field.IsPublic && (fieldAttr is UK_InPortAttribute || fieldAttr is UK_OutPortAttribute || fieldAttr is UK_InOutPortAttribute)) {
                    Debug.LogWarning("Field "+field.Name+" of class "+classType.Name+" is not public and tagged for "+UK_EditorConfig.ProductName+". Ignoring field !!!");
                    continue;
                }
                if(fieldAttr is UK_InPortAttribute) {
                    direction= UK_ParamDirectionEnum.In;
                    registerField= true;
                }
                if(fieldAttr is UK_OutPortAttribute) {
                    direction= UK_ParamDirectionEnum.Out;
                    registerField= true;
                }
                if(fieldAttr is UK_InOutPortAttribute) {
                    direction= UK_ParamDirectionEnum.InOut;
                    registerField= true;
                }
            }
            if(acceptAllPublic && field.IsPublic) {
                registerField= true;
                direction= UK_ParamDirectionEnum.InOut;
            }
            if(registerField) {
                if(field.IsStatic) {
                    DecodeStaticField(company, package, field.Name, classToolTip, classIconPath, classType, field, direction);
                } else {
                    DecodeInstanceField(company, package, field.Name, classToolTip, classIconPath, classType, field, direction);
                }                
            }
        }        
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticField(string company, string package, string displayName, string toolTip, string iconPath, Type classType, FieldInfo field, UK_ParamDirectionEnum dir) {
        string[] paramNames= new string[1]{field.Name};
        Type[] paramTypes= new Type[1]{field.FieldType};
        object[] paramDefaultValues= new object[1]{UK_Types.DefaultValue(field.FieldType)};
        if(dir == UK_ParamDirectionEnum.In || dir == UK_ParamDirectionEnum.InOut) {
            bool[] paramIsOuts= new bool[1]{false};
            UK_DataBase.AddStaticField(company, package, displayName+" (write)", toolTip, iconPath, classType, paramIsOuts, paramNames, paramTypes, paramDefaultValues);                    
        }
        if(dir == UK_ParamDirectionEnum.Out || dir == UK_ParamDirectionEnum.InOut) {
            bool[] paramIsOuts= new bool[]{true};
            UK_DataBase.AddStaticField(company, package, displayName+" (read)", toolTip, iconPath, classType, paramIsOuts, paramNames, paramTypes, paramDefaultValues);                    
        }
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceField(string company, string package, string displayName, string toolTip, string iconPath, Type classType, FieldInfo field, UK_ParamDirectionEnum dir) {
        string[] paramNames= new string[3]{"this", field.Name, "this"};
        Type[] paramTypes= new Type[3]{classType, field.FieldType, classType};
        object[] paramDefaultValues= new object[3]{UK_Types.DefaultValue(classType), UK_Types.DefaultValue(field.FieldType),UK_Types.DefaultValue(classType)};
        if(dir == UK_ParamDirectionEnum.In || dir == UK_ParamDirectionEnum.InOut) {
            bool[] paramIsOuts= new bool[3]{false, false, true};
            UK_DataBase.AddInstanceField(company, package, displayName+" (write)", toolTip, iconPath, classType, paramIsOuts, paramNames, paramTypes, paramDefaultValues);                    
        }
        if(dir == UK_ParamDirectionEnum.Out || dir == UK_ParamDirectionEnum.InOut) {
            bool[] paramIsOuts= new bool[3]{false, true, true};
            UK_DataBase.AddInstanceField(company, package, displayName+" (read)", toolTip, iconPath, classType, paramIsOuts, paramNames, paramTypes, paramDefaultValues);                    
        }
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
                if(methodAttribute is UK_ConversionAttribute) {
                    if(method.IsPublic) {
                        UK_ConversionAttribute convAttr= methodAttribute as UK_ConversionAttribute;
                        iconPath= convAttr.Icon ?? classIconPath;
                        DecodeConversion(company, package, iconPath, classType, method);
                    } else {                        
                        Debug.LogWarning("Conversion "+method.Name+" of class "+classType.Name+" is not public and tagged for "+UK_EditorConfig.ProductName+". Ignoring function !!!");
                    }
                    break;
                }
                else if(methodAttribute is UK_FunctionAttribute) {                                    
                    if(method.IsPublic) {
                        registerMethod= true;
                        // Register execution functions/methods.
                        UK_FunctionAttribute funcAttr= methodAttribute as UK_FunctionAttribute;
                        if(funcAttr.Name    != null) displayName= funcAttr.Name; 
                        if(funcAttr.Return  != null) returnName = funcAttr.Return;
                        if(funcAttr.ToolTip != null) toolTip    = funcAttr.ToolTip;
                        if(funcAttr.Icon    != null) iconPath   = funcAttr.Icon;
                    } else {
                        Debug.LogWarning("Function "+method.Name+" of class "+classType.Name+" is not public and tagged for "+UK_EditorConfig.ProductName+". Ignoring function !!!");                        
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
                             " is not public and tagged for "+UK_EditorConfig.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        if(method.IsStatic == false) {
            Debug.LogWarning("Conversion from "+fromType+" to "+toType+" in class "+classType.Name+
                             " is not static and tagged for "+UK_EditorConfig.ProductName+". Ignoring conversion !!!");
            return;                                        
        }
        UK_DataBase.AddConversion(company, package, iconPath, classType, method, fromType, toType);                                        
    }
    // ----------------------------------------------------------------------
    static void DecodeInstanceMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse return type.
        Type retType= method.ReturnType;
        if(retType == typeof(void)) {
            retName= "";
        }
        // Parse parameters.
        string[] paramNames   = ParseParameterNames(method);
        Type[]   paramTypes   = ParseParameterTypes(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        UK_DataBase.AddInstanceMethod(company, package, displayName, toolTip, iconPath,
                                      classType, method,
                                      paramIsOut, paramNames, paramTypes, paramDefaults,
                                      retName, retType);
    }
    // ----------------------------------------------------------------------
    static void DecodeStaticMethod(string company, string package, string displayName, string toolTip, string iconPath, Type classType, MethodInfo method, string retName) {
        // Parse return type.
        Type retType= method.ReturnType;
        if(retType == typeof(void)) {
            retName= "";
        }
        // Parse parameters.
        string[] paramNames   = ParseParameterNames(method);
        Type[]   paramTypes   = ParseParameterTypes(method);
        bool[]   paramIsOut   = ParseParameterIsOuts(method);
        object[] paramDefaults= ParseParameterDefaults(method);

        UK_DataBase.AddStaticMethod(company, package, displayName, toolTip, iconPath,
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
