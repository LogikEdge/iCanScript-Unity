using UnityEngine;
using System.Collections;

[WD_Class(Company="NewCompany", Package="Test")]
public sealed class WD_Test {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function(Name="MySuperName")]
    public static Prelude.Tuple<Vector2, Vector2> Evaluate(float speed= 1.0f) {
        Vector2 rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 analog1= Time.deltaTime*speed*rawAnalog1;
        return new Prelude.Tuple<Vector2, Vector2>(rawAnalog1, analog1);
    }
    [WD_Function]
    public static float Inc(float f, out float ret) { return (ret= f+1.0f); }
    [WD_Function]
    public void remove(float f) {}
    
    [WD_Conversion] public static float   Conversion(int i)     { return i; }
    [WD_Conversion] public static Vector2 Conversion(float f)   { return new Vector2(f,0); }
    [WD_Conversion] public static Vector3 Conversion(Vector2 v) { return new Vector3(v.x,v.y,0); }
}

[WD_Class(Company="NewCompany", Package="Test")]
public sealed class WD_Test2 {
    [WD_InPort]     public float x;
    [WD_OutPort]    public float z;

    public bool IsValid {
        [WD_Function] get { return true; }
        [WD_Function] set {}
    }

}

