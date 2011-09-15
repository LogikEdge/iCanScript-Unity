using UnityEngine;
using System.Collections;

public sealed class WD_AddVector3 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector3[] xs;
    [WD_InPort]  public Vector3[] ys;
    [WD_OutPort] public Vector3[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}