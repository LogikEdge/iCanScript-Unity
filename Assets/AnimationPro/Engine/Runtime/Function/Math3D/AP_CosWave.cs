using UnityEngine;
using System.Collections;

public sealed class AP_CosWave : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort]  public float freq     = 1.0f;
    [AP_InPort]  public float amplitude= 1.0f;
    [AP_OutPort] public float wave;
    
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
        

    // ======================================================================
    // LIFETIME MANAGEMENT
    // ----------------------------------------------------------------------
    public static AP_CosWave CreateInstance(string _name, AP_Node _parent) {
        AP_CosWave instance= CreateInstance<AP_CosWave>();
        instance.Init(_name, _parent);
        return instance;
    }

    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void doExecute() {
        elapseTime+= Time.deltaTime;
        wave= amplitude * Mathf.Cos(twoPI * freq * elapseTime);
    }
}
