using UnityEngine;
using System.Collections;

public class WD_LogWarning : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort] public string   message= "";
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        if(message != null && message != "") {
            Debug.LogWarning(message);            
        }
    }

}
