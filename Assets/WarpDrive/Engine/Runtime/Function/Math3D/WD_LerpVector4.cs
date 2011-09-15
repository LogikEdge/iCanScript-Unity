using UnityEngine;
using System.Collections;

public sealed class WD_LerpVector4 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector4[] xs;
    [WD_InPort]  public Vector4[] ys;
    [WD_InPort]  public float[] ratios;
    [WD_OutPort] public Vector4[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> x+(y-x)*ratio, xs, ys, ratios);
    }
}
