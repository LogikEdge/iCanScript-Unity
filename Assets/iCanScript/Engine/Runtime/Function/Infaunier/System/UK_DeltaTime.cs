using UnityEngine;
using System.Collections;

[iCS_Class(Company="Infaunier", Package="System")]
public sealed class iCS_DeltaTime {
    [iCS_Function(Return="dt", Icon= "iCS_ClockIcon.psd")]
    public static float DeltaTime(out float invDt) {
        float dt= Time.deltaTime;
        invDt=  1.0f/dt;
        return dt;
    }
}
