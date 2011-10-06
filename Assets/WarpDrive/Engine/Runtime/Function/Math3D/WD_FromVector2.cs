using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public class WD_FromVector2 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector2[]   vs;
    [WD_OutPort] public float[]     xs;
    [WD_OutPort] public float[]     ys;    

        
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        xs= Prelude.map_(xs, (vec)=> vec.x, vs);
        ys= Prelude.map_(ys, (vec)=> vec.y, vs);
    }

}
