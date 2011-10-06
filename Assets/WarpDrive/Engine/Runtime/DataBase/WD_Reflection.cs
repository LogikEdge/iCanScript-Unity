using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_Reflection {
    // ----------------------------------------------------------------------
    // Returns the MethodInfo associated with the AddChild method.
    public static MethodInfo GetAddChildMethodInfo(object obj) {
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod("AddChild",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
        Type objType= obj.GetType();
        MethodInfo methodInfo= objType.GetMethod("RemoveChild",BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
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
                            Debug.LogWarning("Class "+classType+" is not public and tagged for WarpDrive.  Ignoring class !!!");
                            continue;
                        }
                        // Extract class information.
                        WD_ClassAttribute classAttribute= classCustomAttribute as WD_ClassAttribute;
                        string classCompany= classAttribute.Company ?? "MyComnpany";
                        string classPackage= classAttribute.Package ?? "DefaultPackage";
                        // Gather field information.
                        List<FieldInfo> fieldInfos = new List<FieldInfo>();
                        List<bool>      fieldInOuts= new List<bool>();
                        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                                if(fieldAttr is WD_InPortAttribute || fieldAttr is WD_OutPortAttribute) {
                                    if(field.IsPublic == false) {
                                        Debug.LogWarning("Field "+field.Name+" of class "+classType.Name+" is not public and tagged for WarpDrive. Ignoring field !!!");
                                        continue;
                                    }
                                    fieldInfos.Add(field);
                                    fieldInOuts.Add(fieldAttr is WD_OutPortAttribute);
                                }
                            }
                        }
                        // Parse functions and methods.
                        List<string>     methodNames      = new List<string>();
                        List<string>     methodReturnNames= new List<string>();
                        List<string>     methodToolTips   = new List<string>();
                        List<MethodInfo> methodInfos      = new List<MethodInfo>();
                        foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                                if(methodAttribute is WD_ConversionAttribute) {
                                    ParseConversion(classCompany, classPackage, classType, method);
                                }
                                else if(methodAttribute is WD_FunctionAttribute) {                                    
                                    if(method.IsPublic == false) {
                                        Debug.LogWarning("Function "+method.Name+" of class "+classType.Name+" is not public and tagged for WarpDrive. Ignoring function !!!");
                                        continue;                                        
                                    }
                                    // Register execution functions/methods.
                                    WD_FunctionAttribute funcAttr= methodAttribute as WD_FunctionAttribute;
                                    methodInfos.Add(method);
                                    methodNames.Add(funcAttr.Name ?? method.Name);
                                    methodReturnNames.Add(funcAttr.Return ?? "out");
                                    methodToolTips.Add(funcAttr.ToolTip ?? "No ToolTip");
                                }
                            }
                        }                       
                        ParseClass(classCompany, classPackage, classType,
                                   methodInfos.ToArray(), methodNames.ToArray(), methodReturnNames.ToArray(), methodToolTips.ToArray(),
                                   fieldInfos.ToArray(), fieldInOuts.ToArray());
                    }
                    
                }
            }
        }
    }
    // ----------------------------------------------------------------------
    static void ParseClass(string company, string package, Type classType,
                           MethodInfo[] methodInfos, string[] methodNames, string[] returnNames, string[] toolTips,
                           FieldInfo[] fieldInfos, bool[] fieldInOuts) {
        // Extract field names & types.
        string[] fieldNames= new string[fieldInfos.Length];
        Type[]   fieldTypes= new Type[fieldInfos.Length];
        for(int i= 0; i < fieldInfos.Length; ++i) {
            fieldNames[i]= fieldInfos[i].Name;
            fieldTypes[i]= fieldInfos[i].FieldType;
        }
        // Separate functions (static methods) from methods.
        List<int> _methodIndexes= new List<int>();
        List<int> _propertyIndexes= new List<int>();
        List<Type> _propertyTypes= new List<Type>();
        List<bool> _propertyInOuts= new List<bool>();
        for(int i= 0; i < methodInfos.Length; ++i) {
            if(methodInfos[i].ReturnType == typeof(void) && methodInfos[i].GetParameters().Length == 1) {
                _propertyIndexes.Add(i);
                _propertyTypes.Add((methodInfos[i].GetParameters())[0].ParameterType);
                _propertyInOuts.Add(false);
            }
            else if(methodInfos[i].ReturnType != typeof(void) && methodInfos[i].GetParameters().Length == 0) {
                _propertyIndexes.Add(i);
                _propertyInOuts.Add(true);
            }
            else if(methodInfos[i].IsStatic) {
                ParseFunction(company, package, classType, methodNames[i], returnNames[i], toolTips[i], methodInfos[i]);
            }
            else {
                _methodIndexes.Add(i);
            }
        }
        // Class does not need to be registered if it does not have any methods to execute.
        if(_methodIndexes.Count == 0 && _propertyIndexes.Count == 0) return;
        // Build property information.
        Type[] propertyTypes= _propertyTypes.ToArray();
        bool[] propertyInOuts= _propertyInOuts.ToArray();
        string[] propertyNames= new string[_propertyIndexes.Count];
        for(int i= 0; i < _propertyIndexes.Count; ++i) {
            string name= methodInfos[_propertyIndexes[i]].Name;
            string tmp= name.ToUpper();
            if((_propertyInOuts[i] == true && tmp.StartsWith("GET")) || (_propertyInOuts[i] == false && tmp.StartsWith("SET"))) {
                if(name[3] == '_') {
                    name= name.Substring(4);
                }
                else if(Char.IsLower(name[2]) && Char.IsUpper(name[3])) {
                    name= name.Substring(3);                    
                }
            }
            propertyNames[i]= name;
        }
        // Rebuild the method info from the method indexes.
        List<MethodInfo> _methodInfos= new List<MethodInfo>();
        List<string> _methodNames= new List<string>();
        List<string> _returnNames= new List<string>();
        List<string> _toolTips= new List<string>();
        foreach(var i in _methodIndexes) {
            _methodInfos.Add(methodInfos[i]);
            _methodNames.Add(methodNames[i]);
            _returnNames.Add(returnNames[i]);
            _toolTips.Add(toolTips[i]);
        }
        methodInfos= _methodInfos.ToArray();
        methodNames= _methodNames.ToArray();
        returnNames= _returnNames.ToArray();
        toolTips   = _toolTips.ToArray();
        // Parse each method.
        Type[] returnType= new Type[methodInfos.Length];
        string[][] paramNames= new string[methodInfos.Length][];
        Type[][] paramTypes= new Type[methodInfos.Length][];
        bool[][] paramInOuts= new bool[methodInfos.Length][];
        for(int i= 0; i < methodInfos.Length; ++i) {
            // Return types.
            returnType[i]= methodInfos[i].ReturnType;
            if(returnType[i] == typeof(void)) {
                returnType[i] = null;
                returnNames[i]= null;
            }
            // Parameters.
            paramNames[i] = ParseParameterNames(methodInfos[i]);
            paramTypes[i] = ParseParameterTypes(methodInfos[i]);
            paramInOuts[i]= ParseParameterInOuts(methodInfos[i]);
        }        
        // Add to database.
        WD_DataBase.AddClass(company, package, classType,
                             fieldNames, fieldTypes, fieldInOuts,
                             propertyNames, propertyTypes, propertyInOuts,
                             methodInfos, methodNames, returnNames, returnType, toolTips,
                             paramNames, paramTypes, paramInOuts);
    }

    // ----------------------------------------------------------------------
    static void ParseConversion(string company, string package, Type classType, MethodInfo method) {
        Type toType= method.ReturnType;
        ParameterInfo[] parameters= method.GetParameters();
        if(parameters.Length != 1 || toType == null) {
            Debug.LogWarning("Conversion function must have one return type and one parameter. Ignoring conversion function in "+classType.Name);
            return;
        }
        Type fromType= parameters[0].ParameterType;
        if(method.IsPublic == false) {
            Debug.LogWarning("Conversion from "+fromType+" to "+toType+" in class "+classType.Name+" is not public and tagged for WarpDrive. Ignoring conversion !!!");
            return;                                        
        }
        if(method.IsStatic == false) {
            Debug.LogWarning("Conversion from "+fromType+" to "+toType+" in class "+classType.Name+" is not static and tagged for WarpDrive. Ignoring conversion !!!");
            return;                                        
        }
        WD_DataBase.AddConversion(company, package, method, fromType, toType);                                        
    }
    // ----------------------------------------------------------------------
    static void ParseFunction(string company, string package, Type classType, string methodName, string retName, string toolTip, MethodInfo method) {
        // Parse return type.
        Type retType= method.ReturnType;
        if(retType == typeof(void)) {
            retType= null;
            retName= null;
        }
        // Parse parameters.
        string[] paramNames= ParseParameterNames(method);
        Type[]   paramTypes= ParseParameterTypes(method);
        bool[]   paramInOut= ParseParameterInOuts(method);

        WD_DataBase.AddFunction(company, package, classType, methodName, paramNames, paramTypes, paramInOut, retName, retType, toolTip, method);
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
    static bool[] ParseParameterInOuts(MethodInfo method) {
        ParameterInfo[] parameters= method.GetParameters();
        bool[]   paramInOuts= new bool[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramInOuts[i]= parameters[i].IsOut;
        }
        return paramInOuts;
    }
}
