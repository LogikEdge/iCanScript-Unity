using UnityEngine;
using System.Collections;

public sealed class AP_AddVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector3[] xs;
    [AP_InPort]  public Vector3[] ys;
    [AP_OutPort] public Vector3[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AddVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AddVector3 instance= CreateInstance<AP_AddVector3>();
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