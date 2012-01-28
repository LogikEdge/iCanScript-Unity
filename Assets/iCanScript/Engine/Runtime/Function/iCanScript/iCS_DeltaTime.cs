using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Time")]
public sealed class iCS_DeltaTime {
    [iCS_Function(Return="dt", Icon= "iCS_ClockIcon.psd")]
    public static float DeltaTime(out float invDt) {
        float dt= Time.deltaTime;
        invDt=  1.0f/dt;
        return dt;
    }
    [iCS_Function]
    public static float ScaleDeltaTime(float scale) {
        return Time.deltaTime*scale;
    }
}
