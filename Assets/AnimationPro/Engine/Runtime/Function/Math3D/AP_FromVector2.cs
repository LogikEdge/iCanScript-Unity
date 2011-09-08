using UnityEngine;
using System.Collections;

public class AP_FromVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[]   vs;
    [AP_OutPort] public float[]     xs;
    [AP_OutPort] public float[]     ys;    

        
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        xs= Prelude.map_(xs, (vec)=> vec.x, vs);
        ys= Prelude.map_(ys, (vec)=> vec.y, vs);
    }

}
