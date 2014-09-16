using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Library= "Time", Icon= "iCS_ClockIcon.psd")]
public static class iCS_Timer {
    [iCS_Function(Return="endTime")]
    public static float StartTimer(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="isElapsed")]
    public static bool IsTimeElapsed(float endTime, out bool isNotElapsed) {
        bool isElapsed= Time.time >= endTime;
        isNotElapsed= !isElapsed;
        return isElapsed;
    }
    [iCS_Function(Return="remainingTime")]
    public static float RemainingTime(float endTime) {
        var remainingTime= Time.time-endTime;
        return remainingTime > 0 ? remainingTime : 0f; 
    }
}
