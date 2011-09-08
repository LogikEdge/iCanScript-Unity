using UnityEngine;
using System.Collections;

public sealed class AP_ScaleVector4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector4[] xs;
    [AP_InPort]  public float[] scales;
    [AP_OutPort] public Vector4[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ScaleVector4 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ScaleVector4 instance= CreateInstance<AP_ScaleVector4>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,scale)=> scale*x, xs, scales);
    }
}
