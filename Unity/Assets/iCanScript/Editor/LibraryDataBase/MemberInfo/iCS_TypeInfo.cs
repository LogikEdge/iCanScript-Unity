using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_TypeInfo : MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string                   Company     = "(no company)";
    public string                   Package     = null;   // Defaults to declaring namespace
    public Type                     CompilerType= null;
    public List<iCS_MemberInfo>     Members     = new List<iCS_MemberInfo>();

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    iCS_TypeInfo(Type _compilerType, string _displayName, string _company, string _package, string _iconPath, string _description)
    : base(iCS_ObjectTypeEnum.Type, _displayName, _description, _iconPath){
        Company     = _company;
        Package     = _package;
        CompilerType= _compilerType;
    }
}
