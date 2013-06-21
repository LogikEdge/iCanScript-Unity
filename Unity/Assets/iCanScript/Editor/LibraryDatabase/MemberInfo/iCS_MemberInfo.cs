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
    public  iCS_ObjectTypeEnum   objectType      = iCS_ObjectTypeEnum.Unknown;
    public  iCS_TypeInfo         parentTypeInfo  = null;
    public  string               displayName     = null;
    private string               myDescription   = null;
    private string               myIconPath      = null;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_MemberInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                          string _name, string _description, string _iconPath) {
		objectType    = _objType;
		parentTypeInfo= _parentTypeInfo;
		displayName   = _name;
        description   = _description;
        iconPath      = _iconPath;
		// Register ourself in parent type.
		if(_parentTypeInfo != null) {
			_parentTypeInfo.members.Add(this);			
		}

    }

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public iCS_TypeInfo         toTypeInfo          { get { return this as iCS_TypeInfo; }}
    public iCS_ConstructorInfo  toConstructorInfo   { get { return this as iCS_ConstructorInfo; }}
    public iCS_MethodBaseInfo   toMethodBaseInfo    { get { return this as iCS_MethodBaseInfo; }}
    public iCS_MethodInfo       toMethodInfo        { get { return this as iCS_MethodInfo; }}
    public iCS_FieldInfo        toFieldInfo         { get { return this as iCS_FieldInfo; }}
    public iCS_PropertyInfo     toPropertyInfo      { get { return this as iCS_PropertyInfo; }}
    public iCS_MessageInfo      toMessageInfo       { get { return this as iCS_MessageInfo; }}
	public iCS_TypeCastInfo		toTypeCastInfo		{ get { return this as iCS_TypeCastInfo; }}
    // ----------------------------------------------------------------------
    public bool isGlobalScope         { get { return parentTypeInfo == null; }}
    // ----------------------------------------------------------------------
    public bool isTypeInfo            { get { return toTypeInfo != null; }}
    public bool isConstructor         { get { return toConstructorInfo != null; }}
    public bool isMethodBase          { get { return toMethodBaseInfo != null; }}
    public bool isMethod              { get { return toMethodInfo != null; }}
    public bool isField               { get { return toFieldInfo != null; }}
    public bool isMessage             { get { return toMessageInfo != null; }}
    public bool isProperty            { get { return toPropertyInfo != null; }}
	public bool isTypeCast			  { get { return toTypeCastInfo != null; }}
    public bool isInstanceMethodBase  { get { return isMethodBase && toMethodBaseInfo.isInstanceMember; }}
    public bool isClassMethodBase     { get { return isMethodBase && toMethodBaseInfo.isClassMember; }}
    public bool isInstanceMethod      { get { return isMethod && toMethodInfo.isInstanceMember; }}
    public bool isClassMethod         { get { return isMethod && toMethodInfo.isClassMember; }}
    public bool isInstanceMessage     { get { return isMessage && toMessageInfo.isInstanceMember; }}
    public bool isClassMessage        { get { return isMessage && toMessageInfo.isClassMember; }}
    public bool isInstanceField       { get { return isField && toFieldInfo.isInstanceMember; }}
    public bool isClassField          { get { return isField && toFieldInfo.isClassMember; }}
    public bool isGetField            { get { return isField && toFieldInfo.isGet; }}
    public bool isSetField            { get { return isField && toFieldInfo.isSet; }}     
    public bool isGetInstanceField    { get { return isInstanceField && isGetField; }}
    public bool isSetInstanceField    { get { return isInstanceField && isSetField; }}
    public bool isGetClassField       { get { return isClassField && isGetField; }}
    public bool isSetClassField       { get { return isClassField && isSetField; }}
    public bool isGetProperty         { get { return isProperty && toPropertyInfo.isGet; }}
    public bool isSetProperty         { get { return isProperty && toPropertyInfo.isSet; }}
    public bool isGetInstanceProperty { get { return isGetProperty && toPropertyInfo.isInstanceMember; }}
    public bool isSetInstanceProperty { get { return isSetProperty && toPropertyInfo.isInstanceMember; }}
    public bool isGetClassProperty    { get { return isGetProperty && toPropertyInfo.isClassMember; }}
    public bool isSetClassProperty    { get { return isSetProperty && toPropertyInfo.isClassMember; }}

    // ======================================================================
    // Dynamic Properties
    // ----------------------------------------------------------------------
    public virtual string company {
        get {
            return parentTypeInfo.company;            
        }
    }
    public virtual string package {
        get {
            return parentTypeInfo.package;            
        }
    }
    public virtual Type classType {
        get {
            return parentTypeInfo.compilerType;
        }
    }
    public string description {
        get {
            if(String.IsNullOrEmpty(myDescription)) {
                return parentTypeInfo == null ? "" : parentTypeInfo.description;
            }
            return myDescription;            
        }
        set {
            myDescription= value;
        }
    }
    public string iconPath {
        get {
            if(String.IsNullOrEmpty(myIconPath)) {
                return parentTypeInfo == null ? "" : parentTypeInfo.iconPath;
            }
            return myIconPath;            
        }
        set {
            myIconPath= value;
        }
    }
}
