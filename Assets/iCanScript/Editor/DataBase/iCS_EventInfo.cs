using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_EventInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_EventInfo(string company, string package, string name,
                         string toolTip, string iconPath,
                         iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase, FieldInfo fieldInfo,
                         iCS_ParamDirectionEnum[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                         string returnName)
    : base(objType, company, package, name,
           toolTip, iconPath,
           classType, methodBase, fieldInfo,
           paramDirs, paramNames, paramTypes, paramDefaultValues,
           returnName) {}

}
