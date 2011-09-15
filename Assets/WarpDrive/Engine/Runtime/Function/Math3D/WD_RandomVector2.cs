using UnityEngine;
using System.Collections;

public sealed class WD_RandomVector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public Vector2    value;
    [WD_InPort]  public float      scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.insideUnitCircle;
    }
}
