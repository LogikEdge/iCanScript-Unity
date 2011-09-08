using UnityEngine;
using System.Collections;

public class AP_Constant<TYPE,DERIVED> : AP_Action where DERIVED : AP_Constant<TYPE,DERIVED> {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public TYPE constant= default(TYPE);

    // ======================================================================
    // INITIALIZATION
    // ----------------------------------------------------------------------
    public static DERIVED CreateInstance(string _name, AP_Node _parent) {
        DERIVED instance= CreateInstance<DERIVED>();
        instance.Init(_name, _parent);
        return instance;
    }


    // ======================================================================
    // UPDATE
    // ----------------------------------------------------------------------
    protected override void Evaluate() {}
}
