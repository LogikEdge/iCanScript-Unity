using UnityEngine;
using System;
using System.Collections;

public class WD_GetField : WD_FunctionBase {
    // ======================================================================
    // Properties
    // ----------------------------------------------------------------------
    protected object                myThis;
    protected Func<object,object>   myGetMethod;

    // ======================================================================
    // Creation/Destruction
    // ----------------------------------------------------------------------
    public WD_GetField(string name, object theThis, Func<object,object> getMethod, object[] parameters, bool[] paramIsOuts) : base(name, parameters, paramIsOuts) {
        myThis     = theThis;
        myGetMethod= getMethod;
    }
    
    // ======================================================================
    // Execution
    // ----------------------------------------------------------------------
    protected override void DoExecute(int frameId) {
        // Execute function
        myParameters[0]= myGetMethod(myThis);
        MarkAsCurrent(frameId);
    }
}
