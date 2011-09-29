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
        public MethodInfo Method;
        public FunctionDesc(MethodInfo methodInfo) {
            Method= methodInfo;
        }
    }
    public class MethodDesc {
        public MethodInfo Method;
        public MethodDesc(MethodInfo methodInfo) {
            Method= methodInfo;
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
        Debug.Log("Add a conversion from "+fromType+" to "+toType);
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddExecutionFunction(MethodInfo methodInfo) {
        Debug.Log("Adding an evaluation function");
    }
    // ----------------------------------------------------------------------
    // Adds an execution method which requires a context (class properties).
    public static void AddExecutionMethod(MethodInfo methodInfo) {
        Debug.Log("Adding an evaluation method");
    }
}
