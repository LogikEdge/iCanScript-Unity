using UnityEngine;
using System.Collections;

public sealed class WD_And : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public bool[] xs;
    [WD_InPort]  public bool[] ys;
    [WD_OutPort] public bool[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x&y, xs, ys);
    }
}