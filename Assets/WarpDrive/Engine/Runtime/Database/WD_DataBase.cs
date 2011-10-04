using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class WD_DataBase {
    // ======================================================================
    // Storage types.
    // ----------------------------------------------------------------------
    public class ConversionDesc {
        public string      Company;
        public string      Package;
        public MethodInfo  Method;
        public Type        FromType;
        public Type        ToType;
        public ConversionDesc(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) {
            Company= company;
            Package= package;
            Method= methodInfo;
            FromType= fromType;
            ToType= toType;
        }
    }
    public class FunctionDesc {
        public string       Company;
        public string       Package;
        public Type         ClassType;
        public string       MethodName;
        public string       ToolTip;
        public MethodInfo   Method;
        public FunctionDesc(string company, string package, Type classType, string methodName, string toolTip, MethodInfo methodInfo) {
            Company= company;
            Package= package;
            ClassType = classType;
            MethodName= methodName;
            ToolTip   = toolTip;
            Method    = methodInfo;
        }
    }
    public class ClassDesc {
        public string       Company;
        public string       Package;
        public Type         ClassType;
        public string[]     FieldNames;
        public Type[]       FieldTypes;
        public bool[]       FieldInOuts;
        public string[]     PropertyNames;
        public Type[]       PropertyTypes;
        public bool[]       PropertyInOuts;
        public string[]     MethodNames;
        public string[]     ReturnNames;
        public Type[]       ReturnTypes;
        public string[]     ToolTips;
        public string[][]   ParameterNames;
        public Type[][]     ParameterTypes;
        public bool[][]     ParameterInOuts;
        public ClassDesc(string company, string package, Type classType,
                         string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,
                         string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,
                         string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,
                         string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) {
            Company= company;
            Package= package;
            ClassType= classType;
            FieldNames= fieldNames;
            FieldTypes= fieldTypes;
            FieldInOuts= fieldInOuts;
            PropertyNames= propertyNames;
            PropertyTypes= propertyTypes;
            PropertyInOuts= propertyInOuts;
            MethodNames= methodNames;
            ReturnNames= returnNames;
            ReturnTypes= returnTypes;
            ToolTips= toolTips;
            ParameterNames= parameterNames;
            ParameterTypes= parameterTypes;
            ParameterInOuts= parameterInOuts;
        }
    }
    
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    public static List<ConversionDesc>  Conversions= new List<ConversionDesc>();
    public static List<FunctionDesc>    Functions  = new List<FunctionDesc>();
    public static List<ClassDesc>       Classes    = new List<ClassDesc>();
    
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
        Conversions.Add(new ConversionDesc(company, package, methodInfo, fromType, toType));
    }
    // ----------------------------------------------------------------------
    // Adds an execution function (no context).
    public static void AddFunction(string company, string package, Type classType,         // Class info
                                   string methodName,                                      // Function info
                                   string[] paramName, Type[] paramType, bool[] paramInOut,// Parameters info
                                   string retName, Type retType,                           // Return value info
                                   string toolTip, MethodInfo methodInfo) {
//        Debug.Log("Adding function: "+methodName+" from type: "+classType);
        Functions.Add(new FunctionDesc(company, package, classType, methodName, toolTip, methodInfo));
    }
    // ----------------------------------------------------------------------
    // Adds a class.
    public static void AddClass(string company, string package, Type classType,                                     // Class info
                                string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,                         // Field info
                                string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,                // Property info
                                string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,  // Method info
                                string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) {     // Method parameter info
//        Debug.Log("Adding class: "+classType.Name);       
        Classes.Add(new ClassDesc(company, package, classType,
                                  fieldNames, fieldTypes, fieldInOuts,
                                  propertyNames, propertyTypes, propertyInOuts,
                                  methodNames, returnNames, returnTypes, toolTips,
                                  parameterNames, parameterTypes, parameterInOuts));    

//        foreach(var p in propertyNames) {
//            Debug.Log("Property name: "+p);
//        }
    }
    // ----------------------------------------------------------------------
    // Create an instance of a conversion function.
    public object CreateInstance(ConversionDesc convDesc) {
        return null;
    }
    // ----------------------------------------------------------------------
    // Create an instance of a function (no context).
    public object CreateInstance(FunctionDesc funcDesc) {
        return null;
    }
    // ----------------------------------------------------------------------
    // Create an instance of a class.
    public object CreateInstance(ClassDesc classDesc) {
        return null;
    }
}
