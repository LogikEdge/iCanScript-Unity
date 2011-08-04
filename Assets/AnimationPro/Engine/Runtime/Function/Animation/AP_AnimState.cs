using UnityEngine;
using System.Collections;

public class AP_AnimState : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AnimState CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AnimState instance= CreateInstance<AP_AnimState>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
    }
}
