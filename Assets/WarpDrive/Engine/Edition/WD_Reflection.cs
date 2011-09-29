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
                    if((attribute is WD_ClassAttribute)) {
                        foreach(var method in type.GetMethods()) {
                            // Register execution functions/methods.
                            if(method.Name == "Evaluate") {
                                if(method.IsStatic) {
                                    Debug.Log("Found an evaluation function...");                                    
                                } else {
                                    Debug.Log("Found an evaluation method...");                                                                        
                                }
                            }
                            // Also register any conversion functions.
                            else if(method.Name == "Conversion") {
                                if(!method.IsStatic) {
                                    Debug.LogWarning("Found a non-static conversion method. Please declare the conversion function in "+type.Name+" as static.");
                                }
                                Type toType= method.ReturnType;
                                ParameterInfo[] parameters= method.GetParameters();
                                if(parameters.Length != 1 || toType == null) {
                                    Debug.LogWarning("Conversion function must have one return type and one parameter. Ignoring conversion function in "+type.Name);
                                    continue;
                                }
                                Type fromType= parameters[0].ParameterType;
                                WD_FunctionDataBase.AddConversion(method, fromType, toType);                                
                            }
                        }                       
                    }
                }
            }
        }
    }
}
