using UnityEngine;
using System.Collections;

public sealed class AP_ToVector4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[]      xs;
    [AP_InPort]  public float[]      ys;
    [AP_InPort]  public float[]      zs;
    [AP_InPort]  public float[]      ws;
    [AP_OutPort] public Vector4[]    vs;

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ToVector4 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ToVector4 instance= CreateInstance<AP_ToVector4>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        vs= Prelude.zipWith_(vs, (x,y,z,w)=> new Vector4(x,y,z,w), xs, ys, zs, ws);
    }
}
