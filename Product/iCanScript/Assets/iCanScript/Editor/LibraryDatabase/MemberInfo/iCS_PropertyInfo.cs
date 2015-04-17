using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using P=Prelude;

public class iCS_PropertyInfo : iCS_MethodInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_AccessorType     AccessorType= iCS_AccessorType.None;
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool IsGet { get { return AccessorType == iCS_AccessorType.Get || AccessorType == iCS_AccessorType.GetAndSet; }}
    public bool IsSet { get { return AccessorType == iCS_AccessorType.Set || AccessorType == iCS_AccessorType.GetAndSet; }}
    public string PropertyName { get { return DisplayName.Substring(4); }}
    public Type type {
        get {
            return IsGet ? FunctionReturn.type : Parameters[P.length(Parameters)-1].type;
        }
    }
    
    // ======================================================================
    // Builders
    // ----------------------------------------------------------------------
    public iCS_PropertyInfo(iCS_ObjectTypeEnum _objType, iCS_TypeInfo _parentTypeInfo,
                            string _name, string _description, string _iconPath,
                            iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
							iCS_StorageClass _storageClass, iCS_AccessorType _accessorType,
							MethodBase _methodBase)
    : base(_objType, _parentTypeInfo, _name, _description, _iconPath, _parameters, _functionReturn, _storageClass, _methodBase)
	{
	    AccessorType= _accessorType;
	}

}
