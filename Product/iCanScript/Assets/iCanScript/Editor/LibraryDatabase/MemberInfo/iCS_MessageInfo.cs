using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

namespace iCanScript.Editor {
    
    public class iCS_MessageInfo : iCS_FunctionPrototype {
        // ======================================================================
        // Fields
        // ----------------------------------------------------------------------
    
        // ======================================================================
        // Creation/Destruction
        // ----------------------------------------------------------------------
        public iCS_MessageInfo(iCS_TypeInfo _declaringTypeInfo, string _displayName,
                               string _description, string _iconPath,
                               iCS_Parameter[] _parameters,
                               iCS_FunctionReturn _functionReturn,
                               iCS_StorageClass _storageClass)
        : base(_storageClass == iCS_StorageClass.Instance ? iCS_ObjectTypeEnum.InstanceMessage : iCS_ObjectTypeEnum.ClassMessage, _declaringTypeInfo,
               _displayName, _description, _iconPath,
               _parameters, _functionReturn, _storageClass)
        {
        }

    }

}
