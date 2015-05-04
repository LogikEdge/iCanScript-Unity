using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using iCanScript.Engine;

namespace iCanScript.Editor {
    
    public class iCS_FieldInfo : iCS_FunctionPrototype {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
        public iCS_AccessorType     AccessorType= iCS_AccessorType.None;
        public FieldInfo            Field         = null;
    	public Type					type          = null;

        // ======================================================================
        // Accessors
        // ----------------------------------------------------------------------
        public bool IsGet {
            get { return AccessorType == iCS_AccessorType.Get || AccessorType == iCS_AccessorType.GetAndSet; }
        }
        public bool IsSet {
            get { return AccessorType == iCS_AccessorType.Set || AccessorType == iCS_AccessorType.GetAndSet; }
        }
        public override string MethodName {
            get { return Field.Name; }
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
            Field= _fieldInfo;
            AccessorType= _accessorType;
    		type = FunctionReturn != null && FunctionReturn.type != typeof(void) ?
    		            FunctionReturn.type :
    		            _parameters[_parameters.Length-1].type;
        }

        // ======================================================================
        // Instance specific methods
        // ----------------------------------------------------------------------
        public string FieldName    { get { return DisplayName.Substring(4); }}

    }

}
