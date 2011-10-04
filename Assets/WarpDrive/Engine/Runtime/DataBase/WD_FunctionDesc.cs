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
    public MethodInfo   Method;
    public WD_FunctionDesc(string company, string package, Type classType, string methodName, string toolTip, MethodInfo methodInfo) {
        Company= company;
        Package= package;
        ClassType = classType;
        MethodName= methodName;
        ToolTip   = toolTip;
        Method    = methodInfo;
    }
}
