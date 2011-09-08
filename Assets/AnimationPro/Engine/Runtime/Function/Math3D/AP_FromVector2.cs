using UnityEngine;
using System.Collections;

public class AP_FromVector2 : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public Vector2[]   vs;
    [AP_OutPort] public float[]     xs;
    [AP_OutPort] public float[]     ys;    

        
    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_FromVector2 CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_FromVector2 instance= CreateInstance<AP_FromVector2>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }


    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        xs= Prelude.map_(xs, (vec)=> vec.x, vs);
        ys= Prelude.map_(ys, (vec)=> vec.y, vs);
    }

}
