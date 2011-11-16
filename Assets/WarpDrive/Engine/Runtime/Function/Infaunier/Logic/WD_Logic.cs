using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Logic")]
public class WD_Logic {
    // Pure logic operations.
    [WD_Function] public static bool And(bool a, bool b) { return a & b; }
    [WD_Function] public static bool Or (bool a, bool b) { return a | b; }
    [WD_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [WD_Function] public static bool Not(bool a)         { return !a; }

    // Comparaison operations.
    [WD_Function] public static bool IsZero(float a)                    { return Math3D.IsZero(a); }
    [WD_Function] public static bool IsNotZero(float a)                 { return Math3D.IsNotZero(a); }
    [WD_Function] public static bool IsEqual(float a, float b)          { return Math3D.IsEqual(a,b); }
    [WD_Function] public static bool IsNotEqual(float a, float b)       { return Math3D.IsNotEqual(a,b); }
    [WD_Function] public static bool IsGreater(float a, float b)        { return Math3D.IsGreater(a,b); }
    [WD_Function] public static bool IsSmaller(float a, float b)        { return Math3D.IsSmaller(a,b); }
    [WD_Function] public static bool IsGreaterOrEqual(float a, float b) { return Math3D.IsGreaterOrEqual(a,b); }
    [WD_Function] public static bool IsSmallerOrEqual(float a, float b) { return Math3D.IsSmallerOrEqual(a,b); }

    // Decoders.
    [WD_Function] public static void Selector(int selector, out bool a, out bool b) {
        a= (selector & 1) == 0;
        b= (selector & 1) == 1;
    }
    [WD_Function] public static void Selector(int selector, out bool a, out bool b, out bool c, out bool d) {
        a= (selector & 3) == 0;
        b= (selector & 3) == 1;
        c= (selector & 3) == 2;
        d= (selector & 3) == 3;
    }
}
