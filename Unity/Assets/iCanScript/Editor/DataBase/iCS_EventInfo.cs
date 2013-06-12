using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_EventInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    string  myDisplayName= null;
    Type    myReturnType= null;
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_EventInfo(string company, string package, string name,
                         string toolTip, string iconPath,
                         Type classType,
                         iCS_ParamDirection[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                         string returnName, Type returnType)
    : base(iCS_ObjectTypeEnum.Event, company, package, name,
           toolTip, iconPath,
           classType, null, null,
           paramDirs, paramNames, paramTypes, paramDefaultValues,
           returnName) {
        myDisplayName= name;
        myReturnType= returnType;
    }

    // ======================================================================
    // Specialized methods
    // ----------------------------------------------------------------------
    public override Type GetReturnType() {
        return myReturnType;
    } 
    public string GetDisplayName() {
        return myDisplayName;
    }

}
