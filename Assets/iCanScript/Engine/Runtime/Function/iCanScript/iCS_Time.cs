using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Time")]
public sealed class iCS_Time {
    [iCS_Function(Icon= "iCS_ClockIcon.psd")]
    public static float ScaleDeltaTime(float scale) {
        return Time.deltaTime*scale;
    }
}
