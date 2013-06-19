using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_MethodBaseInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public FieldInfo            field         = null;
	public Type					type          = null;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet {
        get { return parameters.Length == 0; }
    }
    public bool isSet {
        get { return !isGet; }
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
						 FieldInfo _fieldInfo)
    : base(objType, _parentType, _name, _description, _iconPath, _parameters, _functionReturn)
    {
        field= _fieldInfo;
		type = _parameters.Length == 1 ? _parameters[0].type : functionReturn.type;
    }

    // ======================================================================
    // Instance specific methods
    // ----------------------------------------------------------------------
    public string fieldName    { get { return displayName.Substring(4); }}

}
