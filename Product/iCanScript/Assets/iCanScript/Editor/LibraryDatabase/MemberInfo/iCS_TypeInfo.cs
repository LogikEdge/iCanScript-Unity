using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using iCanScript;

public class iCS_TypeInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    private string                  myCompany    = null;
    private string                  myLibrary    = null;
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
    public override string Library {
        get {
            if(String.IsNullOrEmpty(myLibrary) && ParentTypeInfo != null) {
                return ParentTypeInfo.Library;
            }
            return myLibrary ?? "";            
        }
    }
    public override Type ClassType {
        get {
            return CompilerType;
        }
    }
    public string ClassName {
        get {
            return ClassType.Name;
        }
    }
    
    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_TypeInfo(string _company, string _library, Type _compilerType, Type _baseType, Type _declaringType,
                        iCS_TypeInfo _declaringTypeInfo, string _displayName, string _description, string _iconPath,
						bool _hideFromLibrary)
    : base(iCS_ObjectTypeEnum.Type, _declaringTypeInfo, _displayName, _description, _iconPath)
    {
        myCompany      = _company;
        myLibrary      = _library;
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
		if(!String.IsNullOrEmpty(Library)) str+= Library+memberSeparator;
		str+= iCS_Types.TypeName(CompilerType);
		return str;    }
}
