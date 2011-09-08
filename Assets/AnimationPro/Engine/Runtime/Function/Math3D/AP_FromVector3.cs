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
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_FromVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_FromVector3 instance= CreateInstance<AP_FromVector3>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        xs= Prelude.map_(xs, (v)=> v.x, vs);
        ys= Prelude.map_(ys, (v)=> v.y, vs);
        zs= Prelude.map_(zs, (v)=> v.z, vs);
    }

}
