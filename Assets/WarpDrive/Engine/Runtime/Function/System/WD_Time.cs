using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="System")]
public sealed class WD_Time : WD_Action {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public float dt;
    [WD_OutPort] public float invDt;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        dt   =  Time.deltaTime;
        invDt=  1.0f/dt;
    }
}
