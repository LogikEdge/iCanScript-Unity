using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ConversionDesc {
    public string      Company;
    public string      Package;
    public MethodInfo  Method;
    public Type        FromType;
    public Type        ToType;
    public WD_ConversionDesc(string company, string package, MethodInfo methodInfo, Type fromType, Type toType) {
        Company= company;
        Package= package;
        Method= methodInfo;
        FromType= fromType;
        ToType= toType;
    }
}
