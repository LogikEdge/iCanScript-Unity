using UnityEngine;
using System.Collections;

public sealed class AP_Scale2Vector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[] xs;
    [AP_InPort]  public float[] scales1;
    [AP_InPort]  public float[] scales2;
    [AP_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Scale2Vector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Scale2Vector2 instance= CreateInstance<AP_Scale2Vector2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,s1,s2)=> s1*s2*x, xs, scales1, scales2);
    }
}

