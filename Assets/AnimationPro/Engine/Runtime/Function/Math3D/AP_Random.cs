using UnityEngine;
using System.Collections;

public sealed class AP_Random : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public float value;
    [AP_InPort]  public float scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.value;
    }
}
