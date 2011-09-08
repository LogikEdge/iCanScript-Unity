using UnityEngine;
using System.Collections;

public sealed class AP_Time : AP_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public float dt;
    [AP_OutPort] public float invDt;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        dt   =  Time.deltaTime;
        invDt=  1.0f/dt;
    }
}
