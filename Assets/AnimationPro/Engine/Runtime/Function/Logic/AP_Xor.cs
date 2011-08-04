using UnityEngine;
using System.Collections;

public sealed class AP_Xor : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public bool[] xs;
    [AP_InPort]  public bool[] ys;
    [AP_OutPort] public bool[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Xor CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Xor instance= CreateInstance<AP_Xor>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        os= Prelude.zipWith_(os, (x,y)=> x^y, xs, ys);
    }
}
