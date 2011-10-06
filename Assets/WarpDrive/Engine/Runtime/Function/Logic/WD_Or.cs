using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public sealed class WD_Or : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public bool[] xs;
    [WD_InPort]  public bool[] ys;
    [WD_OutPort] public bool[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x|y, xs, ys);
    }
}
