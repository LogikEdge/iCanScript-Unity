using UnityEngine;
using System.Collections;

public sealed class WD_Transform : WD_Function {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort] public GameObject   gameObject= null;
    [WD_InPort] public Vector3      translation;
    
    
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

