using UnityEngine;
using System.Collections;

public sealed class WD_Not : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public bool[] xs;
    [WD_OutPort] public bool[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.map_(os, (x)=> !x, xs);
    }
}