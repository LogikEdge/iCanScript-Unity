using UnityEngine;
using System.Collections;

public class AP_Log : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort] public string   message;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        Debug.Log(message);
    }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Log CreateInstance(string _name, AP_Node _parent) {
        AP_Log logger= CreateInstance<AP_Log>();
        logger.Init(_name, _parent);
        logger.message= "";
        return logger;
    }

}
