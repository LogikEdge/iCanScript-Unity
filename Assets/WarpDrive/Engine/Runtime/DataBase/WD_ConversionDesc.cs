using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ConversionDesc : WD_BaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public MethodInfo  Method;
    public Type        FromType;
    public Type        ToType;

    // ======================================================================
    // Initialization
    // ----------------------------------------------------------------------
    public WD_ConversionDesc(string company, string package, Type classType, MethodInfo methodInfo, Type fromType, Type toType)
    : base(company, package, fromType.Name+"->"+toType.Name, "Converts from "+fromType.Name+" to "+toType.Name, classType) {
        Method= methodInfo;
        FromType= fromType;
        ToType= toType;
    }
}
