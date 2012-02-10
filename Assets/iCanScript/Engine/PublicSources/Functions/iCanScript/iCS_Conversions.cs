using UnityEngine;
using System;
using System.Collections;

[iCS_Class(Company="Conversions", Package="bool->...")]
public static class iCS_ConversionsFromBool {
    [iCS_Conversion] public static int    ToInt(bool v)    { return v ? 1 : 0; }
    [iCS_Conversion] public static float  ToFloat(bool v)  { return v ? 1f : 0f; }
    [iCS_Conversion] public static string ToString(bool v) { return v ? "true" : "false"; }
}

[iCS_Class(Company="Conversions", Package="int->...")]
public static class iCS_ConversionsFromInt {
    [iCS_Conversion] public static bool   ToBool(int v)   { return v != 0; }
    [iCS_Conversion] public static float  ToFloat(int v)  { return (float)v; }
    [iCS_Conversion] public static string ToString(int v) { return v.ToString(); }
}


[iCS_Class(Company="Conversions", Package="float->...")]
public static class iCS_ConversionsFromFloat {
    [iCS_Conversion] public static int    ToInt(float v)    { return (int)v; }
    [iCS_Conversion] public static bool   ToBool(float v)   { return !Math3D.IsZero(v); }
    [iCS_Conversion] public static string ToString(float v)	{ return v.ToString(); }
}

[iCS_Class(Company="Conversions", Package="Vector2->...")]
public static class iCS_ConversionsFromVector2 {
    [iCS_Conversion] public static Vector3 ToVector3(Vector2 v) { return v; }
    [iCS_Conversion] public static Vector4 ToVector4(Vector2 v) { return v; }
    [iCS_Conversion] public static string  ToString(Vector2 v)  { return v.ToString(); }
}

[iCS_Class(Company="Conversions", Package="Vector3->...")]
public static class iCS_ConversionsFromVector3 {
    [iCS_Conversion] public static Vector2 ToVector2(Vector3 v) { return v; }
    [iCS_Conversion] public static Vector4 ToVector4(Vector3 v) { return v; }
    [iCS_Conversion] public static string ToString(Vector3 v)   { return v.ToString(); }
}

[iCS_Class(Company="Conversions", Package="Vector4->...")]
public static class iCS_ConversionsFromVector4 {
    [iCS_Conversion] public static Vector3 ToVector3(Vector4 v) { return v; }
    [iCS_Conversion] public static string ToString(Vector4 v)   { return v.ToString(); }
    [iCS_Conversion] public static Vector2 ToVector2(Vector4 v) { return v; }
}

[iCS_Class(Company="Conversions", Package="object->...")]
public static class iCS_ConversionsFromObject {
	[iCS_Conversion] public static string ToString(object v)  { return v.ToString(); }
}

[iCS_Class(Company="Conversions", Package="...[]->Array")]
public static class iCS_ConversionsToArray {
	[iCS_Conversion] public static Array ToArray(Vector3[] v)  { return v as Array; }
}
