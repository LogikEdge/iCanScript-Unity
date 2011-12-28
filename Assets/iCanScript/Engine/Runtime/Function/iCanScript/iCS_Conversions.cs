using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Type Conversions", Name="Conversions")]
public static class iCS_Conversions {
    // ToInt
    [iCS_Conversion] public static int   ToInt(bool v)  { return v ? 1 : 0; }
    [iCS_Conversion] public static int   ToInt(float v) { return (int)v; }

    // ToFloat
    [iCS_Conversion] public static float ToFloat(bool v) { return v ? 1f : 0f; }
    [iCS_Conversion] public static float ToFloat(int v)  { return (float)v; }

    // ToBool
    [iCS_Conversion] public static bool  ToBool(int v)   { return v != 0; }
    [iCS_Conversion] public static bool  ToBool(float v) { return !Math3D.IsZero(v); }

    // ToVector2
    [iCS_Conversion] public static Vector2 ToVector2(Vector3 v) { return v; }
    [iCS_Conversion] public static Vector2 ToVector2(Vector4 v) { return v; }

    // ToVector3
    [iCS_Conversion] public static Vector3 ToVector3(Vector2 v) { return v; }
    [iCS_Conversion] public static Vector3 ToVector3(Vector4 v) { return v; }

    // ToVector3
    [iCS_Conversion] public static Vector4 ToVector4(Vector2 v) { return v; }
    [iCS_Conversion] public static Vector4 ToVector4(Vector3 v) { return v; }

    // ToString
    [iCS_Conversion] public static string ToString(object v)  { return v.ToString(); }
    [iCS_Conversion] public static string ToString(int v)     { return v.ToString(); }
    [iCS_Conversion] public static string ToString(float v)   { return v.ToString(); }
    [iCS_Conversion] public static string ToString(Vector2 v) { return v.ToString(); }
    [iCS_Conversion] public static string ToString(Vector3 v) { return v.ToString(); }
    [iCS_Conversion] public static string ToString(Vector4 v) { return v.ToString(); }

	[iCS_Function] public static object Fred(string s) { return s; }
}
