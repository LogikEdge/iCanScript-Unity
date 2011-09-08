using UnityEngine;
using System.Collections;

public sealed class AP_LerpVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[] xs;
    [AP_InPort]  public Vector2[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LerpVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_LerpVector2 instance= CreateInstance<AP_LerpVector2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> x+(y-x)*ratio, xs, ys, ratios);
    }
}
