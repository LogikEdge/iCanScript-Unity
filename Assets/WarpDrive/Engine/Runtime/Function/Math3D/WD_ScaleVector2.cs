using UnityEngine;
using System.Collections;

public sealed class WD_ScaleVector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector2[] xs;
    [WD_InPort]  public float[] scales;
    [WD_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,scale)=> scale*x, xs, scales);
    }
}
