using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_TypeCastInfo : iCS_MethodInfo {
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_TypeCastInfo(iCS_TypeInfo _classTypeInfo, string _name,
                            string toolTip, string iconPath,
                            Type classType, MethodBase methodBase,
                            iCS_Parameter[] parameters,
                            string returnName)
    : base(_classTypeInfo, _name, toolTip, iconPath, iCS_ObjectTypeEnum.TypeCast,
           classType, methodBase, parameters, returnName)
    {}
}
