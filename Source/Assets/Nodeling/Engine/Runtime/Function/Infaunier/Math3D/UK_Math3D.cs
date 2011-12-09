using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="Math3D")]
public class UK_Math3D {
    [UK_Function(Icon="UK_CalculatorIcon.psd")] public static int     Add(int a, int b)             { return a+b; }
    [UK_Function(Icon="UK_CalculatorIcon.psd")] public static float   Add(float a, float b)       { return a+b; }
    [UK_Function(Icon="UK_CalculatorIcon.psd")] public static Vector2 Add(Vector2 a, Vector2 b) { return a+b; }
    [UK_Function(Icon="UK_CalculatorIcon.psd")] public static Vector3 Add(Vector3 a, Vector3 b) { return a+b; }
    [UK_Function(Icon="UK_CalculatorIcon.psd")] public static Vector4 Add(Vector4 a, Vector4 b) { return a+b; }

    [UK_Function(Icon="UK_SplitIcon.psd")] public static void    FromVector(Vector2 v, out float x, out float y)                           { x= v.x; y= v.y; }
    [UK_Function(Icon="UK_SplitIcon.psd")] public static void    FromVector(Vector3 v, out float x, out float y, out float z)              { x= v.x; y= v.y; z= v.z; }
    [UK_Function(Icon="UK_SplitIcon.psd")] public static void    FromVector(Vector4 v, out float x, out float y, out float z, out float w) { x= v.x; y= v.y; z= v.z; w= v.w; }

    [UK_Function(Icon="UK_JoinIcon.psd")] public static Vector2 ToVector(float x, float y)                   { return new Vector2(x,y); }
    [UK_Function(Icon="UK_JoinIcon.psd")] public static Vector3 ToVector(float x, float y, float z)          { return new Vector3(x,y,z); }
    [UK_Function(Icon="UK_JoinIcon.psd")] public static Vector4 ToVector(float x, float y, float z, float w) { return new Vector4(x,y,z,w); }

    [UK_Function] public static int     Lerp(int v1, int v2, float ratio)            { return (int)(v1+(v2-v1)*ratio); }
    [UK_Function] public static float   Lerp(float v1, float v2, float ratio)        { return v1+(v2-v1)*ratio; }
    [UK_Function] public static Vector2 Lerp(Vector2 v1, Vector2 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    [UK_Function] public static Vector3 Lerp(Vector3 v1, Vector3 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    [UK_Function] public static Vector4 Lerp(Vector4 v1, Vector4 v2, float ratio)    { return v1+(v2-v1)*ratio; }
    
    [UK_Function(Name="Random",Icon="UK_RandomIcon.png")] public static float   Random(float scale)         { return scale*UnityEngine.Random.value; }
    [UK_Function(Name="Random",Icon="UK_RandomIcon.png")] public static Vector2 RandomVector2(float scale)  { return scale*UnityEngine.Random.insideUnitCircle; }
    [UK_Function(Name="Random",Icon="UK_RandomIcon.png")] public static Vector3 RandomVector3(float scale)  { return scale*UnityEngine.Random.insideUnitSphere; }
    
    [UK_Function] public static Vector2 ScaleVector(float scale, Vector2 v) { return scale*v; }
    [UK_Function] public static Vector3 ScaleVector(float scale, Vector3 v) { return scale*v; }
    [UK_Function] public static Vector4 ScaleVector(float scale, Vector4 v) { return scale*v; }

    [UK_Function] public static Vector2 Scale2Vector(float s1, float s2, Vector2 v) { return s1*s2*v; }
    [UK_Function] public static Vector3 Scale2Vector(float s1, float s2, Vector3 v) { return s1*s2*v; }
    [UK_Function] public static Vector4 Scale2Vector(float s1, float s2, Vector4 v) { return s1*s2*v; }
    
    [UK_Function] public static float Magnitude(Vector2 v)   { return v.magnitude; }
    [UK_Function] public static float Magnitude(Vector3 v)   { return v.magnitude; }
    [UK_Function] public static float Magnitude(Vector4 v)   { return v.magnitude; }
}
