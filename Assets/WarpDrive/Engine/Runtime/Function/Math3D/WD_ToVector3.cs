using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_ToVector3 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public float[]      xs;
    [WD_InPort]  public float[]      ys;
    [WD_InPort]  public float[]      zs;
    [WD_OutPort] public Vector3[]    vs;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        vs= Prelude.zipWith_(vs, (x,y,z)=> new Vector3(x,y,z), xs, ys, zs);
    }
}
