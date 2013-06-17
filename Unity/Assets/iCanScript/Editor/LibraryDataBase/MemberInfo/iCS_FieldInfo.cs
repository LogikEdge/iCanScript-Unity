using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public FieldInfo        field       = null;
    public iCS_Parameter    parameter   = null;
    public iCS_AccessorType accessorType= iCS_AccessorType.None;

    // ======================================================================
    // Temporary Property
    // ----------------------------------------------------------------------
    // TODO: Remove temporary property.
    public FieldInfo Field {
        get { return field; }
        set { field= value; }
    }
    public iCS_Parameter Parameter {
        get { return parameter; }
        set { parameter= value; }
    }
    
    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet {
        get { return accessorType == iCS_AccessorType.Get || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public bool isSet {
        get { return accessorType == iCS_AccessorType.Set || accessorType == iCS_AccessorType.GetAndSet; }
    }
    

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _parentType,
                         string _name, string _description, string _iconPath,
                         FieldInfo _fieldInfo,
                         iCS_Parameter _parameter)
    : base(objType, _parentType, _name, _description, _iconPath, _classInfo)
    {
        field    = _fieldInfo;
        parameter= _parameter;
    }

    // ======================================================================
    // Instance specific methods
    // ----------------------------------------------------------------------
    public string fieldName {
        get { return field != null ? field.Name : null; }
    }

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    // Returns string identification as "company/package/type/fieldName".
    public string toString {
        get {
    		string str= parentType.ToString();
            str+= iCS_MemberSeparator+displayName;
    		return str;            
        }
    }
    public override string ToString() {
        return toString;
    }
}
