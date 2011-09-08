using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AP_ToVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[]      xs;
    [AP_InPort]  public float[]      ys;
    [AP_OutPort] public Vector2[]    vs;
        
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        vs= Prelude.zipWith_(vs, (x,y)=> new Vector2(x,y), xs, ys);
    }
}
