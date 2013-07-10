using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public abstract class iCS_MemberInfo {
    // ======================================================================
    // Constants
    // ----------------------------------------------------------------------
    // TODO: Use periods (.) instead of slashes (/) for type string.
    public const string memberSeparator= "/";
    
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public  iCS_ObjectTypeEnum   ObjectType      = iCS_ObjectTypeEnum.Unknown;
    public  iCS_TypeInfo         ParentTypeInfo  = null;
    public  string               DisplayName     = null;
    private string               myDescription   = null;
    private string               myIconPath      = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MemberInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                          string _name, string _description, string _iconPath) {
		ObjectType    = _objType;
		ParentTypeInfo= _parentTypeInfo;
		DisplayName   = _name;
        Description   = _description;
        IconPath      = _iconPath;
		// Register ourself in parent type.
		if(_parentTypeInfo != null) {
			_parentTypeInfo.Members.Add(this);			
		}

    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_TypeInfo         ToTypeInfo          { get { return this as iCS_TypeInfo; }}
    public iCS_ConstructorInfo  ToConstructorInfo   { get { return this as iCS_ConstructorInfo; }}
    public iCS_MethodBaseInfo   ToMethodBaseInfo    { get { return this as iCS_MethodBaseInfo; }}
    public iCS_MethodInfo       ToMethodInfo        { get { return this as iCS_MethodInfo; }}
    public iCS_FieldInfo        ToFieldInfo         { get { return this as iCS_FieldInfo; }}
    public iCS_PropertyInfo     ToPropertyInfo      { get { return this as iCS_PropertyInfo; }}
    public iCS_MessageInfo      ToMessageInfo       { get { return this as iCS_MessageInfo; }}
	public iCS_TypeCastInfo		ToTypeCastInfo		{ get { return this as iCS_TypeCastInfo; }}
    // ----------------------------------------------------------------------
    public bool isGlobalScope          { get { return ParentTypeInfo == null; }}
    // ----------------------------------------------------------------------
    public bool IsTypeInfo             { get { return ToTypeInfo != null; }}
    public bool IsConstructor          { get { return ToConstructorInfo != null; }}
    public bool IsMethodBase           { get { return ToMethodBaseInfo != null; }}
    public bool IsMethod               { get { return ToMethodInfo != null; }}
    public bool IsField                { get { return ToFieldInfo != null; }}
    public bool IsMessage              { get { return ToMessageInfo != null; }}
    public bool IsProperty             { get { return ToPropertyInfo != null; }}
	public bool IsTypeCast			   { get { return ToTypeCastInfo != null; }}
    public bool IsInstanceFunctionBase { get { return IsMethodBase && ToMethodBaseInfo.IsInstanceMember; }}
    public bool IsClassFunctionBase    { get { return IsMethodBase && ToMethodBaseInfo.IsClassMember; }}
    public bool IsInstanceFunction     { get { return IsMethod && ToMethodInfo.IsInstanceMember; }}
    public bool IsClassFunction        { get { return IsMethod && ToMethodInfo.IsClassMember; }}
    public bool IsInstanceMessage      { get { return IsMessage && ToMessageInfo.IsInstanceMember; }}
    public bool IsClassMessage         { get { return IsMessage && ToMessageInfo.IsClassMember; }}
    public bool IsInstanceField        { get { return IsField && ToFieldInfo.IsInstanceMember; }}
    public bool IsClassField           { get { return IsField && ToFieldInfo.IsClassMember; }}
    public bool IsGetField             { get { return IsField && ToFieldInfo.IsGet; }}
    public bool IsSetField             { get { return IsField && ToFieldInfo.IsSet; }}     
    public bool IsGetInstanceField     { get { return IsInstanceField && IsGetField; }}
    public bool IsSetInstanceField     { get { return IsInstanceField && IsSetField; }}
    public bool IsGetClassField        { get { return IsClassField && IsGetField; }}
    public bool IsSetClassField        { get { return IsClassField && IsSetField; }}
    public bool IsGetProperty          { get { return IsProperty && ToPropertyInfo.IsGet; }}
    public bool IsSetProperty          { get { return IsProperty && ToPropertyInfo.IsSet; }}
    public bool IsGetInstanceProperty  { get { return IsGetProperty && ToPropertyInfo.IsInstanceMember; }}
    public bool IsSetInstanceProperty  { get { return IsSetProperty && ToPropertyInfo.IsInstanceMember; }}
    public bool IsGetClassProperty     { get { return IsGetProperty && ToPropertyInfo.IsClassMember; }}
    public bool IsSetClassProperty     { get { return IsSetProperty && ToPropertyInfo.IsClassMember; }}

    // ======================================================================
    // Dynamic Properties
    // ----------------------------------------------------------------------
    public virtual string Company {
        get {
            return ParentTypeInfo.Company;            
        }
    }
    public virtual string Package {
        get {
            return ParentTypeInfo.Package;            
        }
    }
    public virtual Type ClassType {
        get {
            return ParentTypeInfo.CompilerType;
        }
    }
    public string Description {
        get {
            if(String.IsNullOrEmpty(myDescription)) {
                return ParentTypeInfo == null ? "" : ParentTypeInfo.Description;
            }
            return myDescription;            
        }
        set {
            myDescription= value;
        }
    }
    public string IconPath {
        get {
            if(String.IsNullOrEmpty(myIconPath)) {
                return ParentTypeInfo == null ? "" : ParentTypeInfo.IconPath;
            }
            return myIconPath;            
        }
        set {
            myIconPath= value;
        }
    }
}
