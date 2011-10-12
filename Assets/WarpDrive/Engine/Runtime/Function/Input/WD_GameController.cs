using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="Input")]
public sealed class WD_GameController : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public Vector2    rawAnalog1;
    [WD_OutPort] public Vector2    analog1;
    [WD_InPort]  public float      speed= 1.0f;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        analog1= Time.deltaTime*speed*rawAnalog1;
    }
}
