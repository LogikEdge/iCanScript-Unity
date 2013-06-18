using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public class iCS_FieldInfo : iCS_MemberInfo {
    // ======================================================================
    // Fields
    // ----------------------------------------------------------------------
    public FieldInfo            field         = null;
	public Type					type          = null;
    public iCS_Parameter        parameter     = null;
	public iCS_FunctionReturn	functionReturn= null;
    public iCS_AccessorType     accessorType= iCS_AccessorType.None;
	public iCS_StorageClass	    storageClass= iCS_StorageClass.Instance;

    // ======================================================================
    // Accessors
    // ----------------------------------------------------------------------
    public bool isGet {
        get { return accessorType == iCS_AccessorType.Get || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public bool isSet {
        get { return accessorType == iCS_AccessorType.Set || accessorType == iCS_AccessorType.GetAndSet; }
    }
    public bool isInstanceMember {
        get { return storageClass == iCS_StorageClass.Instance; }
    }
    public bool isClassMember {
        get { return storageClass == iCS_StorageClass.Class; }
    }
    

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public iCS_FieldInfo(iCS_ObjectTypeEnum objType, iCS_TypeInfo _parentType,
                         string _name, string _description, string _iconPath,
						 Type _type, iCS_Parameter _parameter,
						 iCS_FunctionReturn _functionReturn, FieldInfo _fieldInfo)
    : base(objType, _parentType, _name, _description, _iconPath)
    {
		type		  = _type;
        field         = _fieldInfo;
        parameter     = _parameter;
		functionReturn= _functionReturn;
		accessorType  = (_functionReturn == null || _functionReturn.type == typeof(void)) ?
							iCS_AccessorType.Set :
							iCS_AccessorType.Get;
		storageClass= (objType == iCS_ObjectTypeEnum.InstanceField) ?
							iCS_StorageClass.Instance :
							iCS_StorageClass.Class;
    }

    // ======================================================================
    // Instance specific methods
    // ----------------------------------------------------------------------
    public string fieldName    { get { return displayName.Substring(4); }}

    // ======================================================================
    // Common method override
    // ----------------------------------------------------------------------
    // Returns string identification as "company/package/type/fieldName".
    public string toString {
        get {
    		string str= parentType.toString;
            str+= iCS_MemberSeparator+displayName;
    		return str;            
        }
    }
    public override string ToString() {
        return toString;
    }
}
