using UnityEngine;
using System.Collections;

public sealed class WD_SinWave : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public float freq= 1.0f;
    [WD_InPort]  public float amplitude= 1.0f;
    [WD_OutPort] public float wave;
    
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
    

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        elapseTime+= Time.deltaTime;
        wave= amplitude * Mathf.Sin(twoPI * freq * elapseTime);
    }

}