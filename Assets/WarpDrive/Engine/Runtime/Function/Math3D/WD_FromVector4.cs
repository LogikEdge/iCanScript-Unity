using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public class WD_FromVector4 : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public Vector4[]   vs;
    [WD_OutPort] public float[]     xs;
    [WD_OutPort] public float[]     ys;    
    [WD_OutPort] public float[]     zs;
    [WD_OutPort] public float[]     ws;

    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        xs= Prelude.map_(xs, (v)=> v.x, vs);
        ys= Prelude.map_(xs, (v)=> v.y, vs);
        zs= Prelude.map_(xs, (v)=> v.z, vs);
        ws= Prelude.map_(xs, (v)=> v.w, vs);
    }

}
