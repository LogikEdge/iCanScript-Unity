using UnityEngine;
using System.Collections;

public class AP_AnimBlend : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AnimBlend CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AnimBlend instance= CreateInstance<AP_AnimBlend>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
    }
}
