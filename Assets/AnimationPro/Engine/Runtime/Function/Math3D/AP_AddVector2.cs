using UnityEngine;
using System.Collections;

public sealed class AP_AddVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[] xs;
    [AP_InPort]  public Vector2[] ys;
    [AP_OutPort] public Vector2[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AddVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AddVector2 instance= CreateInstance<AP_AddVector2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        os= Prelude.zipWith_(os, (x,y)=> x+y, xs, ys);
    }
}