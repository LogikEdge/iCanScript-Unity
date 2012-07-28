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
                          bool[] paramIsOuts, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                          string returnName)
    : base(company, package, _name,
           toolTip, iconPath,
           objType, classType, methodBase, null,
           paramIsOuts, paramNames, paramTypes, paramDefaultValues,
           returnName) {
        
//        Method= methodBase;
    }
}
