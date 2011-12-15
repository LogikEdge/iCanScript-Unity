using UnityEngine;
using System;
using System.Collections;

[iCS_Class(Company="TestCompany", Name="NewClassName")]
public sealed class iCS_Test {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [iCS_Function(Name="MySuperName", Icon="iCS_MusicIcon.psd")]
    public static Prelude.Tuple<Vector2, Vector2> Evaluate(float speed= 1.0f) {
        Vector2 rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 analog1= Time.deltaTime*speed*rawAnalog1;
        return new Prelude.Tuple<Vector2, Vector2>(rawAnalog1, analog1);
    }
    [iCS_Function]
    public static float Inc(float f, out float ret) { return (ret= f+1.0f); }
    [iCS_Function]
    public void remove(float f) {}
    
    [iCS_Conversion] public static float   Conversion(int i)     { return i; }
    [iCS_Conversion] public static int     ConversionFI(float f) { return (int)f; }
    [iCS_Conversion] public static Vector2 Conversion(float f)   { return new Vector2(f,0); }
    [iCS_Conversion] public static Vector3 Conversion(Vector2 v) { return new Vector3(v.x,v.y,0); }
    [iCS_Conversion] public static Vector2 Conversion(Vector3 v) { return new Vector2(v.x,v.y); }

    [iCS_Function] public static void PrintWorking() { Debug.Log("Working"); }
}

[iCS_Class(Company="TestCompany", Icon="iCS_CalculatorIcon.psd")]
public sealed class iCS_Test2 {
    [iCS_InPort]     public float x;
    [iCS_OutPort]    public float z;
    [iCS_InOutPort]  public Vector3 v3;

    public bool IsValid {
        [iCS_Function] get { return true; }
        [iCS_Function] set {}
    }

    [iCS_Function(Return="m1", Icon="iCS_JoystickIcon.psd")]
    public bool Method1(float x, Vector2 v) {
        return true;
    }

    [iCS_Function(Return="m2", Icon="iCS_MovieIcon.psd")]
    public float Method1(Vector2 v2, Vector3 v3) {
        return 1.0f;
    }
}

