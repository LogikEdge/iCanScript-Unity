using UnityEngine;
using System.Collections;

[WD_Class]
public sealed class WD_Test {
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    public static Prelude.Tuple<Vector2, Vector2> Evaluate(float speed= 1.0f) {
        Vector2 rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector2 analog1= Time.deltaTime*speed*rawAnalog1;
        return new Prelude.Tuple<Vector2, Vector2>(rawAnalog1, analog1);
    }

    public static float Conversion(int i) { return i; }
    public static Vector2 Conversion(float f) { return new Vector2(f,0); }
    public static Vector3 Conversion(Vector2 v) { return new Vector3(v.x,v.y,0); }

}
