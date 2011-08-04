using UnityEngine;
using System.Collections;

public sealed class AP_AddInt : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public int[] xs;
    [AP_InPort]  public int[] ys;
    [AP_OutPort] public int[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_AddInt CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_AddInt instance= CreateInstance<AP_AddInt>();
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