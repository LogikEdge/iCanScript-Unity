using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class iCS_TypeInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private string                  myCompany    = null;
    private string                  myPackage    = null;   // Defaults to declaring namespace
    public Type                     compilerType = null;
    public Type                     baseType     = null;
    public Type                     declaringType= null;
    public List<iCS_MemberInfo>     members      = new List<iCS_MemberInfo>();

    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public override string company {
        get {
            if(String.IsNullOrEmpty(myCompany) && parentTypeInfo != null) {
                return parentTypeInfo.company;
            }
            return myCompany ?? "";
        }
    }
    public override string package {
        get {
            if(String.IsNullOrEmpty(myPackage) && parentTypeInfo != null) {
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
    public iCS_TypeInfo(string _company, string _package, Type _compilerType, Type _baseType, Type _declaringType,
                        iCS_TypeInfo _declaringTypeInfo, string _displayName, string _description, string _iconPath)
    : base(iCS_ObjectTypeEnum.Type, _declaringTypeInfo, _displayName, _description, _iconPath)
    {
        myCompany    = _company;
        myPackage    = _package;
        compilerType = _compilerType;
        baseType     = _baseType;
        declaringType= _declaringType;
    }

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    // Returns the type name in the form of "company/package/type".
    // ----------------------------------------------------------------------
    public string toString {
        get {
    		string str= !String.IsNullOrEmpty(company) ? company+memberSeparator : "";
    		if(!String.IsNullOrEmpty(package)) str+= package+memberSeparator;
    		str+= iCS_Types.TypeName(compilerType);
    		return str;            
        }
    }
    // ----------------------------------------------------------------------
    public override string ToString() {
        return toString;
    }
}
