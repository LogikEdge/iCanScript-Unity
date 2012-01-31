using UnityEngine;
using System.Collections;

[iCS_Class(Company="iCanScript", Package="Logic")]
public class iCS_Logic {
    // Pure logic operations.
    [iCS_Function] public static bool And(bool a, bool b) { return a & b; }
    [iCS_Function] public static bool Or (bool a, bool b) { return a | b; }
    [iCS_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [iCS_Function] public static bool Not(bool a)         { return !a; }

    // Comparaison operations.
    [iCS_Function] public static bool IsZero(float a)                           { return Math3D.IsZero(a); }
    [iCS_Function] public static bool IsNotZero(float a)                        { return Math3D.IsNotZero(a); }
    [iCS_Function] public static bool IsEqual(float a, float b)                 { return Math3D.IsEqual(a,b); }
    [iCS_Function] public static bool IsNotEqual(float a, float b)              { return Math3D.IsNotEqual(a,b); }
    [iCS_Function] public static bool IsGreater(float value, float bias)        { return Math3D.IsGreater(value,bias); }
    [iCS_Function] public static bool IsSmaller(float value, float bias)        { return Math3D.IsSmaller(value,bias); }
    [iCS_Function] public static bool IsGreaterOrEqual(float value, float bias) { return Math3D.IsGreaterOrEqual(value,bias); }
    [iCS_Function] public static bool IsSmallerOrEqual(float value, float bias) { return Math3D.IsSmallerOrEqual(value,bias); }

    // Decoders.
    [iCS_Function] public static void Selector(int selector, out bool a, out bool b) {
        a= (selector & 1) == 0;
        b= (selector & 1) == 1;
    }
    [iCS_Function] public static void Selector(int selector, out bool a, out bool b, out bool c, out bool d) {
        a= (selector & 3) == 0;
        b= (selector & 3) == 1;
        c= (selector & 3) == 2;
        d= (selector & 3) == 3;
    }
}
