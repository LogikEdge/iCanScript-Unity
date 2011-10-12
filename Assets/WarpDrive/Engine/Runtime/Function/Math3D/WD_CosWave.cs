using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public sealed class WD_CosWave {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
        
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function(Return="wave")]
    public float CosWave(float freq, float amplitude) {
        elapseTime+= Time.deltaTime;
        return amplitude * Mathf.Cos(twoPI * freq * elapseTime);
    }
}
