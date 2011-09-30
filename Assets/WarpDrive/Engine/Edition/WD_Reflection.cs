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
                foreach(var attribute in classType.GetCustomAttributes(true)) {
                    // Only register classes that have been tagged for WarpDrive.
                    if(attribute is WD_ClassAttribute) {
                        // Validate that the class is public.
                        if(classType.IsPublic == false) {
                            Debug.LogWarning("Class "+classType+" is not public and tagged for WarpDrive.  Ignoring class !!!");
                            continue;
                        }
                        // Gather field information.
                        List<string> fieldNames= new List<string>();
                        List<Type>   fieldTypes= new List<Type>();
                        List<bool>   fieldInOut= new List<bool>();
                        foreach(var field in classType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                            foreach(var fieldAttr in field.GetCustomAttributes(true)) {
                                if(fieldAttr is WD_InPortAttribute || fieldAttr is WD_OutPortAttribute) {
                                    if(field.IsPublic == false) {
                                        Debug.LogWarning("Field "+field.Name+" of class "+classType.Name+" is not public and tagged for WarpDrive. Ignoring field !!!");
                                        continue;
                                    }
                                    fieldNames.Add(field.Name);
                                    fieldTypes.Add(field.FieldType);
                                    fieldInOut.Add(fieldAttr is WD_OutPortAttribute);
                                    Debug.Log("Field "+field.Name+" is found.");
                                }
                            }
                        }
                        // Parse functions and methods.
                        foreach(var method in classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)) {
                            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                                if(methodAttribute is WD_FunctionAttribute) {
                                    if(method.IsPublic == false) {
                                        Debug.LogWarning("Function "+method.Name+" of class "+classType.Name+" is not public and tagged for WarpDrive. Ignoring function !!!");
                                        continue;                                        
                                    }
                                    // Register execution functions/methods.
                                    string methodName= (methodAttribute as WD_FunctionAttribute).Name    ?? method.Name;
                                    string retName   = (methodAttribute as WD_FunctionAttribute).Return  ?? "out";
                                    string toolTip   = (methodAttribute as WD_FunctionAttribute).ToolTip ?? "No ToolTip";
                                    ParseFunction(classType, methodName, retName, toolTip, method);
                                    break;
                                }
                                else if(methodAttribute is WD_ConversionAttribute) {
                                    // Register conversion functions.
                                    ParseConversion(classType, method);
                                }
                            }
                        }                       
                    }
                }
            }
        }
    }
    static void ParseConversion(Type classType, MethodInfo method) {
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
        WD_FunctionDataBase.AddConversion(method, fromType, toType);                                        
    }
    static void ParseFunction(Type classType, string methodName, string retName, string toolTip, MethodInfo method) {
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
            WD_FunctionDataBase.AddExecutionFunction(classType, methodName, paramNames, paramTypes, paramInOut, retName, retType, toolTip, method);
        } else {
            WD_FunctionDataBase.AddExecutionMethod(classType, methodName, paramNames, paramTypes, paramInOut, retName, retType, toolTip, method);
        }        
    }
}
