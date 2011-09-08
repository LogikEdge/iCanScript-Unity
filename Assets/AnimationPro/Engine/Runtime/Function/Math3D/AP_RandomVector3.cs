using UnityEngine;
using System.Collections;

public sealed class AP_RandomVector3 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public Vector3    value;
    [AP_InPort]  public float      scale= 1.0f;


    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_RandomVector3 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_RandomVector3 instance= CreateInstance<AP_RandomVector3>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        value= scale*Random.insideUnitCircle;
    }
}
