using UnityEngine;
using System.Collections;

public sealed class WD_AddInt : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public int[] xs;
    [WD_InPort]  public int[] ys;
    [WD_OutPort] public int[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}