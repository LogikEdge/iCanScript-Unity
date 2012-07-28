using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_ReflectionInfo {
    public iCS_FieldInfo(string company, string package, string name,
                         string toolTip, string iconPath,
                         iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase, FieldInfo fieldInfo,
                         bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                         string returnName)
    : base(company, package, name,
           toolTip, iconPath,
           objType, classType, methodBase, fieldInfo,
           paramIsOuts, paramNames, paramTypes, paramDefaultValues,
           returnName) {}
}
