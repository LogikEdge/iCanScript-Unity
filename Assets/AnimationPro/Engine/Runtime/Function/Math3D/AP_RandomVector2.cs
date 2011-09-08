using UnityEngine;
using System.Collections;

public sealed class AP_RandomVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public Vector2    value;
    [AP_InPort]  public float      scale= 1.0f;


    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_RandomVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_RandomVector2 instance= CreateInstance<AP_RandomVector2>();
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
