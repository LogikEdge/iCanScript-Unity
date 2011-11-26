using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="Math3D")]
public sealed class UK_SinWave {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    float          elapseTime;
    const float    twoPI= 2.0f*Mathf.PI;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [UK_Function(Return="wave")]
    public float SinWave(float freq, float amplitude) {
        elapseTime+= Time.deltaTime;
        return amplitude * Mathf.Sin(twoPI * freq * elapseTime);
    }

}