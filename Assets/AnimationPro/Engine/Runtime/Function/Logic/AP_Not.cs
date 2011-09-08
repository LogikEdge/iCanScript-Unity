using UnityEngine;
using System.Collections;

public sealed class AP_Not : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public bool[] xs;
    [AP_OutPort] public bool[] os;
    
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Not CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_Not instance= CreateInstance<AP_Not>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        os= Prelude.map_(os, (x)=> !x, xs);
    }
}