using UnityEngine;
using System.Collections;

public sealed class AP_Transform : AP_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_InPort] public GameObject   gameObject= null;
    [AP_InPort] public Vector3      translation;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    protected override void Evaluate() {
        if(!IsValid) return;
        gameObject.transform.Translate(translation);
    }

    // ======================================================================
    // CONNECTIONS
    // ----------------------------------------------------------------------
    protected override bool doIsValid() { return gameObject != null; }

}

