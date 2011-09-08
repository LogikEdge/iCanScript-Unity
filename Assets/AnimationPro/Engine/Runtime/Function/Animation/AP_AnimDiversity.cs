using UnityEngine;
using System.Collections;

public class AP_AnimDiversity : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AnimDiversity CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AnimDiversity instance= CreateInstance<AP_AnimDiversity>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
    }
}
