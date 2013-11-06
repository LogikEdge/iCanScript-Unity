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
    public Type                     CompilerType = null;
    public Type                     BaseType     = null;
    public Type                     DeclaringType= null;
    public List<iCS_MemberInfo>     Members      = new List<iCS_MemberInfo>();

    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public override string Company {
        get {
            if(String.IsNullOrEmpty(myCompany) && ParentTypeInfo != null) {
                return ParentTypeInfo.Company;
            }
            return myCompany ?? "";
        }
    }
    public override string Package {
        get {
            if(String.IsNullOrEmpty(myPackage) && ParentTypeInfo != null) {
                return ParentTypeInfo.Package;
            }
            return myPackage ?? "";            
        }
    }
    public override Type ClassType {
        get {
            return CompilerType;
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_TypeInfo(string _company, string _package, Type _compilerType, Type _baseType, Type _declaringType,
                        iCS_TypeInfo _declaringTypeInfo, string _displayName, string _description, string _iconPath,
						bool _hideFromLibrary)
    : base(iCS_ObjectTypeEnum.Type, _declaringTypeInfo, _displayName, _description, _iconPath)
    {
        myCompany      = _company;
        myPackage      = _package;
        CompilerType   = _compilerType;
        BaseType       = _baseType;
        DeclaringType  = _declaringType;
		HideFromLibrary= _hideFromLibrary;
    }

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    // Returns the type name in the form of "company/package/type".
    public override string ToString() {
		string str= !String.IsNullOrEmpty(Company) ? Company+memberSeparator : "";
		if(!String.IsNullOrEmpty(Package)) str+= Package+memberSeparator;
		str+= iCS_Types.TypeName(CompilerType);
		return str;    }
}
