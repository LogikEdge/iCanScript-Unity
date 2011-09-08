using UnityEngine;
using System.Collections;

public sealed class AP_LerpFloat : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[] xs;
    [AP_InPort]  public float[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public float[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> x+(y-x)*ratio, xs, ys, ratios);
    }
}
