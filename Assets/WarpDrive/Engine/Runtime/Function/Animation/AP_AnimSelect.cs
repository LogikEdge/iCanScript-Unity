using UnityEngine;
using System.Collections;

public class AP_AnimSelect : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AnimSelect CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AnimSelect instance= CreateInstance<AP_AnimSelect>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
    }
}
