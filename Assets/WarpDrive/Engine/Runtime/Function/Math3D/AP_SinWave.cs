using UnityEngine;
using System.Collections;

public sealed class AP_SinWave : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float freq= 1.0f;
    [AP_InPort]  public float amplitude= 1.0f;
    [AP_OutPort] public float wave;
    
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
    

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_SinWave CreateInstance(string theFunctionName, AP_Node theParent) {
        AP_SinWave instance= CreateInstance<AP_SinWave>();
        instance.Init(theFunctionName, theParent);
        return instance;
    }
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        elapseTime+= Time.deltaTime;
        wave= amplitude * Mathf.Sin(twoPI * freq * elapseTime);
    }

}