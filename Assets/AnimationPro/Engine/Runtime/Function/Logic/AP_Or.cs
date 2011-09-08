using UnityEngine;
using System.Collections;

public sealed class AP_Or : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public bool[] xs;
    [AP_InPort]  public bool[] ys;
    [AP_OutPort] public bool[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x|y, xs, ys);
    }
}
