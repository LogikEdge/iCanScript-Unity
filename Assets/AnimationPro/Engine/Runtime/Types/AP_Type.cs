using UnityEngine;
using System.Collections;

[System.Serializable]
public class AP_Type {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [SerializeField] string         myTypeName= null;
                     AP_TypeInfo   myTypeInfo= null;


    // ----------------------------------------------------------------------
    // Constrcuts a AP_Type.
    public AP_Type(System.Type theType) {
        myTypeName= AP_TypeSystem.GetTypeName(theType);
    }
    
    // ----------------------------------------------------------------------
    public AP_TypeInfo TypeInfo {
        get {
            if(myTypeInfo == null) {
                myTypeInfo= AP_TypeSystem.GetTypeInfo(myTypeName);
            }
            return myTypeInfo;
        }
        set { ValueType= value.ValueType; }
    }
    
    // ----------------------------------------------------------------------
    public System.Type ValueType {
        get {
            AP_TypeInfo theTypeInfo= TypeInfo;
            return theTypeInfo != null ? theTypeInfo.ValueType : null;
        }
        set {
            myTypeName= AP_TypeSystem.GetTypeName(value);
            myTypeInfo= null;            
        }
    }

}
