using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_MethodBaseInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public iCS_AccessorType     accessorType= iCS_AccessorType.None;
    public FieldInfo            field         = null;
	public Type					type          = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet {
        get { return accessorType == iCS_AccessorType.Get || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public bool isSet {
        get { return accessorType == iCS_AccessorType.Set || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public override string methodName {
        get { return field.Name; }
    }
    

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _parentType,
                         string _name, string _description, string _iconPath,
						 iCS_Parameter[] _parameters, iCS_FunctionReturn _functionReturn,
						 iCS_StorageClass _storageClass, iCS_AccessorType _accessorType,
						 FieldInfo _fieldInfo)
    : base(objType, _parentType, _name, _description, _iconPath, _parameters, _functionReturn, _storageClass)
    {
        field= _fieldInfo;
        accessorType= _accessorType;
		type = functionReturn != null && functionReturn.type != typeof(void) ?
		            functionReturn.type :
		            _parameters[_parameters.Length-1].type;
    }

    // ======================================================================
    // Instance specific methods
    // ----------------------------------------------------------------------
    public string fieldName    { get { return displayName.Substring(4); }}

}
