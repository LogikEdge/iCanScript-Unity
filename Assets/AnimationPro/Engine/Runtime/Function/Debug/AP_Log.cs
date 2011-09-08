using UnityEngine;
using System.Collections;

public class AP_Log : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort] public string   message= "";
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        if(message != null && message != "") {
            Debug.Log(message);
        }
    }
}
