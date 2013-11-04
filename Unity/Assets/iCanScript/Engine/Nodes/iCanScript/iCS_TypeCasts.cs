using UnityEngine;
using System;
using System.Collections;

[iCS_Class(Company="iCanScript")]
public static class iCS_TypeCasts {
    // To Bool.
    [iCS_TypeCast] public static bool   ToBool(object o)    { return o != null; }
    [iCS_TypeCast] public static bool   ToBool(int v)       { return v != 0; }
    [iCS_TypeCast] public static bool   ToBool(float v)     { return Math3D.IsNotZero(v); }
    [iCS_TypeCast] public static bool   ToBool(string s)    { return s != null && s != ""; }
    [iCS_TypeCast] public static bool   ToBool(Vector2 v)   { return Math3D.IsNotZero(v); }
    [iCS_TypeCast] public static bool   ToBool(Vector3 v)   { return Math3D.IsNotZero(v); }
    [iCS_TypeCast] public static bool   ToBool(Vector4 v)   { return Math3D.IsNotZero(v); }
    
    // To Int.
    [iCS_TypeCast] public static int    ToInt(bool v)   { return v ? 1 : 0; }
    [iCS_TypeCast] public static int    ToInt(float v)  { return (int)v; }

    // To Float.
    [iCS_TypeCast] public static float  ToFloat(bool v) { return v ? 1f : 0f; }
    [iCS_TypeCast] public static float  ToFloat(int v)  { return (float)v; }
    
    // To String.
	[iCS_TypeCast] public static string ToString(object v)      { return v.ToString(); }
    [iCS_TypeCast] public static string ToString(bool v)        { return v ? "true" : "false"; }
    [iCS_TypeCast] public static string ToString(int v)         { return v.ToString(); }
    [iCS_TypeCast] public static string ToString(float v)       { return v.ToString(); }
    [iCS_TypeCast] public static string ToString(Vector2 v)     { return v.ToString(); }
    [iCS_TypeCast] public static string ToString(Vector3 v)     { return v.ToString(); }
    [iCS_TypeCast] public static string ToString(Vector4 v)     { return v.ToString(); }

    // To Vector2.
    [iCS_TypeCast] public static Vector2 ToVector2(Vector3 v)   { return v; }
    [iCS_TypeCast] public static Vector2 ToVector2(Vector4 v)   { return v; }

    // To Vector3.
    [iCS_TypeCast] public static Vector3 ToVector3(Vector2 v)   { return v; }
    [iCS_TypeCast] public static Vector3 ToVector3(Vector4 v)   { return v; }

    // To Vector4.
    [iCS_TypeCast] public static Vector4 ToVector4(Vector2 v)   { return v; }
    [iCS_TypeCast] public static Vector4 ToVector4(Vector3 v)   { return v; }

    // To ... (usefull automatic conversions)
    [iCS_TypeCast] public static CharacterController ToCharacterController(GameObject go) { return go.GetComponent(typeof(CharacterController)) as CharacterController; }
    [iCS_TypeCast] public static Transform           ToTransform(GameObject go)           { return go.transform; }	
}


