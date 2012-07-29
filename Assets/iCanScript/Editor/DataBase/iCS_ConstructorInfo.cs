using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_ConstructorInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
//    public MethodBase   Method= null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_ConstructorInfo(string company, string package, string _name,
                          string toolTip, string iconPath,
                          Type classType, MethodBase methodBase,
                          iCS_ParamDirectionEnum[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                          string returnName)
    : base(iCS_ObjectTypeEnum.Constructor, company, package, _name,
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
        return ClassType;
    } 
}
