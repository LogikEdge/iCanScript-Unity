using UnityEngine;
using System.Collections;

public sealed class AP_GameController : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public Vector2    rawAnalog1;
    [AP_OutPort] public Vector2    analog1;
    [AP_InPort]  public float      speed;
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        rawAnalog1= new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        analog1= Time.deltaTime*speed*rawAnalog1;
    }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_GameController CreateInstance(string _name, AP_Node _parent) {
        AP_GameController controller= CreateInstance<AP_GameController>();
        controller.Init(_name, _parent);
        controller.speed= 1.0f;
        return controller;
    }

}
