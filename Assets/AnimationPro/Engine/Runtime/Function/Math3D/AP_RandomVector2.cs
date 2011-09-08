using UnityEngine;
using System.Collections;

public sealed class AP_RandomVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public Vector2    value;
    [AP_InPort]  public float      scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.insideUnitCircle;
    }
}
