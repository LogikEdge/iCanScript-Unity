using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ReflectionFuncDesc : WD_ReflectionBaseDesc {
    public string       ReturnName;
    public Type         ReturnType;
    public string[]     ParameterNames;
    public Type[]       ParameterTypes;
    public bool[]       ParameterIsOuts;
    public object[]     ParameterDefaults;
    public MethodInfo   Method;
    public WD_ReflectionFuncDesc(string company, string package, string classToolTip, Type classType,
                           string methodName, string returnName, Type returnType, string toolTip, string icon,
                           string[] paramNames, Type[] paramTypes, bool[] paramIsOuts, object[] paramDefaults,
                           MethodInfo methodInfo) : base(company, package, methodName, toolTip ?? classToolTip, icon, classType) {
        ClassType        = classType;
        ReturnName       = returnName;
        ReturnType       = returnType;
        ParameterNames   = paramNames;
        ParameterTypes   = paramTypes;
        ParameterIsOuts  = paramIsOuts;
        ParameterDefaults= paramDefaults;
        Method           = methodInfo;
    }
}
