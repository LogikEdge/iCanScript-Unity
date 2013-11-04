using UnityEngine;
using Object=System.Object;

[iCS_Class(Company="iCanScript")]
public static class iCS_Conditions {
    // Comparaison operations.
    [iCS_Function(Return="If")]
    public static bool IsZero(float a, out bool Else)                           { bool result= Math3D.IsZero(a); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsNotZero(float a, out bool Else)                        { bool result= Math3D.IsNotZero(a); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsEqual(float a, float b, out bool Else)                 { bool result= Math3D.IsEqual(a,b); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsNotEqual(float a, float b, out bool Else)              { bool result= Math3D.IsNotEqual(a,b); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsGreater(float value, float bias, out bool Else)        { bool result= Math3D.IsGreater(value,bias); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsSmaller(float value, float bias, out bool Else)        { bool result= Math3D.IsSmaller(value,bias); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsGreaterOrEqual(float value, float bias, out bool Else) { bool result= Math3D.IsGreaterOrEqual(value,bias); Else= !result; return result; }
    [iCS_Function(Return="If")]
    public static bool IsSmallerOrEqual(float value, float bias, out bool Else) { bool result= Math3D.IsSmallerOrEqual(value,bias); Else= !result; return result; }
}

[iCS_Class(Company="iCanScript")]
public static class iCS_Boolean {
    [iCS_Function] public static bool And(bool a, bool b) { return a & b; }
    [iCS_Function] public static bool Or (bool a, bool b) { return a | b; }
    [iCS_Function] public static bool Xor(bool a, bool b) { return a ^ b; }
    [iCS_Function] public static bool Not(bool a)         { return !a; }    
}

[iCS_Class(Company="iCanScript")]
public static class iCS_Choice {
	[iCS_Function(Return="out")]
	public static float  Choice(float trueValue,  float falseValue,  bool sel) { return sel ? trueValue : falseValue; }
	[iCS_Function(Return="out")]
	public static int    Choice(int trueValue,    int falseValue,    bool sel) { return sel ? trueValue : falseValue; }
	[iCS_Function(Return="out")]
	public static string Choice(string trueValue, string falseValue, bool sel) { return sel ? trueValue : falseValue; }
	[iCS_Function(Return="out")]
	public static Object Choice(Object trueValue, Object falseValue, bool sel) { return sel ? trueValue : falseValue; }
}