using UnityEngine;
using System.Collections;

public class WD_Constant<TYPE,DERIVED> : WD_Action where DERIVED : WD_Constant<TYPE,DERIVED> {
    // ======================================================================
    // PROPERTIES
    // ----------------------------------------------------------------------
    [WD_OutPort] public TYPE constant= default(TYPE);

    // ======================================================================
    // UPDATE
    // ----------------------------------------------------------------------
    public override void Evaluate() {}
}
