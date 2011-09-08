using UnityEngine;
using System.Collections;

public sealed class AP_Time : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public float dt;
    [AP_OutPort] public float invDt;


    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_Time CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Time time= CreateInstance<AP_Time>();
        time.Init(theFunctionName, theParent);
        return time;
    }
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        dt   =  Time.deltaTime;
        invDt=  1.0f/dt;
    }
}
