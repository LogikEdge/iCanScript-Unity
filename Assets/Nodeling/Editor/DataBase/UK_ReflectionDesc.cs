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
    public string           ToolTip;
    public string           IconPath;
    public UK_RuntimeDesc   RuntimeDesc;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public UK_ObjectTypeEnum ObjectType  { get { return RuntimeDesc.ObjectType; }}
    public Type              ClassType   { get { return RuntimeDesc.ClassType; }}
    public string            DisplayName { get { return RuntimeDesc.DisplayName; }}
    public string[]          ParamNames  { get { return RuntimeDesc.ParamNames; }}
    public string            ReturnName  { get { return RuntimeDesc.ReturnName; }}
    public Type              ReturnType  { get { return RuntimeDesc.ReturnType; }}

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
        ToolTip    = toolTip;
        IconPath   = iconPath;

        // Fill-in runtime details.
        RuntimeDesc= new UK_RuntimeDesc(objType, company, package, name, classType, methodInfo != null ? methodInfo.Name : null,
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
