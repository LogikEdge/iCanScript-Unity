using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_EventInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    iCS_Parameter[] Parameters= null;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_EventInfo(iCS_TypeInfo _parentTypeInfo, string name,
                         string toolTip, string iconPath,
                         Type classType,
                         iCS_Parameter[] parameters,
                         string returnName, Type returnType)
    : base(iCS_ObjectTypeEnum.Event, _parentTypeInfo, name,
           toolTip, iconPath,
           parameters,
           returnName) {
    }

}
