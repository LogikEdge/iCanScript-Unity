using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_AddVector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector2[] xs;
    [WD_InPort]  public Vector2[] ys;
    [WD_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}