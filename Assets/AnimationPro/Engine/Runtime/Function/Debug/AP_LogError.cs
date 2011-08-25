using UnityEngine;
using System.Collections;

public class AP_LogError : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort] public string   message;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        if(message != null && message != "") {
            Debug.LogError(message);            
        }
    }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LogError CreateInstance(string _name, AP_Node _parent) {
        AP_LogError logger= CreateInstance<AP_LogError>();
        logger.Init(_name, _parent);
        logger.message= "";
        return logger;
    }

}
