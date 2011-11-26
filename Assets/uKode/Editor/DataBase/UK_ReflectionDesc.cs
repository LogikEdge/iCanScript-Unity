using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class UK_ReflectionDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string           Company;
    public string           Package;
    public Type             ClassType;
    public string           DisplayName;
    public string           ToolTip;
    public string           IconPath;
    public string[]         ParamNames;
    public string           ReturnName;
    public UK_RuntimeDesc   RuntimeDesc;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public UK_ObjectTypeEnum ObjectType { get { return RuntimeDesc.ObjectType; } set { RuntimeDesc.ObjectType= value; }}
    public Type              ReturnType { get { return RuntimeDesc.ReturnType; } set { RuntimeDesc.ReturnType= value; }}
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public UK_ReflectionDesc(string company, string package, string name,
                             string toolTip, string iconPath,
                             UK_ObjectTypeEnum objType, Type classType, MethodInfo methodInfo,
                             bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                             string returnName, Type returnType) {
        // Editor object information.
        Company    = company;
        Package    = package;
        DisplayName= name;
        ToolTip    = toolTip;
        ClassType  = classType;
        IconPath   = iconPath;

        // Fill-in editor names
        ParamNames= paramNames;
        ReturnName= returnName;
 
        // Fill-in runtime details.
        RuntimeDesc= new UK_RuntimeDesc();
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
    public string Encode(int id) {
        return RuntimeDesc.Encode(id);
    }
}
