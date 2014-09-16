using UnityEngine;
using System.Collections;

//[iCS_Class(Company="iCanScript", Icon= "iCS_ClockIcon.psd")]
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
    [iCS_Function(Return="endTime")]
    public static float ComputeElapseTime(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="endTime")]
    public static float StartTimer(float time) {
        return Time.time+time;
    }
    [iCS_Function(Return="isElapsed")]
    public static bool IsTimerElapsed(float endTime, out bool isNotElapsed) {
        bool isElapsed= Time.time >= endTime;
        isNotElapsed= !isElapsed;
        return isElapsed;
    }
    
    // =======================================================================
    static float ourLastFrameTime= 0f;
    static int   ourFrameCount= -1000;
    static float ourSmoothIntervalTime= 0;
    public static float smoothIntervalTime {
        [iCS_Function]
        get {
            int frameCount= Time.frameCount;
            if(ourFrameCount != frameCount) {
                ourFrameCount= frameCount;
                float time= Time.time;
                float deltaTime= Time.smoothDeltaTime;
                float interval= time-ourLastFrameTime;
                ourLastFrameTime= time;
                if(interval > deltaTime*4f) {
                    ourSmoothIntervalTime= Mathf.Lerp(ourSmoothIntervalTime, deltaTime, 0.1f);
                }
                else {
                    ourSmoothIntervalTime= Mathf.Lerp(ourSmoothIntervalTime, interval, 0.1f);
                }
            }
            return ourSmoothIntervalTime;
        }
    }
}
