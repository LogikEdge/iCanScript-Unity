using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_ReflectionInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
//    public FieldInfo    Field     = null;
//    public bool         IsGetField= true;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldInfo(string company, string package, string name,
                         string toolTip, string iconPath,
                         iCS_ObjectTypeEnum objType, Type classType, FieldInfo fieldInfo,
                         iCS_ParamDirectionEnum[] paramDirs, string[] paramNames, Type[] paramTypes, object[] paramDefaultValues,
                         string returnName)
    : base(objType, company, package, name,
           toolTip, iconPath,
           classType, null, fieldInfo,
           paramDirs, paramNames, paramTypes, paramDefaultValues,
           returnName) {
//        Field     = fieldInfo;
//        IsGetField= fieldInfo != null ? (paramTypes.Length == 0) : true;       
    }

    // ======================================================================
    // Specialized methods
    // ----------------------------------------------------------------------
    public override Type GetReturnType() {
        return IsGetField ? Field.FieldType : typeof(void);
    } 

}
