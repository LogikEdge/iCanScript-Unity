using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_FunctionDataBase {
    // ======================================================================
    // Storage types.
    // ----------------------------------------------------------------------
    public class ConversionDesc {
        public MethodInfo  Method;
        public Type        FromType;
        public Type        ToType;
        public ConversionDesc(MethodInfo methodInfo, Type fromType, Type toType) {
            Method= methodInfo;
            FromType= fromType;
            ToType= toType;
        }
    }
    public class FunctionDesc {
        public Type         ClassType;
        public string       MethodName;
        public string       ToolTip;
        public MethodInfo   Method;
        public FunctionDesc(Type classType, string methodName, string toolTip, MethodInfo methodInfo) {
            ClassType = classType;
            MethodName= methodName;
            ToolTip   = toolTip;
            Method    = methodInfo;
        }
    }
    public class MethodDesc {
        public Type         ClassType;
        public string       MethodName;
        public string       ToolTip;
        public MethodInfo   Method;
        public MethodDesc(Type classType, string methodName, string toolTip, MethodInfo methodInfo) {
            ClassType = classType;
            MethodName= methodName;
            ToolTip   = toolTip;
            Method    = methodInfo;
        }
    }

    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<ConversionDesc>  Conversions= new List<ConversionDesc>();
    public static List<FunctionDesc>    Functions  = new List<FunctionDesc>();
    public static List<MethodDesc>      Methods    = new List<MethodDesc>(); 
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Conversions.Clear();
        Functions.Clear();
        Methods.Clear();
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(MethodInfo methodInfo, Type fromType, Type toType) {
        foreach(var desc in Conversions) {
            if(desc.FromType == fromType && desc.ToType == toType) {
                Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                return;
            }
        }
        Conversions.Add(new ConversionDesc(methodInfo, fromType, toType));
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddExecutionFunction(Type classType, string methodName,                      // Function info
                                            string[] paramName, Type[] paramType, bool[] paramInOut,// Parameters info
                                            string retName, Type retType,                           // Return value info
                                            string toolTip, MethodInfo methodInfo) {
        Debug.Log("Adding function: "+methodName+" from type: "+classType);
        Functions.Add(new FunctionDesc(classType, methodName, toolTip, methodInfo));
    }
    // ----------------------------------------------------------------------
    // Adds an execution method which requires a context (class properties).
    public static void AddExecutionMethod(Type classType, string methodName,                        // Method info
                                          string[] paramName, Type[] paramType, bool[] paramInOut,  // Parameters info
                                          string retName, Type retType,                             // Return value info
                                          string toolTip, MethodInfo methodInfo) {
        Debug.Log("Adding method: "+methodName+" from type: "+classType);
        Methods.Add(new MethodDesc(classType, methodName, toolTip, methodInfo));
    }
}
