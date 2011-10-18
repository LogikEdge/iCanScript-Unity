using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_FunctionDesc : WD_BaseDesc {
    public string       ReturnName;
    public Type         ReturnType;
    public string[]     ParameterNames;
    public Type[]       ParameterTypes;
    public bool[]       ParameterInOuts;
    public MethodInfo   Method;
    public WD_FunctionDesc(string company, string package, string classToolTip, Type classType,
                           string methodName, string returnName, Type returnType, string toolTip, string icon,
                           string[] paramNames, Type[] paramTypes, bool[] paramInOuts,
                           MethodInfo methodInfo) : base(company, package, methodName, toolTip ?? classToolTip, icon, classType) {
        ClassType      = classType;
        ReturnName     = returnName;
        ReturnType     = returnType;
        ParameterNames = paramNames;
        ParameterTypes = paramTypes;
        ParameterInOuts= paramInOuts;
        Method         = methodInfo;
    }
}
