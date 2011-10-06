using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_ToVector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public float[]      xs;
    [WD_InPort]  public float[]      ys;
    [WD_OutPort] public Vector2[]    vs;
        
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        vs= Prelude.zipWith_(vs, (x,y)=> new Vector2(x,y), xs, ys);
    }
}
