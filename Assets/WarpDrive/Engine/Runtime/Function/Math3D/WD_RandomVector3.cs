using UnityEngine;
using System.Collections;

public sealed class WD_RandomVector3 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public Vector3    value;
    [WD_InPort]  public float      scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.insideUnitCircle;
    }
}
