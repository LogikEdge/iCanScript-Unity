using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ConversionDesc : WD_BaseDesc {
    public MethodInfo  Method;
    public Type        FromType;
    public Type        ToType;
    public WD_ConversionDesc(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) : base(company, package) {
        Method= methodInfo;
        FromType= fromType;
        ToType= toType;
    }
    public WD_RuntimeMethod CreateRuntime() {
        return WD_RuntimeMethod.CreateFunction(Method.Invoke, 1);
    }
}
