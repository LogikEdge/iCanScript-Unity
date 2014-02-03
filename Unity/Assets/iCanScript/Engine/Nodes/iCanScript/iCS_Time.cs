using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
public static class iCS_Time {
    [iCS_Function(Return="value*dt")]
    public static float ScaleByDeltaTime(float value= 1f) {
        return Time.deltaTime*value;
    }
}
