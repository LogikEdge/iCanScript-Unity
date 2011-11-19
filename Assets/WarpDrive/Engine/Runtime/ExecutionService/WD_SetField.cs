using UnityEngine;
using System;
using System.Collections;

public class WD_SetField : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object                myThis;
    protected Action<object,object> mySetMethod;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_SetField(string name, object theThis, Action<object,object> setMethod, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
        myThis     = theThis;
        mySetMethod= setMethod;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        mySetMethod(myThis, myParameters[0]);
        MarkAsCurrent(frameId);
    }
}
