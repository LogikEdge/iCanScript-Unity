using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ReflectionDesc {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string           Company;
    public string           Package;
    public string           ToolTip;
    public string           IconPath;
    public iCS_RuntimeDesc  RuntimeDesc;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_ObjectTypeEnum ObjectType  { get { return RuntimeDesc.ObjectType; }}
    public Type               ClassType   { get { return RuntimeDesc.ClassType; }}
    public string             DisplayName { get { return RuntimeDesc.DisplayName; }}
    public string[]           ParamNames  { get { return RuntimeDesc.ParamNames; }}
    public string             ReturnName  { get { return RuntimeDesc.ReturnName; }}
    public Type               ReturnType  { get { return RuntimeDesc.ReturnType; }}

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ReflectionDesc(string company, string package, string name,
                              string toolTip, string iconPath,
                              iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase,
                              bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                              string returnName, Type returnType) {
        // Editor object information.
        Company    = company;
        Package    = package;
        ToolTip    = toolTip;
        IconPath   = iconPath;

        // Fill-in runtime details.
        RuntimeDesc= new iCS_RuntimeDesc(objType, company, package, name, classType, methodBase != null ? methodBase.Name : null,
                                         paramNames, paramTypes, paramIsOuts, paramDefaultValues,
                                         returnName, returnType);
    }

    // ======================================================================
    // Archiving
    // ----------------------------------------------------------------------
    public string Encode(int id) {
        return RuntimeDesc.Encode(id);
    }
}
