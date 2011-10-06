using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_Scale2Vector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector2[] xs;
    [WD_InPort]  public float[] scales1;
    [WD_InPort]  public float[] scales2;
    [WD_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        os= Prelude.zipWith_(os, (x,s1,s2)=> s1*s2*x, xs, scales1, scales2);
    }
}

