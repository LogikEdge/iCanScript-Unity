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
    protected override void doExecute() {
        if(!IsValid) return;
        gameObject.transform.Translate(translation);
    }

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static AP_Transform CreateInstance(string _name, AP_Node _parent) {
        AP_Transform transform= CreateInstance<AP_Transform>();
        transform.Init(_name, _parent);
        return transform;
    }


    // ======================================================================
    // CONNECTIONS
    // ----------------------------------------------------------------------
    protected override bool doIsValid() { return gameObject != null; }

}

