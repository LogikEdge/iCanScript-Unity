using UnityEngine;
using System.Collections;

public class AP_LogWarning : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort] public string   message;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        if(message != null && message != "") {
            Debug.LogWarning(message);            
        }
    }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LogWarning CreateInstance(string _name, AP_Node _parent) {
        AP_LogWarning logger= CreateInstance<AP_LogWarning>();
        logger.Init(_name, _parent);
        logger.message= "";
        return logger;
    }

}
