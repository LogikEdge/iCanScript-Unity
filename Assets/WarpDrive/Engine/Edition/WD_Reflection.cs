using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_Reflection {
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
        WD_FunctionDataBase.Clear();
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
                           MethodInfo[] methodInfos, string[] methodNames, string[] methodReturnNames, string[] methodToolTips,
                           FieldInfo[] fieldInfos, bool[] fieldInOuts) {
        // Separate functions (static methods) from methods.
        List<int> methodIndexes= new List<int>();
        for(int i= 0; i < methodInfos.Length; ++i) {
            if(methodInfos[i].IsStatic) {
                ParseFunction(company, package, classType, methodNames[i], methodReturnNames[i], methodToolTips[i], methodInfos[i]);
            }
            else {
                methodIndexes.Add(i);
            }
        }
        // Class does not need to be registered if it does not have any methods to execute.
        if(methodIndexes.Count == 0) return;
        // Parse the parameters of each method.
        
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
        WD_FunctionDataBase.AddConversion(company, package, method, fromType, toType);                                        
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
        ParameterInfo[] parameters= method.GetParameters();
        bool[]   paramInOut= new bool[parameters.Length];
        string[] paramNames= new string[parameters.Length];
        Type[]   paramTypes= new Type[parameters.Length];
        for(int i= 0; i < parameters.Length; ++i) {
            paramInOut[i]= parameters[i].IsOut;
            paramNames[i]= parameters[i].Name;
            paramTypes[i]= parameters[i].ParameterType;
        }

        if(method.IsStatic) {
            WD_FunctionDataBase.AddFunction(company, package, classType, methodName, paramNames, paramTypes, paramInOut, retName, retType, toolTip, method);
        } else {
            WD_FunctionDataBase.AddMethod(company, package, classType, methodName, paramNames, paramTypes, paramInOut, retName, retType, toolTip, method);
        }        
    }
}
