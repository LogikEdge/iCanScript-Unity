using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
public static class iCS_Timer {
    [iCS_Function(Return="elapseTime")]
    public static float ComputeElapseTime(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="elapseTime")]
    public static float StartTimer(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="isElapsed")]
    public static bool IsTimeElapsed(float elapseTime, out bool isNotElapsed) {
        bool isElapsed= Time.time >= elapseTime;
        isNotElapsed= !isElapsed;
        return isElapsed;
    }
}
