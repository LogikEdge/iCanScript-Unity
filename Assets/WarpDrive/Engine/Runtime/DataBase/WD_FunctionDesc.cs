using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_FunctionDesc {
    public string       Company;
    public string       Package;
    public Type         ClassType;
    public string       MethodName;
    public string       ToolTip;
    public string[]     ParameterNames;
    public Type[]       ParameterTypes;
    public bool[]       ParameterInOuts;
    public MethodInfo   Method;
    public WD_FunctionDesc(string company, string package, Type classType,
                           string methodName, string toolTip,
                           string[] paramNames, Type[] paramTypes, bool[] paramInOuts,
                           MethodInfo methodInfo) {
        Company= company;
        Package= package;
        ClassType = classType;
        MethodName= methodName;
        ToolTip   = toolTip;
        ParameterNames = paramNames;
        ParameterTypes = paramTypes;
        ParameterInOuts= paramInOuts;
        Method    = methodInfo;
    }
    public WD_RuntimeMethod CreateRuntime() {
        return WD_RuntimeMethod.CreateFunction(Method.Invoke, ParameterNames.Length);
    }
}
