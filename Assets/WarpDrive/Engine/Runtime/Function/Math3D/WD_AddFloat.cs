using UnityEngine;
using System.Collections;

public sealed class WD_AddFloat : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public float[] xs;
    [WD_InPort]  public float[] ys;
    [WD_OutPort] public float[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}