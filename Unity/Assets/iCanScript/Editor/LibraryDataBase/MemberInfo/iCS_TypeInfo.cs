using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_TypeInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public string                   company     = null;
    public string                   package     = null;   // Defaults to declaring namespace
    public Type                     compilerType= null;
    public List<iCS_MemberInfo>     members     = new List<iCS_MemberInfo>();

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    iCS_TypeInfo(string _company, string _package, Type _compilerType,
                 iCS_TypeInfo _parentType, string _displayName, string _description, string _iconPath)
    : base(iCS_ObjectTypeEnum.Type, _parentType, _displayName, _description, _iconPath)
    {
        company     = _company ?? (isGlobalScope ? null : _parentType.company);
        package     = _package ?? (isGlobalScope ? null : _parentType.toString);
        compilerType= _compilerType;
    }

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    // Returns the type name in the form of "company/package/type".
    // ----------------------------------------------------------------------
    public string toString {
        get {
    		string str= !String.IsNullOrEmpty(company) ? company+MemberSeparator : "";
    		if(!String.IsNullOrEmpty(package)) str+= package+MemberSeparator;
    		str+= iCS_types.TypeName(displayName);
    		return str;            
        }
    }
    // ----------------------------------------------------------------------
    public override string ToString() {
        return toString;
    }
}
