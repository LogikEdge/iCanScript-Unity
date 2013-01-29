using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_MethodInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
//    public MethodBase   Method= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MethodInfo(string company, string package, string _name,
                          string toolTip, string iconPath,
                          iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase,
                          iCS_ParamDirectionEnum[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                          string returnName)
    : base(objType, company, package, _name,
           toolTip, iconPath,
           classType, methodBase, null,
           paramDirs, paramNames, paramTypes, paramDefaultValues,
           returnName) {
        
//        Method= methodBase;
    }

    // ======================================================================
    // Specialized methods
    // ----------------------------------------------------------------------
    public override Type GetReturnType() {
        MethodInfo methodInfo= Method as MethodInfo;
        return methodInfo == null ? typeof(void) : methodInfo.ReturnType;
    } 

}
