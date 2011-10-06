using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_CosWave : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort]  public float freq     = 1.0f;
    [WD_InPort]  public float amplitude= 1.0f;
    [WD_OutPort] public float wave;
    
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
        

    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        elapseTime+= Time.deltaTime;
        wave= amplitude * Mathf.Cos(twoPI * freq * elapseTime);
    }
}
