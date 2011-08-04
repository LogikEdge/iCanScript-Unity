using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AP_ToVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[]      xs;
    [AP_InPort]  public float[]      ys;
    [AP_OutPort] public Vector2[]    vs;
        
    
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ToVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ToVector2 instance= CreateInstance<AP_ToVector2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        vs= Prelude.zipWith_(vs, (x,y)=> new Vector2(x,y), xs, ys);
    }
}
