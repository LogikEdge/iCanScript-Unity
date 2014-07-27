using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
public static class iCS_Time {
    [iCS_Function(Return="value*dt")]
    public static float ScaleByDeltaTime(float value= 1f) {
        return Time.deltaTime*value;
    }
    [iCS_Function(Return="value*dt")]
    public static Vector2 ScaleByDeltaTime(Vector2 value) {
        return Time.deltaTime*value;
    }
    [iCS_Function(Return="value*dt")]
    public static Vector3 ScaleByDeltaTime(Vector3 value) {
        return Time.deltaTime*value;
    }
    [iCS_Function(Return="value*dt")]
    public static Vector4 ScaleByDeltaTime(Vector4 value) {
        return Time.deltaTime*value;
    }
    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
    public static float LerpUsingDeltaTime(float v1, float v2, float ratio) {
        return Mathf.Lerp(v1,v2,ratio*Time.deltaTime);
    }
    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
    public static Vector2 LerpUsingDeltaTime(Vector2 v1, Vector2 v2, float ratio) {
        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
    }
    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
    public static Vector3 LerpUsingDeltaTime(Vector3 v1, Vector3 v2, float ratio) {
        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
    }
    [iCS_Function(Return="Lerp(v1,v2,ratio*dt)")]
    public static Vector4 LerpUsingDeltaTime(Vector4 v1, Vector4 v2, float ratio) {
        return Math3D.Lerp(v1,v2,ratio*Time.deltaTime);
    }
    [iCS_Function(Return="elapseTime")]
    public static float ComputeElapseTime(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="isElapsed")]
    public static bool IsTimeElapsed(float elapseTime, out bool isNotElapsed) {
        bool isElapsed= Time.time >= elapseTime;
        isNotElapsed= !isElapsed;
        return isElapsed;
    }
}
