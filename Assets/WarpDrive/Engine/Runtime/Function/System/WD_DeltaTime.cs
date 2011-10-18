using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="System")]
public sealed class WD_DeltaTime {
    [WD_Function(Return="dt", Icon= "Assets/WarpDrive/Editor/Resources/WD_ClockIcon.psd")]
    public static float DeltaTime(out float invDt) {
        float dt= Time.deltaTime;
        invDt=  1.0f/dt;
        return dt;
    }
}
