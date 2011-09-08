using UnityEngine;
using System.Collections;

public sealed class AP_ScaleVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector3[] xs;
    [AP_InPort]  public float[] scales;
    [AP_OutPort] public Vector3[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ScaleVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ScaleVector3 instance= CreateInstance<AP_ScaleVector3>();
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
