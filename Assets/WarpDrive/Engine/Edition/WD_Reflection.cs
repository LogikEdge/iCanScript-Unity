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
    // Scan all assemblies.
    public static void ParseAppDomain() {
        // Remove all previously registered functions.
        WD_FunctionDataBase.Clear();
        // Scan the application for functions/methods/conversions to register.
        foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach(var type in assembly.GetTypes()) {
                foreach(var attribute in type.GetCustomAttributes(true)) {
                    // Only register classes that have been tagged for WarpDrive.
                    if(attribute is WD_ClassAttribute) {
                        foreach(var method in type.GetMethods()) {
                            foreach(var methodAttribute in method.GetCustomAttributes(true)) {
                                if(methodAttribute is WD_FunctionAttribute) {
                                    // Register execution functions/methods.
                                    string methodName= (methodAttribute as WD_FunctionAttribute).Name   ?? method.Name;
                                    string retName   = (methodAttribute as WD_FunctionAttribute).Return ?? "out";
                                    ParseFunction(methodName, type, retName, method);
                                    break;
                                }
                                else if(methodAttribute is WD_ConversionAttribute) {
                                    // Register conversion functions.
                                    ParseConversion(type, method);
                                }
                            }
                        }                       
                    }
                }
            }
        }
    }
    static void ParseConversion(Type classType, MethodInfo method) {
        if(!method.IsStatic) {
            Debug.LogWarning("Found a non-static conversion method. Please declare the conversion function in "+classType.Name+" as static.");
            return;
        }
        Type toType= method.ReturnType;
        ParameterInfo[] parameters= method.GetParameters();
        if(parameters.Length != 1 || toType == null) {
            Debug.LogWarning("Conversion function must have one return type and one parameter. Ignoring conversion function in "+classType.Name);
            return;
        }
        Type fromType= parameters[0].ParameterType;
        WD_FunctionDataBase.AddConversion(method, fromType, toType);                                        
    }
    static void ParseFunction(string methodName, Type classType, string retName, MethodInfo method) {
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
            WD_FunctionDataBase.AddExecutionFunction(methodName, paramNames, paramTypes, paramInOut, retName, retType, method);
        } else {
            WD_FunctionDataBase.AddExecutionMethod(methodName, classType, paramNames, paramTypes, paramInOut, retName, retType, method);
        }        
    }
}
