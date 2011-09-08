using UnityEngine;
using System.Collections;

public sealed class AP_LerpInt : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[] xs;
    [AP_InPort]  public int[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public int[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LerpInt CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_LerpInt instance= CreateInstance<AP_LerpInt>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> (int)(x+(y-x)*ratio), xs, ys, ratios);
    }
}
