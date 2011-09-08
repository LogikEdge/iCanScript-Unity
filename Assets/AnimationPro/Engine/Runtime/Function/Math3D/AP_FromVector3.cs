using UnityEngine;
using System.Collections;

public class AP_FromVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector3[]   vs;
    [AP_OutPort] public float[]     xs;
    [AP_OutPort] public float[]     ys;    
    [AP_OutPort] public float[]     zs;


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        xs= Prelude.map_(xs, (v)=> v.x, vs);
        ys= Prelude.map_(ys, (v)=> v.y, vs);
        zs= Prelude.map_(zs, (v)=> v.z, vs);
    }

}
