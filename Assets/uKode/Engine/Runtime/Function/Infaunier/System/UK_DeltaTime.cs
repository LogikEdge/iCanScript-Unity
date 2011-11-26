using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="System")]
public sealed class UK_DeltaTime {
    [UK_Function(Return="dt", Icon= "UK_ClockIcon.psd")]
    public static float DeltaTime(out float invDt) {
        float dt= Time.deltaTime;
        invDt=  1.0f/dt;
        return dt;
    }
}
