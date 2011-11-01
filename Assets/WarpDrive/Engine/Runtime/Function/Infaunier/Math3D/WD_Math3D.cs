using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Math3D")]
public class WD_Math3D {
    [WD_Function(Icon="WD_CalculatorIcon.psd")] public static int     AddInt(int a, int b)             { return a+b; }
    [WD_Function(Icon="WD_CalculatorIcon.psd")] public static float   AddFloat(float a, float b)       { return a+b; }
    [WD_Function(Icon="WD_CalculatorIcon.psd")] public static Vector2 AddVector2(Vector2 a, Vector2 b) { return a+b; }
    [WD_Function(Icon="WD_CalculatorIcon.psd")] public static Vector3 AddVector3(Vector3 a, Vector3 b) { return a+b; }
    [WD_Function(Icon="WD_CalculatorIcon.psd")] public static Vector4 AddVector4(Vector4 a, Vector4 b) { return a+b; }

    [WD_Function(Icon="WD_SplitIcon.psd")] public static void    FromVector2(Vector2 v, out float x, out float y)                           { x= v.x; y= v.y; }
    [WD_Function(Icon="WD_SplitIcon.psd")] public static void    FromVector3(Vector3 v, out float x, out float y, out float z)              { x= v.x; y= v.y; z= v.z; }
    [WD_Function(Icon="WD_SplitIcon.psd")] public static void    FromVector4(Vector4 v, out float x, out float y, out float z, out float w) { x= v.x; y= v.y; z= v.z; w= v.w; }

    [WD_Function(Icon="WD_JoinIcon.psd")] public static Vector2 ToVector2(float x, float y)                   { return new Vector2(x,y); }
    [WD_Function(Icon="WD_JoinIcon.psd")] public static Vector3 ToVector3(float x, float y, float z)          { return new Vector3(x,y,z); }
    [WD_Function(Icon="WD_JoinIcon.psd")] public static Vector4 ToVector4(float x, float y, float z, float w) { return new Vector4(x,y,z,w); }

    [WD_Function] public static int     LerpInt(int v1, int v2, float ratio)                { return (int)(v1+(v2-v1)*ratio); }
    [WD_Function] public static float   LerpFloat(float v1, float v2, float ratio)          { return v1+(v2-v1)*ratio; }
    [WD_Function] public static Vector2 LerpVector2(Vector2 v1, Vector2 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    [WD_Function] public static Vector3 LerpVector3(Vector3 v1, Vector3 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    [WD_Function] public static Vector4 LerpVector4(Vector4 v1, Vector4 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    
    [WD_Function] public static float   Random(float scale)         { return scale*UnityEngine.Random.value; }
    [WD_Function] public static Vector2 RandomVector2(float scale)  { return scale*UnityEngine.Random.insideUnitCircle; }
    [WD_Function] public static Vector3 RandomVector3(float scale)  { return scale*UnityEngine.Random.insideUnitSphere; }
    
    [WD_Function] public static Vector2 ScaleVector2(float scale, Vector2 v) { return scale*v; }
    [WD_Function] public static Vector3 ScaleVector3(float scale, Vector3 v) { return scale*v; }
    [WD_Function] public static Vector4 ScaleVector4(float scale, Vector4 v) { return scale*v; }

    [WD_Function] public static Vector2 Scale2Vector2(float s1, float s2, Vector2 v) { return s1*s2*v; }
    [WD_Function] public static Vector3 Scale2Vector3(float s1, float s2, Vector3 v) { return s1*s2*v; }
    [WD_Function] public static Vector4 Scale2Vector4(float s1, float s2, Vector4 v) { return s1*s2*v; }
    
    [WD_Function] public static float MagnitudeVector2(Vector2 v)   { return v.magnitude; }
    [WD_Function] public static float MagnitudeVector3(Vector3 v)   { return v.magnitude; }
    [WD_Function] public static float MagnitudeVector4(Vector4 v)   { return v.magnitude; }
}
