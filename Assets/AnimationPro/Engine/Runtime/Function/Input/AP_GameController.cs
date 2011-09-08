using UnityEngine;
using System.Collections;

public sealed class AP_GameController : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public Vector2    rawAnalog1;
    [AP_OutPort] public Vector2    analog1;
    [AP_InPort]  public float      speed= 1.0f;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        analog1= Time.deltaTime*speed*rawAnalog1;
    }

}
