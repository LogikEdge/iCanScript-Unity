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
                            Type classType, MethodBase methodBase,
                            iCS_Parameter[] parameters,
                            string returnName)
    : base(iCS_ObjectTypeEnum.TypeCast, company, package, _name,
           toolTip, iconPath,
           classType, methodBase, null,
           parameters,
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
