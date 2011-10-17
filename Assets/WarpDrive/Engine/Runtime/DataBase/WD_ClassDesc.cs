using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

public class WD_ClassDesc : WD_BaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string[]     FieldNames;
    public Type[]       FieldTypes;
    public bool[]       FieldInOuts;
    public string[]     PropertyNames;
    public Type[]       PropertyTypes;
    public bool[]       PropertyInOuts;
    public MethodInfo[] MethodInfos;
    public string[]     MethodNames;
    public string[]     ReturnNames;
    public Type[]       ReturnTypes;
    public string[]     ToolTips;
    public string[][]   ParameterNames;
    public Type[][]     ParameterTypes;
    public bool[][]     ParameterInOuts;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_ClassDesc(string company, string package, string className, string toolTip, Type classType,
                        string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,
                        string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,
                        MethodInfo[] methodInfos, string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,
                        string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) : base (company, package, className, toolTip, classType) {
        ClassType= classType;
        FieldNames= fieldNames;
        FieldTypes= fieldTypes;
        FieldInOuts= fieldInOuts;
        PropertyNames= propertyNames;
        PropertyTypes= propertyTypes;
        PropertyInOuts= propertyInOuts;
        MethodInfos= methodInfos;
        MethodNames= methodNames;
        ReturnNames= returnNames;
        ReturnTypes= returnTypes;
        ToolTips= toolTips;
        ParameterNames= parameterNames;
        ParameterTypes= parameterTypes;
        ParameterInOuts= parameterInOuts;
    }
}
