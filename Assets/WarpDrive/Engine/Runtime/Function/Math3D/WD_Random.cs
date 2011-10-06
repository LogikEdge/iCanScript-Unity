using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_Random : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public float value;
    [WD_InPort]  public float scale= 1.0f;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        value= scale*Random.value;
    }
}
