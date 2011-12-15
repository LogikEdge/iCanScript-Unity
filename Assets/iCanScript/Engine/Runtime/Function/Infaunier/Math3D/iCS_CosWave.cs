using UnityEngine;
using System.Collections;

[iCS_Class(Company="Infaunier", Package="Math3D")]
public sealed class iCS_CosWave {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
        
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [iCS_Function(Return="wave")]
    public float CosWave(float freq, float amplitude) {
        elapseTime+= Time.deltaTime;
        return amplitude * Mathf.Cos(twoPI * freq * elapseTime);
    }
}
