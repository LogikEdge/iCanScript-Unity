using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class WD_ReflectionFuncDesc : WD_ReflectionBaseDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string[]         ParamNames;
    public string           ReturnName;
    public WD_RuntimeDesc   RuntimeDesc;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_ReflectionFuncDesc(string company, string package, string name,
                                 string toolTip, string iconPath,
                                 WD_ObjectTypeEnum objType, Type classType, MethodInfo methodInfo,
                                 bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                                 string returnName, Type returnType) : base(company, package, name, toolTip, iconPath, classType) {
        // Fill-in editor names
        ParamNames= paramNames;
        ReturnName= returnName;
 
        // Fill-in runtime details.
        RuntimeDesc= new WD_RuntimeDesc();
        RuntimeDesc.ObjectType= objType;
        RuntimeDesc.ClassType= classType;
        RuntimeDesc.MethodName= methodInfo != null ? methodInfo.Name : null;
        RuntimeDesc.ParamIsOuts= paramIsOuts;
        RuntimeDesc.ParamTypes= paramTypes;
        RuntimeDesc.ParamDefaultValues= paramDefaultValues;
        RuntimeDesc.ReturnType= returnType;
        
        // To be obsoleted
        RuntimeDesc.Company   = company;
        RuntimeDesc.Package   = package;
        RuntimeDesc.ParamNames= paramNames;
        RuntimeDesc.ReturnName= returnName;
    }

    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    public override string Encode() {
        return RuntimeDesc.Encode();
    }
}
