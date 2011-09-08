using UnityEngine;
using System.Collections;

public class AP_FromVector4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector4[]   vs;
    [AP_OutPort] public float[]     xs;
    [AP_OutPort] public float[]     ys;    
    [AP_OutPort] public float[]     zs;
    [AP_OutPort] public float[]     ws;

    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        xs= Prelude.map_(xs, (v)=> v.x, vs);
        ys= Prelude.map_(xs, (v)=> v.y, vs);
        zs= Prelude.map_(xs, (v)=> v.z, vs);
        ws= Prelude.map_(xs, (v)=> v.w, vs);
    }

}
