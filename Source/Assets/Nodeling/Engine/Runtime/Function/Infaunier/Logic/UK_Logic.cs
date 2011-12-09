using UnityEngine;
using System.Collections;

[UK_Class(Company="Infaunier", Package="Logic")]
public class UK_Logic {
    // Pure logic operations.
    [UK_Function] public static bool And(bool a, bool b) { return a & b; }
    [UK_Function] public static bool Or (bool a, bool b) { return a | b; }
    [UK_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [UK_Function] public static bool Not(bool a)         { return !a; }

    // Comparaison operations.
    [UK_Function] public static bool IsZero(float a)                           { return Math3D.IsZero(a); }
    [UK_Function] public static bool IsNotZero(float a)                        { return Math3D.IsNotZero(a); }
    [UK_Function] public static bool IsEqual(float a, float b)                 { return Math3D.IsEqual(a,b); }
    [UK_Function] public static bool IsNotEqual(float a, float b)              { return Math3D.IsNotEqual(a,b); }
    [UK_Function] public static bool IsGreater(float value, float bias)        { return Math3D.IsGreater(value,bias); }
    [UK_Function] public static bool IsSmaller(float value, float bias)        { return Math3D.IsSmaller(value,bias); }
    [UK_Function] public static bool IsGreaterOrEqual(float value, float bias) { return Math3D.IsGreaterOrEqual(value,bias); }
    [UK_Function] public static bool IsSmallerOrEqual(float value, float bias) { return Math3D.IsSmallerOrEqual(value,bias); }

    // Decoders.
    [UK_Function] public static void Selector(int selector, out bool a, out bool b) {
        a= (selector & 1) == 0;
        b= (selector & 1) == 1;
    }
    [UK_Function] public static void Selector(int selector, out bool a, out bool b, out bool c, out bool d) {
        a= (selector & 3) == 0;
        b= (selector & 3) == 1;
        c= (selector & 3) == 2;
        d= (selector & 3) == 3;
    }
}
