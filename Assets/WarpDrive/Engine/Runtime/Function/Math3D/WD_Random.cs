using UnityEngine;
using System.Collections;

public sealed class WD_Random : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public float value;
    [WD_InPort]  public float scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.value;
    }
}
