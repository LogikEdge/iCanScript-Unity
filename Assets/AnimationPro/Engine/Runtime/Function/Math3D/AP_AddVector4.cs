using UnityEngine;
using System.Collections;

public sealed class AP_AddVector4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector4[] xs;
    [AP_InPort]  public Vector4[] ys;
    [AP_OutPort] public Vector4[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AddVector4 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AddVector4 instance= CreateInstance<AP_AddVector4>();
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