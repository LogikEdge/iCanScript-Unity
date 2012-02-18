using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Conditions")]
public class iCS_Logic {
    // Comparaison operations.
    [iCS_Function] public static bool IsZero(float a, out bool not)                           { bool result= Math3D.IsZero(a); not= !result; return result; }
    [iCS_Function] public static bool IsNotZero(float a, out bool not)                        { bool result= Math3D.IsNotZero(a); not= !result; return result; }
    [iCS_Function] public static bool IsEqual(float a, float b, out bool not)                 { bool result= Math3D.IsEqual(a,b); not= !result; return result; }
    [iCS_Function] public static bool IsNotEqual(float a, float b, out bool not)              { bool result= Math3D.IsNotEqual(a,b); not= !result; return result; }
    [iCS_Function] public static bool IsGreater(float value, float bias, out bool not)        { bool result= Math3D.IsGreater(value,bias); not= !result; return result; }
    [iCS_Function] public static bool IsSmaller(float value, float bias, out bool not)        { bool result= Math3D.IsSmaller(value,bias); not= !result; return result; }
    [iCS_Function] public static bool IsGreaterOrEqual(float value, float bias, out bool not) { bool result= Math3D.IsGreaterOrEqual(value,bias); not= !result; return result; }
    [iCS_Function] public static bool IsSmallerOrEqual(float value, float bias, out bool not) { bool result= Math3D.IsSmallerOrEqual(value,bias); not= !result; return result; }
}

[iCS_Class(Company="iCanScript", Package="Boolean")]
public class iCS_Boolean {
    [iCS_Function] public static bool And(bool a, bool b) { return a & b; }
    [iCS_Function] public static bool Or (bool a, bool b) { return a | b; }
    [iCS_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [iCS_Function] public static bool Not(bool a)         { return !a; }    
}