using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Time", Icon= "iCS_ClockIcon.psd")]
public static class iCS_Time {
    [iCS_Function(Return="value*dt")]
    public static float timesDT(float value) {
        return Time.deltaTime*value;
    }
}
