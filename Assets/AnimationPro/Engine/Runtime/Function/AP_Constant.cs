using UnityEngine;
using System.Collections;

public class AP_Constant<TYPE,DERIVED> : AP_Action where DERIVED : AP_Constant<TYPE,DERIVED> {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [AP_OutPort] public TYPE constant= default(TYPE);

    // ======================================================================
    // UPDATE
    // ----------------------------------------------------------------------
    protected override void Evaluate() {}
}
