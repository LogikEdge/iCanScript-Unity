using UnityEngine;
using System.Collections;

public sealed class AP_Or : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public bool[] xs;
    [AP_InPort]  public bool[] ys;
    [AP_OutPort] public bool[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Or CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Or instance= CreateInstance<AP_Or>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.zipWith_(os, (x,y)=> x|y, xs, ys);
    }
}
