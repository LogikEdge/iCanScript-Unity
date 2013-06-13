using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_PropertyInfo : iCS_MethodInfo {
    public iCS_PropertyInfo(string company, string package, string _name,
                          string toolTip, string iconPath,
                          iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase,
                          iCS_Parameter[] parameters,
                          string returnName)
    : base(company, package, _name, toolTip, iconPath, objType, classType, methodBase, parameters, returnName) {}
}
