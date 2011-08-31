using UnityEngine;
using System.Collections;

public sealed class AP_LerpVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector3[] xs;
    [AP_InPort]  public Vector3[] ys;
    [AP_InPort]  public float[] ratios;
    [AP_OutPort] public Vector3[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_LerpVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_LerpVector3 instance= CreateInstance<AP_LerpVector3>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        os= Prelude.zipWith_(os, (x,y,ratio)=> x+(y-x)*ratio, xs, ys, ratios);
    }
}
