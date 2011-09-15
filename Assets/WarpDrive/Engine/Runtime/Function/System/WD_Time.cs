using UnityEngine;
using System.Collections;

public sealed class WD_Time : WD_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public float dt;
    [WD_OutPort] public float invDt;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        dt   =  Time.deltaTime;
        invDt=  1.0f/dt;
    }
}
