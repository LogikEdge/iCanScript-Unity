using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class AP_ToVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float[]      xs;
    [AP_InPort]  public float[]      ys;
    [AP_InPort]  public float[]      zs;
    [AP_OutPort] public Vector3[]    vs;


    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_ToVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_ToVector3 instance= CreateInstance<AP_ToVector3>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }

    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        vs= Prelude.zipWith_(vs, (x,y,z)=> new Vector3(x,y,z), xs, ys, zs);
    }
}
