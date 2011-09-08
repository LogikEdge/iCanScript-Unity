using UnityEngine;
using System.Collections;

public sealed class AP_AddInt : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[] xs;
    [AP_InPort]  public int[] ys;
    [AP_OutPort] public int[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}