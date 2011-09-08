using UnityEngine;
using System.Collections;

public sealed class AP_AddFloat : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[] xs;
    [AP_InPort]  public float[] ys;
    [AP_OutPort] public float[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AddFloat CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AddFloat instance= CreateInstance<AP_AddFloat>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}