using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

public class WD_ClassDesc : WD_BaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public Type         ClassType;
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
    public WD_ClassDesc(string company, string package, Type classType,
                        string[] fieldNames, Type[] fieldTypes, bool[] fieldInOuts,
                        string[] propertyNames, Type[] propertyTypes, bool[] propertyInOuts,
                        MethodInfo[] methodInfos, string[] methodNames, string[] returnNames, Type[] returnTypes, string[] toolTips,
                        string[][] parameterNames, Type[][] parameterTypes, bool[][] parameterInOuts) : base (company, package) {
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
    // ----------------------------------------------------------------------
    public object CreateRuntimeClass() {
        return Inf.IsA(ClassType, typeof(ScriptableObject)) ?
            ScriptableObject.CreateInstance(ClassType) :
            Activator.CreateInstance(ClassType);            
    }
    // ----------------------------------------------------------------------
    public object CreateEditorObjects(string name, WD_EditorObject parent, object rtObj) {
        return null;
    }
    
}
