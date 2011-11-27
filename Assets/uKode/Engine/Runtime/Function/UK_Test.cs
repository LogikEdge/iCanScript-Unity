using UnityEngine;
using System;
using System.Collections;

[UK_Class(Company="TestCompany", Name="NewClassName")]
public sealed class UK_Test {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [UK_Function(Name="MySuperName", Icon="UK_MusicIcon.psd")]
    public static Prelude.Tuple<Vector2, Vector2> Evaluate(float speed= 1.0f) {
        Vector2 rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 analog1= Time.deltaTime*speed*rawAnalog1;
        return new Prelude.Tuple<Vector2, Vector2>(rawAnalog1, analog1);
    }
    [UK_Function]
    public static float Inc(float f, out float ret) { return (ret= f+1.0f); }
    [UK_Function]
    public void remove(float f) {}
    
    [UK_Conversion] public static float   Conversion(int i)     { return i; }
    [UK_Conversion] public static Vector2 Conversion(float f)   { return new Vector2(f,0); }
    [UK_Conversion] public static Vector3 Conversion(Vector2 v) { return new Vector3(v.x,v.y,0); }
    [UK_Conversion] public static Vector2 Conversion(Vector3 v) { return new Vector2(v.x,v.y); }

    [UK_Function] public static void PrintWorking() { Debug.Log("Working"); }
}

[UK_Class(Company="TestCompany", Icon="UK_CalculatorIcon.psd")]
public sealed class UK_Test2 {
    [UK_InPort]     public float x;
    [UK_OutPort]    public float z;
    [UK_InOutPort]  public Vector3 v3;

    public bool IsValid {
        [UK_Function] get { return true; }
        [UK_Function] set {}
    }

    [UK_Function(Return="m1", Icon="UK_JoystickIcon.psd")]
    public bool Method1(float x, Vector2 v) {
        return true;
    }

    [UK_Function(Return="m2", Icon="UK_MovieIcon.psd")]
    public float Method1(Vector2 v2, Vector3 v3) {
        return 1.0f;
    }
}

