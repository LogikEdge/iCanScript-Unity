using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public sealed class WD_Not : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public bool[] xs;
    [WD_OutPort] public bool[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        os= Prelude.map_(os, (x)=> !x, xs);
    }
}