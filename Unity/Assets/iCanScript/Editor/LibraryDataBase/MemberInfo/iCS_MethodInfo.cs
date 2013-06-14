using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_MethodInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public MethodBase           Method= null;
    public iCS_Parameter[]      Parameters= null;
    public iCS_FunctionReturn   FunctionReturn= null;
    public iCS_StorageSpecifier StorageSpecifier= iCS_StorageSpecifier.Instance;      

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isInstanceMember {
        get { return StorageSpecifier == iCS_StorageSpecifier.Instance; }
    }
    public bool isClassMember {
        get { return StorageSpecifier == iCS_StorageSpecifier.Class; }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MethodInfo(iCS_ObjectTypeEnum objType,, iCS_TypeInfo _classInfo,
                          string _name, string _description, string _iconPath,
                          MethodBase _methodBase, iCS_Parameter[] _parameters,
                          string returnName)
    : base(objType, _classInfo, _name, _description, _iconPath) {
        Method        = _methodBase;
        Parameters    = _parameters;
        FunctionReturn= new iCS_FunctionReturn(returnName, _methodBase.ReturnType, this);
    }

    // ======================================================================
    // Specialized methods
    // ----------------------------------------------------------------------
    public Type ReturnType {
        get { return FunctionReturn != null ? FunctionReturn.type : typeof(void); }
    } 

}
