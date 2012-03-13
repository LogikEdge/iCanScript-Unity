using UnityEngine;
using System.Collections;

[iCS_Class]
public static class iCS_Conditions {
    // Comparaison operations.
    [iCS_Function(Return="True")]
    public static bool IsZero(float a, out bool False)                           { bool result= Math3D.IsZero(a); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsNotZero(float a, out bool False)                        { bool result= Math3D.IsNotZero(a); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsEqual(float a, float b, out bool False)                 { bool result= Math3D.IsEqual(a,b); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsNotEqual(float a, float b, out bool False)              { bool result= Math3D.IsNotEqual(a,b); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsGreater(float value, float bias, out bool False)        { bool result= Math3D.IsGreater(value,bias); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsSmaller(float value, float bias, out bool False)        { bool result= Math3D.IsSmaller(value,bias); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsGreaterOrEqual(float value, float bias, out bool False) { bool result= Math3D.IsGreaterOrEqual(value,bias); False= !result; return result; }
    [iCS_Function(Return="True")]
    public static bool IsSmallerOrEqual(float value, float bias, out bool False) { bool result= Math3D.IsSmallerOrEqual(value,bias); False= !result; return result; }
}

[iCS_Class]
public static class iCS_Boolean {
    [iCS_Function] public static bool And(bool a, bool b) { return a & b; }
    [iCS_Function] public static bool Or (bool a, bool b) { return a | b; }
    [iCS_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [iCS_Function] public static bool Not(bool a)         { return !a; }    
}