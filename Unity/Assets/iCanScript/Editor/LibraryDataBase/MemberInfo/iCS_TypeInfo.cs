using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_TypeInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private string                  myCompany   = null;
    private string                  myPackage   = null;   // Defaults to declaring namespace
    public Type                     compilerType= null;
    public List<iCS_MemberInfo>     members     = new List<iCS_MemberInfo>();

    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public override string company {
        get {
            if(String.IsEmptyOrNull(myCompany) && parentTypeInfo != null) {
                return parentTypeInfo.company;
            }
            return myCompany ?? "";
        }
    }
    public override string package {
        get {
            if(String.IsEmptyOrNull(myPackage) && parentTypeInfo != null) {
                return parentTypeInfo.package;
            }
            return myPackage ?? "";            
        }
    }
    public override Type classType {
        get {
            return compilerType;
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    iCS_TypeInfo(string _company, string _package, Type _compilerType,
                 iCS_TypeInfo _parentType, string _displayName, string _description, string _iconPath)
    : base(iCS_ObjectTypeEnum.Type, _parentType, _displayName, _description, _iconPath)
    {
        myCompany   = _company;
        myPackage   = _package;
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
