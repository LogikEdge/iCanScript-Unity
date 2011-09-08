using UnityEngine;
using System.Collections;

public sealed class AP_ScaleVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[] xs;
    [AP_InPort]  public float[] scales;
    [AP_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ScaleVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ScaleVector2 instance= CreateInstance<AP_ScaleVector2>();
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
