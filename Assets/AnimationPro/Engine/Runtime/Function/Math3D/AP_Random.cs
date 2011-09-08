using UnityEngine;
using System.Collections;

public sealed class AP_Random : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public float value;
    [AP_InPort]  public float scale= 1.0f;


    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_Random CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Random instance= CreateInstance<AP_Random>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.value;
    }
}
