using UnityEngine;
using System.Collections;

public sealed class AP_Scale2Vector4 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector4[] xs;
    [AP_InPort]  public float[] scales1;
    [AP_InPort]  public float[] scales2;
    [AP_OutPort] public Vector4[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Scale2Vector4 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Scale2Vector4 instance= CreateInstance<AP_Scale2Vector4>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        os= Prelude.zipWith_(os, (x,s1,s2)=> s1*s2*x, xs, scales1, scales2);
    }
}
