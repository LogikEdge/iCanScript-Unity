using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_DataBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<WD_ConversionDesc>  Conversions= new List<WD_ConversionDesc>();
    public static List<WD_FunctionDesc>    Functions  = new List<WD_FunctionDesc>();
    public static List<WD_ClassDesc>       Classes    = new List<WD_ClassDesc>();
    
    // ======================================================================
    // Container management functions
    // ----------------------------------------------------------------------
    // Removes all previously recorded functions.
    public static void Clear() {
        Conversions.Clear();
        Functions.Clear();
        Classes.Clear();
    }
    // ----------------------------------------------------------------------
    // Adds a conversion function
    public static void AddConversion(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) {
        foreach(var desc in Conversions) {
            if(desc.FromType == fromType && desc.ToType == toType) {
                Debug.LogWarning("Duplicate conversion function from "+fromType+" to "+toType+" exists in classes "+desc.Method.DeclaringType+" and "+methodInfo.DeclaringType);
                return;
            }
        }
//        Debug.Log("Adding conversion from "+fromType+" to "+toType);
        Conversions.Add(new WD_ConversionDesc(company, package, methodInfo, fromType, toType));
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddFunction(string company, string package, Type classType,         // Class info
                                   string methodName,                                      // Function info
                                   string[] paramName, Type[] paramType, bool[] paramInOut,// Parameters info
                                   string retName, Type retType,                           // Return value info
                                   string toolTip, MethodInfo methodInfo) {
//        Debug.Log("Adding function: "+methodName+" from type: "+classType);
        Functions.Add(new WD_FunctionDesc(company, package, classType, methodName, toolTip, methodInfo));
    }
    // ----------------------------------------------------------------------
    // Adds a class.
    public static void AddClass(string company, string package, Type classType,                                     // Class info
                                string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,                         // Field info
                                string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,                // Property info
                                string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,  // Method info
                                string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) {     // Method parameter info
//        Debug.Log("Adding class: "+classType.Name);       
        Classes.Add(new WD_ClassDesc(company, package, classType,
                                     fieldNames, fieldTypes, fieldInOuts,
                                     propertyNames, propertyTypes, propertyInOuts,
                                     methodNames, returnNames, returnTypes, toolTips,
                                     parameterNames, parameterTypes, parameterInOuts));    
    }
    // ----------------------------------------------------------------------
    // Create an instance of a conversion function.
    public object CreateInstance(WD_ConversionDesc convDesc) {
        return null;
    }
    // ----------------------------------------------------------------------
    // Create an instance of a function (no context).
    public object CreateInstance(WD_FunctionDesc funcDesc) {
        return null;
    }
    // ----------------------------------------------------------------------
    // Create an instance of a class.
    public object CreateInstance(WD_ClassDesc classDesc) {
        return null;
    }
}
