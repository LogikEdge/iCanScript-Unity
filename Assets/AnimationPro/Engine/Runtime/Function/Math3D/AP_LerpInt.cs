using UnityEngine;
using System.Collections;

public sealed class AP_LerpInt : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[] xs;
    [AP_InPort]  public int[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public int[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> (int)(x+(y-x)*ratio), xs, ys, ratios);
    }
}
