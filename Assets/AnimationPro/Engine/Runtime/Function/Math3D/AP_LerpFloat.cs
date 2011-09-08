using UnityEngine;
using System.Collections;

public sealed class AP_LerpFloat : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[] xs;
    [AP_InPort]  public float[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public float[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LerpFloat CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_LerpFloat instance= CreateInstance<AP_LerpFloat>();
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
