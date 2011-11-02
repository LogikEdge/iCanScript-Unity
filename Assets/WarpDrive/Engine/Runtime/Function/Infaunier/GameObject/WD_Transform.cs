using UnityEngine;
using System.Collections;

[WD_Class(Company="Infaunier", Package="GameObject", Name="Transform")]
public sealed class WD_Transform {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_InPort] public GameObject   gameObject= null;
    [WD_InPort] public Vector3      translation;
    
    
    // ======================================================================
    // EXECUTION
    // ----------------------------------------------------------------------
    [WD_Function]
    public override void Evaluate() {
        if(!IsValid) return;
        gameObject.transform.Translate(translation);
    }

    // ======================================================================
    // CONNECTIONS
    // ----------------------------------------------------------------------
    protected override bool doIsValid() { return gameObject != null; }

}

