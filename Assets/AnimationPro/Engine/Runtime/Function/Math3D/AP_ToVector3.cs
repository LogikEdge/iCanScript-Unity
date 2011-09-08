using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AP_ToVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[]      xs;
    [AP_InPort]  public float[]      ys;
    [AP_InPort]  public float[]      zs;
    [AP_OutPort] public Vector3[]    vs;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        vs= Prelude.zipWith_(vs, (x,y,z)=> new Vector3(x,y,z), xs, ys, zs);
    }
}
