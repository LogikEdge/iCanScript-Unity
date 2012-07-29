using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_TypeCastInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
//    public MethodBase   Method= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_TypeCastInfo(string company, string package, string _name,
                            string toolTip, string iconPath,
                            iCS_ObjectTypeEnum objType, Type classType, MethodBase methodBase,
                            iCS_ParamDirectionEnum[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                            string returnName)
    : base(company, package, _name,
           toolTip, iconPath,
           objType, classType, methodBase, null,
           paramDirs, paramNames, paramTypes, paramDefaultValues,
           returnName) {

//        Method= methodBase;
    }
}
