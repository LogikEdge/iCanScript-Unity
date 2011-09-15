using UnityEngine;
using System.Collections;

[System.Serializable]
public class WD_Type {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [SerializeField] string         myTypeName= null;
                     WD_TypeInfo   myTypeInfo= null;


    // ----------------------------------------------------------------------
    // Constrcuts a WD_Type.
    public WD_Type(System.Type theType) {
        myTypeName= WD_TypeSystem.GetTypeName(theType);
    }
    
    // ----------------------------------------------------------------------
    public WD_TypeInfo TypeInfo {
        get {
            if(myTypeInfo == null) {
                myTypeInfo= WD_TypeSystem.GetTypeInfo(myTypeName);
            }
            return myTypeInfo;
        }
        set { ValueType= value.ValueType; }
    }
    
    // ----------------------------------------------------------------------
    public System.Type ValueType {
        get {
            WD_TypeInfo theTypeInfo= TypeInfo;
            return theTypeInfo != null ? theTypeInfo.ValueType : null;
        }
        set {
            myTypeName= WD_TypeSystem.GetTypeName(value);
            myTypeInfo= null;            
        }
    }

}
